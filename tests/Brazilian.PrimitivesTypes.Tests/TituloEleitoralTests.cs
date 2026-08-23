using System.Globalization;
using Brazilian.PrimitivesTypes;
using Xunit;

namespace Brazilian.PrimitivesTypes.Tests;

public sealed class TituloEleitoralTests
{
    public static TheoryData<string, BrazilianState> NationalOrigins => new()
    {
        { "000123450159", BrazilianState.SaoPaulo },
        { "000123450256", BrazilianState.MinasGerais },
        { "000123450353", BrazilianState.RioDeJaneiro },
        { "000123450450", BrazilianState.RioGrandeDoSul },
        { "000123450558", BrazilianState.Bahia },
        { "000123450655", BrazilianState.Parana },
        { "000123450752", BrazilianState.Ceara },
        { "000123450850", BrazilianState.Pernambuco },
        { "000123450957", BrazilianState.SantaCatarina },
        { "000123451058", BrazilianState.Goias },
        { "000123451155", BrazilianState.Maranhao },
        { "000123451252", BrazilianState.Paraiba },
        { "000123451350", BrazilianState.Para },
        { "000123451457", BrazilianState.EspiritoSanto },
        { "000123451554", BrazilianState.Piaui },
        { "000123451651", BrazilianState.RioGrandeDoNorte },
        { "000123451759", BrazilianState.Alagoas },
        { "000123451856", BrazilianState.MatoGrosso },
        { "000123451953", BrazilianState.MatoGrossoDoSul },
        { "000123452054", BrazilianState.DistritoFederal },
        { "000123452151", BrazilianState.Sergipe },
        { "000123452259", BrazilianState.Amazonas },
        { "000123452356", BrazilianState.Rondonia },
        { "000123452453", BrazilianState.Acre },
        { "000123452550", BrazilianState.Amapa },
        { "000123452658", BrazilianState.Roraima },
        { "000123452755", BrazilianState.Tocantins },
    };

    [Theory]
    [MemberData(nameof(NationalOrigins))]
    public void ParseAcceptsNationalOriginsAndMapsToBrazilianState(string value, BrazilianState expectedState)
    {
        TituloEleitoral titulo = TituloEleitoral.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal(value, titulo.Value);
        Assert.Equal("00012345", titulo.NumeroSequencial);
        Assert.False(titulo.IsExterior);
        Assert.True(titulo.TryGetState(out BrazilianState state));
        Assert.Equal(expectedState, state);
        Assert.Equal(value, titulo.ToString());
    }

    [Fact]
    public void ParseAcceptsExteriorOriginWithoutBrazilianState()
    {
        TituloEleitoral titulo = TituloEleitoral.Parse("000123452852", CultureInfo.InvariantCulture);

        Assert.Equal("28", titulo.CodigoOrigem);
        Assert.True(titulo.IsExterior);
        Assert.False(titulo.TryGetState(out BrazilianState state));
        Assert.Equal(BrazilianState.Unknown, state);
    }

    [Theory]
    [InlineData("000000060116", BrazilianState.SaoPaulo)]
    [InlineData("000000060213", BrazilianState.MinasGerais)]
    public void ParseAppliesSaoPauloAndMinasGeraisFirstCheckDigitException(string value, BrazilianState expectedState)
    {
        TituloEleitoral titulo = TituloEleitoral.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal(value, titulo.Value);
        Assert.True(titulo.TryGetState(out BrazilianState state));
        Assert.Equal(expectedState, state);
    }

    [Fact]
    public void ParseKeepsDefaultRemainderTenMappingForOtherOrigins()
    {
        TituloEleitoral titulo = TituloEleitoral.Parse("000000060302", CultureInfo.InvariantCulture);

        Assert.Equal("03", titulo.CodigoOrigem);
        Assert.True(titulo.TryGetState(out BrazilianState state));
        Assert.Equal(BrazilianState.RioDeJaneiro, state);
    }

    [Theory]
    [InlineData("000123450059")]
    [InlineData("000123452952")]
    [InlineData("000123450150")]
    [InlineData("000123450158")]
    [InlineData("000123550159")]
    public void IsValidReturnsFalseForInvalidOriginOrCheckDigits(string value)
    {
        Assert.False(TituloEleitoral.IsValid(value));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("00123450159")]
    [InlineData("0001234501590")]
    [InlineData("00012345015A")]
    [InlineData("0001.2345.0159")]
    [InlineData(" 000123450159")]
    [InlineData("000123450159 ")]
    [InlineData("titulo 000123450159")]
    [InlineData("０００１２３４５０１５９")]
    public void TryParseReturnsFalseForInvalidInput(string? value)
    {
        bool parsed = TituloEleitoral.TryParse(value, out TituloEleitoral result);

        Assert.False(parsed);
        Assert.Equal(default, result);
    }

    [Fact]
    public void CanonicalRepresentationKeepsTwelveDigits()
    {
        TituloEleitoral titulo = TituloEleitoral.Parse("000123450159", CultureInfo.InvariantCulture);

        Assert.Equal(12, titulo.Value.Length);
        Assert.Equal("00012345", titulo.NumeroSequencial);
        Assert.Equal("01", titulo.CodigoOrigem);
    }

    [Theory]
    [InlineData("")]
    [InlineData("00123450159")]
    [InlineData("000123450150")]
    public void ParseThrowsFormatExceptionForInvalidInput(string value)
    {
        Assert.Throws<FormatException>(() => TituloEleitoral.Parse(value, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ParseThrowsFormatExceptionForNullInput()
    {
        Assert.Throws<FormatException>(() => TituloEleitoral.Parse(null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void EqualityUsesCanonicalValue()
    {
        TituloEleitoral first = TituloEleitoral.Parse("000123450159", CultureInfo.InvariantCulture);
        TituloEleitoral second = TituloEleitoral.Parse("000123450159", CultureInfo.InvariantCulture);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void ParseAndTryParseSupportSpanContracts()
    {
        ReadOnlySpan<char> value = "000123450159".AsSpan();

        TituloEleitoral parsed = TituloEleitoral.Parse(value, CultureInfo.InvariantCulture);
        bool success = TituloEleitoral.TryParse(value, CultureInfo.InvariantCulture, out TituloEleitoral tryParsed);

        Assert.True(success);
        Assert.Equal("000123450159", parsed.Value);
        Assert.Equal(parsed, tryParsed);
    }

    [Fact]
    public void DefaultTituloEleitoralValueThrows()
    {
        TituloEleitoral titulo = default;

        Assert.Throws<InvalidOperationException>(() => titulo.Value);
    }
}
