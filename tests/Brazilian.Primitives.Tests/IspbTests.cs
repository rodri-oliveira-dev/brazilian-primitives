using System.Globalization;
using Brazilian.Primitives;
using Xunit;

namespace Brazilian.Primitives.Tests;

public sealed class IspbTests
{
    [Theory]
    [InlineData("12345678")]
    [InlineData("00000001")]
    [InlineData("00000000")]
    public void ParseAcceptsEightAsciiDigits(string value)
    {
        Ispb ispb = Ispb.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal(value, ispb.Value);
        Assert.Equal(value, ispb.ToString());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1234567")]
    [InlineData("123456789")]
    [InlineData("1234567A")]
    [InlineData("1234-5678")]
    [InlineData(" 12345678")]
    [InlineData("12345678 ")]
    [InlineData("ISPB 12345678")]
    [InlineData("１２３４５６７８")]
    public void TryParseReturnsFalseForInvalidInput(string? value)
    {
        bool parsed = Ispb.TryParse(value, out Ispb result);

        Assert.False(parsed);
        Assert.Equal(default, result);
    }

    [Fact]
    public void StructureDoesNotMeanAssignedParticipant()
    {
        Ispb ispb = Ispb.Parse("00000000", CultureInfo.InvariantCulture);

        Assert.Equal("00000000", ispb.Value);
    }

    [Fact]
    public void CnpjBaseIsNotAcceptedWhenItIsNotEightDigits()
    {
        Assert.False(Ispb.IsValid("00000000E08G12"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("1234567")]
    [InlineData("1234567A")]
    public void ParseThrowsFormatExceptionForInvalidInput(string value)
    {
        Assert.Throws<FormatException>(() => Ispb.Parse(value, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ParseThrowsFormatExceptionForNullInput()
    {
        Assert.Throws<FormatException>(() => Ispb.Parse(null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void EqualityUsesCanonicalValue()
    {
        Ispb first = Ispb.Parse("00000001", CultureInfo.InvariantCulture);
        Ispb second = Ispb.Parse("00000001", CultureInfo.InvariantCulture);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void ParseAndTryParseSupportSpanContracts()
    {
        ReadOnlySpan<char> value = "12345678".AsSpan();

        Ispb parsed = Ispb.Parse(value, CultureInfo.InvariantCulture);
        bool success = Ispb.TryParse(value, CultureInfo.InvariantCulture, out Ispb tryParsed);

        Assert.True(success);
        Assert.Equal("12345678", parsed.Value);
        Assert.Equal(parsed, tryParsed);
    }

    [Fact]
    public void DefaultIspbValueThrows()
    {
        Ispb ispb = default;

        Assert.Throws<InvalidOperationException>(() => ispb.Value);
    }
}
