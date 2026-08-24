using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Converters;

public sealed class ChavePixValueConverterTests
{
    [Fact]
    public void ConverterExposesAsciiMappingHints()
    {
        ChavePixValueConverter converter = new();
        ConverterMappingHints mappingHints = Assert.IsType<ConverterMappingHints>(converter.MappingHints);

        Assert.Equal(77, mappingHints.Size);
        Assert.False(mappingHints.IsUnicode ?? true);
    }

    [Fact]
    public void ConverterUsesCanonicalPersistenceRepresentationToResolveAmbiguousCpf()
    {
        Cpf ambiguousCpf = Cpf.Parse("11900000083", CultureInfo.InvariantCulture);
        Assert.True(MobilePhone.IsValid(ambiguousCpf.Value));

        ChavePix original = ChavePix.From(ambiguousCpf);
        ChavePixValueConverter converter = new();

        string providerValue = Assert.IsType<string>(converter.ConvertToProvider(original));
        ChavePix rehydrated = Assert.IsType<ChavePix>(converter.ConvertFromProvider(providerValue));

        Assert.Equal("11900000083", providerValue);
        Assert.Equal(original, rehydrated);
        Assert.Equal(TipoChavePix.Cpf, rehydrated.Tipo);
    }

    [Fact]
    public void ConverterRoundTripsEveryCanonicalPixKeyKind()
    {
        ChavePixValueConverter converter = new();
        ChavePixCase[] cases =
        [
            new(
                ChavePix.From(Cpf.Parse("11900000083", CultureInfo.InvariantCulture)),
                "11900000083",
                TipoChavePix.Cpf),
            new(
                ChavePix.From(Cnpj.Parse("00000000E08G12", CultureInfo.InvariantCulture)),
                "00000000E08G12",
                TipoChavePix.Cnpj),
            new(
                ChavePix.From(MobilePhone.Parse("(11) 98765-4321", CultureInfo.InvariantCulture)),
                "+5511987654321",
                TipoChavePix.Celular),
            new(
                ChavePix.From(Email.Parse("User@Example.COM", CultureInfo.InvariantCulture)),
                "user@example.com",
                TipoChavePix.Email),
            new(
                ChavePix.FromChaveAleatoria("550e8400-e29b-41d4-a716-446655440000"),
                "550e8400-e29b-41d4-a716-446655440000",
                TipoChavePix.Aleatoria),
        ];

        foreach (ChavePixCase testCase in cases)
        {
            string providerValue = Assert.IsType<string>(converter.ConvertToProvider(testCase.ModelValue));
            ChavePix rehydrated = Assert.IsType<ChavePix>(converter.ConvertFromProvider(providerValue));

            Assert.Equal(testCase.ProviderValue, providerValue);
            Assert.Equal(testCase.ModelValue, rehydrated);
            Assert.Equal(testCase.Tipo, rehydrated.Tipo);
        }
    }

    [Fact]
    public void ConverterRejectsUnsupportedPersistedCanonicalShape()
    {
        ChavePixValueConverter converter = new();

        Assert.Throws<FormatException>(() => converter.ConvertFromProvider("unsupported"));
    }

    private sealed record ChavePixCase(
        ChavePix ModelValue,
        string ProviderValue,
        TipoChavePix Tipo);
}
