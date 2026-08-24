using Brazilian.PrimitivesTypes;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Converters;

public sealed class BrazilianStateCodeValueConverterTests
{
    public static TheoryData<BrazilianState, string> StateCodes => new()
    {
        { BrazilianState.Acre, "AC" },
        { BrazilianState.Alagoas, "AL" },
        { BrazilianState.Amapa, "AP" },
        { BrazilianState.Amazonas, "AM" },
        { BrazilianState.Bahia, "BA" },
        { BrazilianState.Ceara, "CE" },
        { BrazilianState.DistritoFederal, "DF" },
        { BrazilianState.EspiritoSanto, "ES" },
        { BrazilianState.Goias, "GO" },
        { BrazilianState.Maranhao, "MA" },
        { BrazilianState.MatoGrosso, "MT" },
        { BrazilianState.MatoGrossoDoSul, "MS" },
        { BrazilianState.MinasGerais, "MG" },
        { BrazilianState.Para, "PA" },
        { BrazilianState.Paraiba, "PB" },
        { BrazilianState.Parana, "PR" },
        { BrazilianState.Pernambuco, "PE" },
        { BrazilianState.Piaui, "PI" },
        { BrazilianState.RioDeJaneiro, "RJ" },
        { BrazilianState.RioGrandeDoNorte, "RN" },
        { BrazilianState.RioGrandeDoSul, "RS" },
        { BrazilianState.Rondonia, "RO" },
        { BrazilianState.Roraima, "RR" },
        { BrazilianState.SantaCatarina, "SC" },
        { BrazilianState.SaoPaulo, "SP" },
        { BrazilianState.Sergipe, "SE" },
        { BrazilianState.Tocantins, "TO" },
    };

    [Theory]
    [MemberData(nameof(StateCodes))]
    public void ConverterRoundTripsEveryStableUfCode(BrazilianState state, string code)
    {
        BrazilianStateCodeValueConverter converter = new();

        Assert.Equal(code, converter.ConvertToProvider(state));
        Assert.Equal(state, converter.ConvertFromProvider(code));
    }

    [Fact]
    public void ConverterRejectsUnknownStateAndInvalidCode()
    {
        BrazilianStateCodeValueConverter converter = new();

        Assert.Throws<InvalidOperationException>(() => converter.ConvertToProvider(BrazilianState.Unknown));
        Assert.Throws<FormatException>(() => converter.ConvertFromProvider("XX"));
    }
}
