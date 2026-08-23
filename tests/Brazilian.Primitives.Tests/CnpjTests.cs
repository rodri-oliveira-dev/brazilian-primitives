using System.Globalization;
using Brazilian.Primitives;
using Xunit;

namespace Brazilian.Primitives.Tests;

public sealed class CnpjTests
{
    [Theory]
    [InlineData("11222333000181")]
    [InlineData("33000167000101")]
    [InlineData("04252011000110")]
    [InlineData("27865757000102")]
    [InlineData("00000001000136")]
    [InlineData("12345678000195")]
    public void IsValidReturnsTrueForKnownValidCnpjs(string value)
    {
        Assert.True(Cnpj.IsValid(value));
    }

    [Theory]
    [InlineData("11222333000181")]
    [InlineData("11.222.333/0001-81")]
    public void ParseNormalizesSupportedRepresentations(string value)
    {
        Cnpj cnpj = Cnpj.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal("11222333000181", cnpj.Value);
        Assert.Equal("11.222.333/0001-81", cnpj.Formatted);
        Assert.Equal("11222333000181", cnpj.ToString());
        Assert.Equal("11222333000181", cnpj.ToString("G", formatProvider: null));
        Assert.Equal("11.222.333/0001-81", cnpj.ToString("F", formatProvider: null));
    }

    [Theory]
    [InlineData("00000000000000")]
    [InlineData("11111111111111")]
    [InlineData("22222222222222")]
    [InlineData("33333333333333")]
    [InlineData("44444444444444")]
    [InlineData("55555555555555")]
    [InlineData("66666666666666")]
    [InlineData("77777777777777")]
    [InlineData("88888888888888")]
    [InlineData("99999999999999")]
    public void IsValidReturnsFalseForRepeatedCharacters(string value)
    {
        Assert.False(Cnpj.IsValid(value));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1122233300018")]
    [InlineData("112223330001810")]
    [InlineData("11222333000191")]
    [InlineData("11222333000180")]
    [InlineData("11222333A00181")]
    [InlineData("11.A22.333/0001-81")]
    [InlineData("11-222-333.0001/81")]
    [InlineData("11 222 333 0001 81")]
    [InlineData("１１２２２３３３０００１８１")]
    public void TryParseReturnsFalseForInvalidInput(string? value)
    {
        bool parsed = Cnpj.TryParse(value, out Cnpj result);

        Assert.False(parsed);
        Assert.Equal(default, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1122233300018")]
    [InlineData("11222333000191")]
    [InlineData("11222333A00181")]
    public void ParseThrowsFormatExceptionForInvalidInput(string value)
    {
        Assert.Throws<FormatException>(() => Cnpj.Parse(value, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ParseThrowsFormatExceptionForNullInput()
    {
        Assert.Throws<FormatException>(() => Cnpj.Parse(null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void EqualityUsesNormalizedValue()
    {
        Cnpj unmasked = Cnpj.Parse("11222333000181", CultureInfo.InvariantCulture);
        Cnpj masked = Cnpj.Parse("11.222.333/0001-81", CultureInfo.InvariantCulture);

        Assert.Equal(unmasked, masked);
        Assert.Equal(unmasked.GetHashCode(), masked.GetHashCode());
    }

    [Fact]
    public void ParsePreservesLeadingZeros()
    {
        Cnpj cnpj = Cnpj.Parse("04.252.011/0001-10", CultureInfo.InvariantCulture);

        Assert.Equal("04252011000110", cnpj.Value);
        Assert.Equal("04.252.011/0001-10", cnpj.Formatted);
    }

    [Fact]
    public void ParseAndTryParseSupportSpanContracts()
    {
        ReadOnlySpan<char> value = "11.222.333/0001-81".AsSpan();

        Cnpj parsed = Cnpj.Parse(value, CultureInfo.InvariantCulture);
        bool success = Cnpj.TryParse(value, CultureInfo.InvariantCulture, out Cnpj tryParsed);

        Assert.True(success);
        Assert.Equal("11222333000181", parsed.Value);
        Assert.Equal(parsed, tryParsed);
    }

    [Fact]
    public void ToStringThrowsFormatExceptionForUnsupportedFormat()
    {
        Cnpj cnpj = Cnpj.Parse("11222333000181", CultureInfo.InvariantCulture);

        Assert.Throws<FormatException>(() => cnpj.ToString("X", formatProvider: null));
    }
}
