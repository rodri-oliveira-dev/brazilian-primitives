using Brazilian.PrimitivesTypes;
using Xunit;

namespace Brazilian.PrimitivesTypes.Tests;

public sealed class RgStateAwareTests
{
    [Theory]
    [InlineData(BrazilianState.Acre, "123456")]
    [InlineData(BrazilianState.Alagoas, "1234567")]
    [InlineData(BrazilianState.Amapa, "123456789")]
    [InlineData(BrazilianState.Amazonas, "123456789")]
    [InlineData(BrazilianState.Bahia, "1234567890")]
    [InlineData(BrazilianState.Ceara, "123456789")]
    [InlineData(BrazilianState.DistritoFederal, "1234567")]
    [InlineData(BrazilianState.EspiritoSanto, "123456789")]
    [InlineData(BrazilianState.Goias, "123456789")]
    [InlineData(BrazilianState.Maranhao, "123456789")]
    [InlineData(BrazilianState.MatoGrosso, "123456789")]
    [InlineData(BrazilianState.MatoGrossoDoSul, "123456789")]
    [InlineData(BrazilianState.MinasGerais, "12345678")]
    [InlineData(BrazilianState.Para, "123456789")]
    [InlineData(BrazilianState.Paraiba, "123456789")]
    [InlineData(BrazilianState.Parana, "12345678")]
    [InlineData(BrazilianState.Pernambuco, "123456789")]
    [InlineData(BrazilianState.Piaui, "123456789")]
    [InlineData(BrazilianState.RioDeJaneiro, "123456789")]
    [InlineData(BrazilianState.RioGrandeDoNorte, "123456789")]
    [InlineData(BrazilianState.RioGrandeDoSul, "1234567890")]
    [InlineData(BrazilianState.Rondonia, "123456789")]
    [InlineData(BrazilianState.Roraima, "123456789")]
    [InlineData(BrazilianState.SantaCatarina, "123456789")]
    [InlineData(BrazilianState.SaoPaulo, "120300011")]
    [InlineData(BrazilianState.Sergipe, "123456789")]
    [InlineData(BrazilianState.Tocantins, "123456789")]
    public void IsValidReturnsTrueForSupportedStateVectors(BrazilianState state, string value)
    {
        Assert.True(Rg.IsValid(value, state));
    }

    [Theory]
    [InlineData(BrazilianState.Acre)]
    [InlineData(BrazilianState.Alagoas)]
    [InlineData(BrazilianState.Amapa)]
    [InlineData(BrazilianState.Amazonas)]
    [InlineData(BrazilianState.Bahia)]
    [InlineData(BrazilianState.Ceara)]
    [InlineData(BrazilianState.DistritoFederal)]
    [InlineData(BrazilianState.EspiritoSanto)]
    [InlineData(BrazilianState.Goias)]
    [InlineData(BrazilianState.Maranhao)]
    [InlineData(BrazilianState.MatoGrosso)]
    [InlineData(BrazilianState.MatoGrossoDoSul)]
    [InlineData(BrazilianState.MinasGerais)]
    [InlineData(BrazilianState.Para)]
    [InlineData(BrazilianState.Paraiba)]
    [InlineData(BrazilianState.Parana)]
    [InlineData(BrazilianState.Pernambuco)]
    [InlineData(BrazilianState.Piaui)]
    [InlineData(BrazilianState.RioDeJaneiro)]
    [InlineData(BrazilianState.RioGrandeDoNorte)]
    [InlineData(BrazilianState.RioGrandeDoSul)]
    [InlineData(BrazilianState.Rondonia)]
    [InlineData(BrazilianState.Roraima)]
    [InlineData(BrazilianState.SantaCatarina)]
    [InlineData(BrazilianState.SaoPaulo)]
    [InlineData(BrazilianState.Sergipe)]
    [InlineData(BrazilianState.Tocantins)]
    public void IsValidReturnsFalseForWrongLengthInEveryState(BrazilianState state)
    {
        Assert.False(Rg.IsValid("1", state));
    }

    [Theory]
    [InlineData("120300011", "120300011", "12.030.001-1")]
    [InlineData("12.030.001-1", "120300011", "12.030.001-1")]
    [InlineData("00000005X", "00000005X", "00.000.005-X")]
    [InlineData("00.000.005-X", "00000005X", "00.000.005-X")]
    [InlineData("00.000.005-x", "00000005X", "00.000.005-X")]
    public void SaoPauloNormalizesAndValidatesDocumentedCheckDigit(string input, string expectedValue, string expectedFormatted)
    {
        Rg rg = Rg.Parse(input, BrazilianState.SaoPaulo);

        Assert.Equal(expectedValue, rg.Value);
        Assert.Equal(expectedFormatted, rg.Formatted);
        Assert.Equal(BrazilianState.SaoPaulo, rg.State);
        Assert.Equal(expectedValue, rg.ToString());
    }

    [Theory]
    [InlineData("120300012")]
    [InlineData("12.030.001-2")]
    [InlineData("000000050")]
    [InlineData("00.000.005-0")]
    public void SaoPauloRejectsInvalidCheckDigits(string value)
    {
        Assert.False(Rg.IsValid(value, BrazilianState.SaoPaulo));
    }

