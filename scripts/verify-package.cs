#:property RestorePackagesWithLockFile=false

using System.IO.Compression;
using System.Reflection.Metadata;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Xml.Linq;

if (args.Length < 1)
{
    PrintUsage();
    return 1;
}

var packageDirectory = Path.GetFullPath(args[0]);
var requireSourceLink = false;
var packageId = "Brazilian.PrimitivesTypes";
string? expectedVersion = null;
string? expectedDependency = null;

for (var index = 1; index < args.Length; index++)
{
    switch (args[index])
    {
        case "--require-source-link":
            requireSourceLink = true;
            break;
        case "--package-id" when index + 1 < args.Length:
            packageId = args[++index];
            break;
        case "--expected-version" when index + 1 < args.Length:
            expectedVersion = args[++index];
            break;
        case "--expected-dependency" when index + 1 < args.Length:
            expectedDependency = args[++index];
            break;
        default:
            PrintUsage();
            return 1;
    }
}

var nupkg = FindPackageById(packageDirectory, packageId);
var symbolsPath = Path.ChangeExtension(nupkg, ".snupkg");
if (!File.Exists(symbolsPath))
{
    throw new InvalidOperationException($"Pacote de símbolos não encontrado: {Path.GetFileName(symbolsPath)}");
}

using var packageArchive = ZipFile.OpenRead(nupkg);
var nuspecEntry = packageArchive.Entries.Single(entry =>
    entry.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase));

using var nuspecStream = nuspecEntry.Open();
var nuspec = XDocument.Load(nuspecStream);
var ns = nuspec.Root!.Name.Namespace;
var metadata = nuspec.Root.Element(ns + "metadata")
    ?? throw new InvalidOperationException("Metadados do .nuspec não encontrados.");

AssertEqual(packageId, metadata.Element(ns + "id")?.Value, "PackageId");
AssertNotBlank(metadata.Element(ns + "description")?.Value, "Description");
AssertEntryExists(packageArchive, "README.md");

if (!string.IsNullOrWhiteSpace(expectedVersion))
{
    AssertEqual(expectedVersion, metadata.Element(ns + "version")?.Value, "Version");
}

if (!string.IsNullOrWhiteSpace(expectedDependency))
{
    AssertDependency(metadata, ns, expectedDependency);
}

var assemblyEntryPath = $"lib/net10.0/{packageId}.dll";
var assemblyEntry = packageArchive.GetEntry(assemblyEntryPath)
    ?? throw new InvalidOperationException($"Assembly principal não encontrado no pacote: {assemblyEntryPath}");
AssertEntryExists(packageArchive, $"lib/net10.0/{packageId}.xml");

using var assemblyStream = new MemoryStream();
using (var entryStream = assemblyEntry.Open())
{
    entryStream.CopyTo(assemblyStream);
}

assemblyStream.Position = 0;
using var peReader = new PEReader(assemblyStream);
var assemblyMetadata = peReader.GetMetadataReader();
var assemblyDefinition = assemblyMetadata.GetAssemblyDefinition();
var assemblyVersion = assemblyDefinition.Version.ToString();
var fileVersion = GetAssemblyStringAttribute(
    assemblyMetadata,
    "System.Reflection.AssemblyFileVersionAttribute");
var informationalVersion = GetAssemblyStringAttribute(
    assemblyMetadata,
    "System.Reflection.AssemblyInformationalVersionAttribute");

if (!string.IsNullOrWhiteSpace(expectedVersion))
{
    var expectedAssemblyVersion = ToAssemblyVersion(expectedVersion);
    AssertEqual(expectedAssemblyVersion, assemblyVersion, "AssemblyVersion");
    AssertEqual(expectedAssemblyVersion, fileVersion, "FileVersion");
    AssertInformationalVersion(expectedVersion, informationalVersion);
}

var repository = metadata.Element(ns + "repository");
var repositoryUrl = repository?.Attribute("url")?.Value;
var repositoryCommit = repository?.Attribute("commit")?.Value;

if (repository is not null)
{
    AssertEqual("git", repository.Attribute("type")?.Value, "RepositoryType");
}

if (requireSourceLink)
{
    if (repository is null)
    {
        throw new InvalidOperationException("Metadado repository não encontrado no .nuspec.");
    }

    AssertNotBlank(repositoryUrl, "RepositoryUrl");
    AssertNotBlank(repositoryCommit, "RepositoryCommit");
}

