using System.Reflection;
using Xunit;

namespace Brazilian.PrimitivesTypes.Tests;

public sealed class ArchitectureDependencyTests
{
    private static readonly string[] PersistenceAssemblyPrefixes =
    [
        "Dapper",
        "Microsoft.EntityFrameworkCore",
        "Microsoft.Data.SqlClient",
        "System.Data.SqlClient",
    ];

    [Fact]
    public void CoreAssemblyMustRemainPersistenceAgnostic()
    {
        Assembly coreAssembly = typeof(Cpf).Assembly;

        string[] violations = coreAssembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty)
            .Where(IsPersistenceReference)
            .ToArray();

        Assert.Empty(violations);
    }

    private static bool IsPersistenceReference(string assemblyName)
    {
        return PersistenceAssemblyPrefixes.Any(
            prefix => assemblyName.StartsWith(prefix, StringComparison.Ordinal));
    }
}
