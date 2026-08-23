using System.Globalization;
using Brazilian.PrimitivesTypes;
using Xunit;

namespace Brazilian.PrimitivesTypes.Tests;

public sealed class NitTests
{
    [Theory]
    [InlineData("12345678901")]
    [InlineData("00000000001")]
    [InlineData("11111111111")]
    public void ParseAcceptsElevenAsciiDigitsWithoutChecksumClaim(string value)
    {
        Nit nit = Nit.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal(value, nit.Value);
        Assert.Equal(value, nit.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1234567890")]
    [InlineData("123456789012")]
    [InlineData("1234567890A")]
    [InlineData("123.45678.90-1")]
    [InlineData(" 12345678901")]
    [InlineData("12345678901 ")]
    [InlineData("NIT 12345678901")]
    [InlineData("１２３４５６７８９０１")]
    public void TryParseReturnsFalseForInvalidInput(string? value)
    {
        bool parsed = Nit.TryParse(value, out Nit result);

        Assert.False(parsed);
        Assert.Equal(default, result);
    }

    [Fact]
    public void FormatOnlyValidationDoesNotRejectArbitraryCheckDigit()
    {
        Assert.True(Nit.IsValid("12345678900"));
        Assert.True(Nit.IsValid("12345678909"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("1234567890")]
    [InlineData("1234567890A")]
    public void ParseThrowsFormatExceptionForInvalidInput(string value)
    {
        Assert.Throws<FormatException>(() => Nit.Parse(value, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ParseThrowsFormatExceptionForNullInput()
    {
        Assert.Throws<FormatException>(() => Nit.Parse(null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void EqualityUsesCanonicalValue()
    {
        Nit first = Nit.Parse("00000000001", CultureInfo.InvariantCulture);
        Nit second = Nit.Parse("00000000001", CultureInfo.InvariantCulture);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void ParseAndTryParseSupportSpanContracts()
    {
        ReadOnlySpan<char> value = "12345678901".AsSpan();

        Nit parsed = Nit.Parse(value, CultureInfo.InvariantCulture);
        bool success = Nit.TryParse(value, CultureInfo.InvariantCulture, out Nit tryParsed);

        Assert.True(success);
        Assert.Equal("12345678901", parsed.Value);
        Assert.Equal(parsed, tryParsed);
    }

    [Fact]
    public void DefaultNitValueThrows()
    {
        Nit nit = default;

        Assert.Throws<InvalidOperationException>(() => nit.Value);
    }
}
