using System.Globalization;
using Brazilian.PrimitivesTypes;
using Xunit;

namespace Brazilian.PrimitivesTypes.Tests;

public sealed class CnsTests
{
    [Theory]
    [InlineData("123456789010000")]
    [InlineData("234567890120004")]
    [InlineData("200000000000003")]
    [InlineData("700000000000005")]
    [InlineData("800000000000001")]
    [InlineData("900000000000008")]
    [InlineData("898001160001008")]
    public void IsValidReturnsTrueForKnownValidCnsValues(string value)
    {
        Assert.True(Cns.IsValid(value));
    }

    [Theory]
    [InlineData("123456789010001")]
    [InlineData("234567890120005")]
    [InlineData("700000000000004")]
    [InlineData("800000000000002")]
    [InlineData("900000000000009")]
    public void IsValidReturnsFalseForInvalidCheckDigits(string value)
    {
        Assert.False(Cns.IsValid(value));
    }

    [Fact]
    public void ParseReturnsCanonicalFifteenDigits()
    {
        Cns cns = Cns.Parse("123456789010000", CultureInfo.InvariantCulture);

        Assert.Equal("123456789010000", cns.Value);
        Assert.Equal("123456789010000", cns.ToString());
    }

    [Theory]
    [InlineData("133456789010000")]
    [InlineData("124456789010000")]
    [InlineData("123556789010000")]
    [InlineData("710000000000005")]
    [InlineData("800001000000001")]
    [InlineData("900000000100008")]
    public void IsValidReturnsFalseWhenBasePositionIsMutated(string value)
    {
        Assert.False(Cns.IsValid(value));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("12345678901000")]
    [InlineData("1234567890100000")]
    [InlineData("12345678901000A")]
    [InlineData("123 456 789 010 000")]
    [InlineData(" 123456789010000")]
    [InlineData("123456789010000 ")]
    [InlineData("CNS 123456789010000")]
    [InlineData("３４５６７８９０１００００")]
    [InlineData("000000000000000")]
    [InlineData("300000000000000")]
    public void TryParseReturnsFalseForInvalidInput(string? value)
    {
        bool parsed = Cns.TryParse(value, out Cns result);

        Assert.False(parsed);
        Assert.Equal(default, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("12345678901000")]
    [InlineData("123456789010001")]
    public void ParseThrowsFormatExceptionForInvalidInput(string value)
    {
        Assert.Throws<FormatException>(() => Cns.Parse(value, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ParseThrowsFormatExceptionForNullInput()
    {
        Assert.Throws<FormatException>(() => Cns.Parse(null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void EqualityUsesCanonicalValue()
    {
        Cns first = Cns.Parse("123456789010000", CultureInfo.InvariantCulture);
        Cns second = Cns.Parse("123456789010000", CultureInfo.InvariantCulture);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void ParseAndTryParseSupportSpanContracts()
    {
        ReadOnlySpan<char> value = "700000000000005".AsSpan();

        Cns parsed = Cns.Parse(value, CultureInfo.InvariantCulture);
        bool success = Cns.TryParse(value, CultureInfo.InvariantCulture, out Cns tryParsed);

        Assert.True(success);
        Assert.Equal("700000000000005", parsed.Value);
        Assert.Equal(parsed, tryParsed);
    }

    [Fact]
    public void DefaultCnsValueThrows()
    {
        Cns cns = default;

        Assert.Throws<InvalidOperationException>(() => cns.Value);
    }
}
