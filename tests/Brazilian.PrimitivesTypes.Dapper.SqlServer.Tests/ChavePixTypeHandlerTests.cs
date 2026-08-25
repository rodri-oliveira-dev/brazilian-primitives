using System.Data;
using System.Globalization;
using Brazilian.PrimitivesTypes;
using Brazilian.PrimitivesTypes.Dapper.SqlServer.Tests.Infrastructure;
using Xunit;

namespace Brazilian.PrimitivesTypes.Dapper.SqlServer.Tests;

public sealed class ChavePixTypeHandlerTests
{
    [Fact]
    public void HandlerRoundTripsEveryCanonicalPixKeyKind()
    {
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

        BrazilianPrimitiveTypeHandler<ChavePix> handler = BrazilianPrimitiveSqlServerMappings.ChavePixMapping.Handler;

        foreach (ChavePixCase testCase in cases)
        {
            TestDbParameter parameter = new();

            handler.SetValue(parameter, testCase.ModelValue);
            ChavePix rehydrated = handler.Parse(testCase.ProviderValue);

            Assert.Equal(DbType.AnsiString, parameter.DbType);
            Assert.Equal(77, parameter.Size);
            Assert.Equal(testCase.ProviderValue, parameter.Value);
            Assert.Equal(testCase.ModelValue, rehydrated);
            Assert.Equal(testCase.Tipo, rehydrated.Tipo);
        }
    }

    [Fact]
    public void HandlerPreservesAmbiguousCpfSemanticType()
    {
        Cpf ambiguousCpf = Cpf.Parse("11900000083", CultureInfo.InvariantCulture);
        Assert.True(MobilePhone.IsValid(ambiguousCpf.Value));

        ChavePix original = ChavePix.From(ambiguousCpf);
        ChavePix rehydrated = BrazilianPrimitiveSqlServerMappings.ChavePixMapping.Handler.Parse(original.Value);

        Assert.Equal(original, rehydrated);
        Assert.Equal(TipoChavePix.Cpf, rehydrated.Tipo);
    }

    [Fact]
    public void HandlerRejectsUnsupportedPersistedCanonicalShape()
    {
        Assert.Throws<FormatException>(
            () => BrazilianPrimitiveSqlServerMappings.ChavePixMapping.Handler.Parse("unsupported"));
    }

    private sealed record ChavePixCase(
        ChavePix ModelValue,
        string ProviderValue,
        TipoChavePix Tipo);
}
