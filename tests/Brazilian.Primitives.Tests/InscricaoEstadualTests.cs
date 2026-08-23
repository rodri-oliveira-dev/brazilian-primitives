using Brazilian.Primitives;
using Xunit;

namespace Brazilian.Primitives.Tests;

public sealed class InscricaoEstadualTests
{
    public static TheoryData<BrazilianState, string> ValidCanonicalValues => new()
    {
        { BrazilianState.Acre, "0100482300112" },
        { BrazilianState.Alagoas, "240000048" },
        { BrazilianState.Amapa, "030123459" },
        { BrazilianState.Amazonas, "041234567" },
        { BrazilianState.Bahia, "12345678" },
        { BrazilianState.Ceara, "060000015" },
        { BrazilianState.DistritoFederal, "0730000100109" },
        { BrazilianState.EspiritoSanto, "082345678" },
        { BrazilianState.Goias, "109876547" },
        { BrazilianState.Maranhao, "120000385" },
        { BrazilianState.MatoGrosso, "00130000019" },
        { BrazilianState.MatoGrossoDoSul, "280000383" },
        { BrazilianState.MinasGerais, "0623079040081" },
        { BrazilianState.Para, "150000006" },
        { BrazilianState.Paraiba, "160000017" },
        { BrazilianState.Parana, "1234567850" },
        { BrazilianState.Pernambuco, "032141840" },
        { BrazilianState.Piaui, "190000014" },
        { BrazilianState.RioDeJaneiro, "12345678" },
        { BrazilianState.RioGrandeDoNorte, "200000040" },
        { BrazilianState.RioGrandeDoSul, "2243658792" },
        { BrazilianState.Rondonia, "00000000625213" },
        { BrazilianState.Roraima, "240066281" },
        { BrazilianState.SantaCatarina, "251040852" },
        { BrazilianState.SaoPaulo, "110042490114" },
        { BrazilianState.Sergipe, "270000018" },
        { BrazilianState.Tocantins, "29010227836" },
    };

    [Theory]
    [MemberData(nameof(ValidCanonicalValues))]
    public void ParseAcceptsDocumentedCanonicalLengthForEveryState(BrazilianState state, string value)
    {
        InscricaoEstadual inscricao = InscricaoEstadual.Parse(value, state);

        Assert.Equal(value, inscricao.Value);
        Assert.Equal(value, inscricao.Formatted);
        Assert.Equal(state, inscricao.State);
        Assert.Equal(value, inscricao.ToString());
    }

    [Theory]
    [InlineData(BrazilianState.Bahia, "123456789")]
    [InlineData(BrazilianState.Pernambuco, "03214184000123")]
    [InlineData(BrazilianState.RioGrandeDoNorte, "2004004010")]
    public void ParseAcceptsStatesWithMultipleDocumentedLengths(BrazilianState state, string value)
    {
        Assert.True(InscricaoEstadual.IsValid(value, state));
    }

    [Theory]
    [MemberData(nameof(ValidCanonicalValues))]
    public void TryParseRejectsLengthAboveStateStrategy(BrazilianState state, string value)
    {
        bool parsed = InscricaoEstadual.TryParse(value + "00", state, out InscricaoEstadual result);

        Assert.False(parsed);
        Assert.Equal(default, result);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("ISENTO")]
    [InlineData("isento")]
    [InlineData("110.042.490.114")]
    [InlineData("11004249011A")]
    [InlineData(" 110042490114")]
    [InlineData("110042490114 ")]
    [InlineData("IE 110042490114")]
    [InlineData("１１００４２４９０１１４")]
    public void TryParseReturnsFalseForInvalidInput(string? value)
    {
        bool parsed = InscricaoEstadual.TryParse(value, BrazilianState.SaoPaulo, out InscricaoEstadual result);

        Assert.False(parsed);
        Assert.Equal(default, result);
    }

    [Fact]
    public void UnknownStateIsRejected()
    {
        Assert.False(InscricaoEstadual.IsValid("123456789", BrazilianState.Unknown));
    }

    [Fact]
    public void EqualityIncludesStateContext()
    {
        InscricaoEstadual amazonas = InscricaoEstadual.Parse("123456789", BrazilianState.Amazonas);
        InscricaoEstadual ceara = InscricaoEstadual.Parse("123456789", BrazilianState.Ceara);

        Assert.NotEqual(amazonas, ceara);
        Assert.NotEqual(amazonas.GetHashCode(), ceara.GetHashCode());
    }

    [Fact]
    public void ParseThrowsFormatExceptionForInvalidInput()
    {
        Assert.Throws<FormatException>(() => InscricaoEstadual.Parse("ISENTO", BrazilianState.SaoPaulo));
    }

    [Fact]
    public void DefaultInscricaoEstadualValueThrows()
    {
        InscricaoEstadual inscricao = default;

        Assert.Throws<InvalidOperationException>(() => inscricao.Value);
    }
}