using var symbolsArchive = ZipFile.OpenRead(symbolsPath);
var pdbEntryPath = $"lib/net10.0/{packageId}.pdb";
var pdbEntry = symbolsArchive.GetEntry(pdbEntryPath)
    ?? throw new InvalidOperationException($"PDB não encontrado no .snupkg: {pdbEntryPath}");

using var pdbStream = new MemoryStream();
using (var entryStream = pdbEntry.Open())
{
    entryStream.CopyTo(pdbStream);
}

pdbStream.Position = 0;
using var provider = MetadataReaderProvider.FromPortablePdbStream(pdbStream);
var reader = provider.GetMetadataReader();
var sourceLinkId = new Guid("CC110556-A091-4D38-9FEC-25AB9A351A6A");
string? sourceLinkJson = null;

foreach (var handle in reader.CustomDebugInformation)
{
    var information = reader.GetCustomDebugInformation(handle);
    if (reader.GetGuid(information.Kind) != sourceLinkId)
    {
        continue;
    }

    sourceLinkJson = Encoding.UTF8.GetString(reader.GetBlobBytes(information.Value));
    break;
}

if (requireSourceLink)
{
    AssertNotBlank(sourceLinkJson, "Source Link JSON");
}

if (!string.IsNullOrWhiteSpace(sourceLinkJson)
    && !sourceLinkJson.Contains("raw.githubusercontent.com", StringComparison.OrdinalIgnoreCase))
{
    throw new InvalidOperationException(
        $"Source Link não aponta para conteúdo versionado do GitHub: {sourceLinkJson}");
}

Console.WriteLine($"Pacote validado: {Path.GetFileName(nupkg)}");
Console.WriteLine($"Símbolos validados: {Path.GetFileName(symbolsPath)}");

if (!string.IsNullOrWhiteSpace(expectedVersion))
{
    Console.WriteLine($"Versão validada: {expectedVersion}");
    Console.WriteLine($"AssemblyVersion validada: {assemblyVersion}");
    Console.WriteLine($"FileVersion validada: {fileVersion}");
    Console.WriteLine($"InformationalVersion validada: {informationalVersion}");
}

if (!string.IsNullOrWhiteSpace(repositoryUrl))
{
    Console.WriteLine($"RepositoryUrl: {repositoryUrl}");
}
else
{
    Console.WriteLine("RepositoryUrl não disponível neste contexto de build.");
}

if (!string.IsNullOrWhiteSpace(sourceLinkJson))
{
    Console.WriteLine("Source Link encontrado no PDB portátil.");
}
else
{
    Console.WriteLine("Source Link não disponível neste contexto de build.");
}

return 0;

static string FindPackageById(string packageDirectory, string expectedPackageId)
{
    var matches = new List<string>();

    foreach (var path in Directory.EnumerateFiles(packageDirectory, "*.nupkg"))
    {
        if (path.EndsWith(".snupkg", StringComparison.OrdinalIgnoreCase))
        {
            continue;
        }

        using var archive = ZipFile.OpenRead(path);
        var nuspecEntry = archive.Entries.Single(entry =>
            entry.FullName.EndsWith(".nuspec", StringComparison.OrdinalIgnoreCase));
        using var stream = nuspecEntry.Open();
        var document = XDocument.Load(stream);
        var ns = document.Root!.Name.Namespace;
        var actualPackageId = document.Root.Element(ns + "metadata")?.Element(ns + "id")?.Value;

        if (string.Equals(expectedPackageId, actualPackageId, StringComparison.Ordinal))
        {
            matches.Add(path);
        }
    }

    return matches.Count switch
    {
        1 => matches[0],
        0 => throw new InvalidOperationException($"Pacote '{expectedPackageId}' não encontrado em {packageDirectory}."),
        _ => throw new InvalidOperationException($"Mais de um pacote '{expectedPackageId}' foi encontrado em {packageDirectory}.")
    };
}

static void PrintUsage()
{
    Console.Error.WriteLine(
        "Uso: dotnet run --file scripts/verify-package.cs -- <diretorio-de-pacotes> "
        + "[--package-id <id>] [--require-source-link] [--expected-version <versao>] "
        + "[--expected-dependency <id>]");
}

