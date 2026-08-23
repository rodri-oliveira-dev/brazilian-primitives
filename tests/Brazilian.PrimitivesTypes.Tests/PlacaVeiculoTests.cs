using System.Globalization;
using Brazilian.PrimitivesTypes;
using Xunit;

namespace Brazilian.PrimitivesTypes.Tests;

public sealed class PlacaVeiculoTests
{
    [Theory]
    [InlineData("ABC1234")]
    [InlineData("ABC-1234")]
    [InlineData("abc1234")]
    public void ParseAcceptsPreviousNationalPattern(string value)
    {
        PlacaVeiculo placa = PlacaVeiculo.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal("ABC1234", placa.Value);
        Assert.Equal("ABC-1234", placa.Formatted);
        Assert.Equal(PadraoPlacaVeiculo.NacionalAnterior, placa.Padrao);
        Assert.Equal("ABC1234", placa.ToString());
        Assert.Equal("ABC1234", placa.ToString("G", formatProvider: null));
        Assert.Equal("ABC-1234", placa.ToString("F", formatProvider: null));
    }

    [Theory]
    [InlineData("ABC1D23")]
    [InlineData("abc1d23")]
    [InlineData("ABC1J23")]
    public void ParseAcceptsMercosurPattern(string value)
    {
        PlacaVeiculo placa = PlacaVeiculo.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal(value.Replace("-", string.Empty, StringComparison.Ordinal).ToUpperInvariant(), placa.Value);
        Assert.Equal(placa.Value, placa.Formatted);
        Assert.Equal(PadraoPlacaVeiculo.Mercosul, placa.Padrao);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("AB12345")]
    [InlineData("ABC12D3")]
    [InlineData("ABC-1D23")]
    [InlineData("AB-C1234")]
    [InlineData("ABC 1234")]
    [InlineData("ABC12345")]
    [InlineData("ABC123")]
    [InlineData("ÁBC1234")]
    [InlineData("ＡＢＣ１２３４")]
    [InlineData("placa ABC1234")]
    public void TryParseReturnsFalseForInvalidInput(string? value)
    {
        bool parsed = PlacaVeiculo.TryParse(value, out PlacaVeiculo result);

        Assert.False(parsed);
        Assert.Equal(default, result);
    }

    [Fact]
    public void EqualityUsesCanonicalValue()
    {
        PlacaVeiculo raw = PlacaVeiculo.Parse("ABC1234", CultureInfo.InvariantCulture);
        PlacaVeiculo formatted = PlacaVeiculo.Parse("abc-1234", CultureInfo.InvariantCulture);

        Assert.Equal(raw, formatted);
        Assert.Equal(raw.GetHashCode(), formatted.GetHashCode());
    }

    [Fact]
    public void ConverterParaPadraoMercosulUsesOfficialDigitLetterTable()
    {
        string[] expected = ["ABC1A34", "ABC1B34", "ABC1C34", "ABC1D34", "ABC1E34", "ABC1F34", "ABC1G34", "ABC1H34", "ABC1I34", "ABC1J34"];

        for (int digit = 0; digit <= 9; digit++)
        {
            PlacaVeiculo converted = PlacaVeiculo.Parse($"ABC1{digit}34", CultureInfo.InvariantCulture).ConverterParaPadraoMercosul();

            Assert.Equal(expected[digit], converted.Value);
            Assert.Equal(PadraoPlacaVeiculo.Mercosul, converted.Padrao);
        }
    }

    [Fact]
    public void ConverterParaPadraoMercosulThrowsForMercosurPlate()
    {
        PlacaVeiculo placa = PlacaVeiculo.Parse("ABC1D23", CultureInfo.InvariantCulture);

        Assert.Throws<InvalidOperationException>(() => placa.ConverterParaPadraoMercosul());
    }

    [Fact]
    public void ParseDoesNotInferVisualCategoryOrVehicleKind()
    {
        PlacaVeiculo placa = PlacaVeiculo.Parse("ABC1D23", CultureInfo.InvariantCulture);

        Assert.Equal(PadraoPlacaVeiculo.Mercosul, placa.Padrao);
        Assert.DoesNotContain("Categoria", nameof(PlacaVeiculo.GetType), StringComparison.Ordinal);
    }

    [Fact]
    public void ParseAndTryParseSupportSpanContracts()
    {
        ReadOnlySpan<char> value = "abc-1234".AsSpan();

        PlacaVeiculo parsed = PlacaVeiculo.Parse(value, CultureInfo.InvariantCulture);
        bool success = PlacaVeiculo.TryParse(value, CultureInfo.InvariantCulture, out PlacaVeiculo tryParsed);

        Assert.True(success);
        Assert.Equal("ABC1234", parsed.Value);
        Assert.Equal(parsed, tryParsed);
    }

    [Fact]
    public void ToStringThrowsFormatExceptionForUnsupportedFormat()
    {
        PlacaVeiculo placa = PlacaVeiculo.Parse("ABC1234", CultureInfo.InvariantCulture);

        Assert.Throws<FormatException>(() => placa.ToString("X", formatProvider: null));
    }

    [Fact]
    public void DefaultPlacaVeiculoValueThrows()
    {
        PlacaVeiculo placa = default;

        Assert.Throws<InvalidOperationException>(() => placa.Value);
    }
}