    [Theory]
    [InlineData("12-030-001.1")]
    [InlineData("12.030.001 1")]
    [InlineData(" 12.030.001-1")]
    [InlineData("12.030.001-1 ")]
    [InlineData("12.030.00A-1")]
    [InlineData("１２.０３０.００１-１")]
    public void SaoPauloRejectsNonCanonicalMasksAndCharacters(string value)
    {
        Assert.False(Rg.IsValid(value, BrazilianState.SaoPaulo));
    }

    [Fact]
    public void RioDeJaneiroSupportsNineDigitLegacyMaskWithoutClaimingChecksumValidation()
    {
        Rg raw = Rg.Parse("123456789", BrazilianState.RioDeJaneiro);
        Rg masked = Rg.Parse("12.345.678-9", BrazilianState.RioDeJaneiro);

        Assert.Equal(raw, masked);
        Assert.Equal("123456789", raw.Value);
        Assert.Equal("12.345.678-9", raw.Formatted);
    }

    [Theory]
    [InlineData("12345678")]
    [InlineData("1.234.567-8")]
    public void RioDeJaneiroRejectsIncorrectEightDigitShape(string value)
    {
        Assert.False(Rg.IsValid(value, BrazilianState.RioDeJaneiro));
    }

    [Theory]
    [InlineData("12345678")]
    [InlineData("12.345.678")]
    [InlineData("MG-12345678")]
    [InlineData("mg-12345678")]
    [InlineData("MG-12.345.678")]
    [InlineData("M-12345678")]
    [InlineData("M-12.345.678")]
    [InlineData("m-12.345.678")]
    public void MinasGeraisNormalizesKnownLegacyRepresentations(string value)
    {
        Rg rg = Rg.Parse(value, BrazilianState.MinasGerais);

        Assert.Equal("12345678", rg.Value);
        Assert.Equal("MG-12.345.678", rg.Formatted);
        Assert.Equal(BrazilianState.MinasGerais, rg.State);
    }

    [Theory]
    [InlineData("1.234.567-8")]
    [InlineData("M1.234.567-8")]
    [InlineData("M12345678")]
    [InlineData("MG 12.345.678")]
    public void MinasGeraisRejectsIncorrectRioStyleAndLoosePrefixes(string value)
    {
        Assert.False(Rg.IsValid(value, BrazilianState.MinasGerais));
    }

    [Fact]
    public void MinasGeraisEqualityUsesCanonicalDigitsAcrossPrefixes()
    {
        Rg raw = Rg.Parse("12345678", BrazilianState.MinasGerais);
        Rg prefixed = Rg.Parse("MG-12.345.678", BrazilianState.MinasGerais);
        Rg historicalPrefix = Rg.Parse("M-12.345.678", BrazilianState.MinasGerais);

        Assert.Equal(raw, prefixed);
        Assert.Equal(raw, historicalPrefix);
    }

    [Fact]
    public void SantaCatarinaSupportsKnownDisplayMask()
    {
        Rg raw = Rg.Parse("123456789", BrazilianState.SantaCatarina);
        Rg masked = Rg.Parse("123.456.789", BrazilianState.SantaCatarina);

        Assert.Equal(raw, masked);
        Assert.Equal("123.456.789", raw.Formatted);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("12345678A")]
    [InlineData("123.456.789")]
    [InlineData("123 456 789")]
    public void FormatOnlyStateRejectsUnsupportedRepresentations(string? value)
    {
        bool parsed = Rg.TryParse(value, BrazilianState.Amazonas, out Rg result);

        Assert.False(parsed);
        Assert.Equal(default, result);
    }

    [Fact]
    public void UnknownAndUndefinedStatesAreRejected()
    {
        Assert.False(Rg.IsValid("123456789", BrazilianState.Unknown));
        Assert.False(Rg.IsValid("123456789", (BrazilianState)999));
        Assert.Throws<FormatException>(() => Rg.Parse("123456789", BrazilianState.Unknown));
    }

    [Fact]
    public void EqualityIncludesIssuingState()
    {
        Rg amazonas = Rg.Parse("123456789", BrazilianState.Amazonas);
        Rg amapa = Rg.Parse("123456789", BrazilianState.Amapa);

        Assert.NotEqual(amazonas, amapa);
        Assert.Equal(amazonas.Value, amapa.Value);
    }

    [Fact]
    public void EqualityUsesNormalizedRepresentationWithinTheSameState()
    {
        Rg raw = Rg.Parse("120300011", BrazilianState.SaoPaulo);
        Rg masked = Rg.Parse("12.030.001-1", BrazilianState.SaoPaulo);

        Assert.Equal(raw, masked);
        Assert.Equal(raw.GetHashCode(), masked.GetHashCode());
    }

    [Fact]
    public void ParseThrowsFormatExceptionForInvalidValue()
    {
        Assert.Throws<FormatException>(() => Rg.Parse("not-an-rg", BrazilianState.Parana));
    }
}