static void AssertEntryExists(ZipArchive archive, string path)
{
    if (archive.GetEntry(path) is null)
    {
        throw new InvalidOperationException($"Entrada obrigatória ausente no pacote: {path}");
    }
}

static void AssertDependency(XElement metadata, XNamespace ns, string expectedDependency)
{
    var dependencyExists = metadata
        .Descendants(ns + "dependency")
        .Any(dependency => string.Equals(
            dependency.Attribute("id")?.Value,
            expectedDependency,
            StringComparison.Ordinal));

    if (!dependencyExists)
    {
        throw new InvalidOperationException($"Dependência obrigatória ausente no pacote: {expectedDependency}");
    }
}

static void AssertEqual(string expected, string? actual, string field)
{
    if (!string.Equals(expected, actual, StringComparison.Ordinal))
    {
        throw new InvalidOperationException(
            $"{field}: esperado '{expected}', obtido '{actual ?? "<null>"}'.");
    }
}

static void AssertNotBlank(string? value, string field)
{
    if (string.IsNullOrWhiteSpace(value))
    {
        throw new InvalidOperationException($"{field} não pode ser vazio.");
    }
}

static string ToAssemblyVersion(string semVer)
{
    var coreVersion = semVer.Split('-', 2)[0];
    if (!Version.TryParse(coreVersion, out var parsed) || parsed.Build < 0)
    {
        throw new InvalidOperationException($"Versão SemVer inválida para metadata de assembly: {semVer}");
    }

    return new Version(parsed.Major, parsed.Minor, parsed.Build, 0).ToString();
}

static void AssertInformationalVersion(string expectedVersion, string? actual)
{
    AssertNotBlank(actual, "InformationalVersion");

    if (!actual!.StartsWith(expectedVersion, StringComparison.Ordinal))
    {
        throw new InvalidOperationException(
            $"InformationalVersion: esperado prefixo '{expectedVersion}', obtido '{actual}'.");
    }

    var suffix = actual[expectedVersion.Length..];
    if (suffix.Length > 0 && !suffix.StartsWith('+'))
    {
        throw new InvalidOperationException(
            $"InformationalVersion: sufixo inesperado após '{expectedVersion}': '{suffix}'.");
    }
}

static string? GetAssemblyStringAttribute(MetadataReader reader, string expectedTypeName)
{
    var assembly = reader.GetAssemblyDefinition();

    foreach (var handle in assembly.GetCustomAttributes())
    {
        var attribute = reader.GetCustomAttribute(handle);
        if (!string.Equals(GetAttributeTypeName(reader, attribute), expectedTypeName, StringComparison.Ordinal))
        {
            continue;
        }

        var valueReader = reader.GetBlobReader(attribute.Value);
        if (valueReader.ReadUInt16() != 1)
        {
            throw new InvalidOperationException($"Formato inválido do atributo {expectedTypeName}.");
        }

        return valueReader.ReadSerializedString();
    }

    return null;
}

static string? GetAttributeTypeName(MetadataReader reader, CustomAttribute attribute)
{
    EntityHandle typeHandle = attribute.Constructor.Kind switch
    {
        HandleKind.MemberReference => reader.GetMemberReference((MemberReferenceHandle)attribute.Constructor).Parent,
        HandleKind.MethodDefinition => reader.GetMethodDefinition((MethodDefinitionHandle)attribute.Constructor).GetDeclaringType(),
        _ => default
    };

    return typeHandle.Kind switch
    {
        HandleKind.TypeReference => GetTypeReferenceName(reader, (TypeReferenceHandle)typeHandle),
        HandleKind.TypeDefinition => GetTypeDefinitionName(reader, (TypeDefinitionHandle)typeHandle),
        _ => null
    };
}

static string GetTypeReferenceName(MetadataReader reader, TypeReferenceHandle handle)
{
    var type = reader.GetTypeReference(handle);
    var ns = reader.GetString(type.Namespace);
    var name = reader.GetString(type.Name);
    return string.IsNullOrEmpty(ns) ? name : $"{ns}.{name}";
}

static string GetTypeDefinitionName(MetadataReader reader, TypeDefinitionHandle handle)
{
    var type = reader.GetTypeDefinition(handle);
    var ns = reader.GetString(type.Namespace);
    var name = reader.GetString(type.Name);
    return string.IsNullOrEmpty(ns) ? name : $"{ns}.{name}";
}
