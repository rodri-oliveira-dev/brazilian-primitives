using System.Globalization;
using Brazilian.Primitives;
using Xunit;

namespace Brazilian.Primitives.Tests;

public sealed class CodigoCompeTests
{
    [Theory]
    [InlineData("001")]
    [InlineData("033")]
    [InlineData("237")]
    public void ParseAcceptsThreeAsciiDigitsExceptSentinels(string value)
    {
        CodigoCompe codigo = CodigoCompe.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal(value, codigo.Value);
        Assert.Equal(value, codigo.ToString());
    }

    [Fact]
    public void ParsePreservesLeadingZeros()
    {
        CodigoCompe codigo = CodigoCompe.Parse("001", CultureInfo.InvariantCulture);

        Assert.Equal("001", codigo.Value);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("01")]
    [InlineData("0001")]
    [InlineData("99A")]
    [InlineData("999")]
    [InlineData("001-")]
    [InlineData(" 001")]
    [InlineData("001 ")]
    [InlineData("COMPE 001")]
    [InlineData("００１")]
    [InlineData("12345678")]
    public void TryParseReturnsFalseForInvalidInput(string? value)
    {
        bool parsed = CodigoCompe.TryParse(value, out CodigoCompe result);

        Assert.False(parsed);
        Assert.Equal(default, result);
    }

    [Fact]
    public void StructureDoesNotMeanAssignedCode()
    {
        CodigoCompe codigo = CodigoCompe.Parse("000", CultureInfo.InvariantCulture);

        Assert.Equal("000", codigo.Value);
    }

    [Theory]
    [InlineData("")]
    [InlineData("01")]
    [InlineData("999")]
    [InlineData("99A")]
    public void ParseThrowsFormatExceptionForInvalidInput(string value)
    {
        Assert.Throws<FormatException>(() => CodigoCompe.Parse(value, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ParseThrowsFormatExceptionForNullInput()
    {
        Assert.Throws<FormatException>(() => CodigoCompe.Parse(null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void EqualityUsesCanonicalValue()
    {
        CodigoCompe first = CodigoCompe.Parse("001", CultureInfo.InvariantCulture);
        CodigoCompe second = CodigoCompe.Parse("001", CultureInfo.InvariantCulture);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void ParseAndTryParseSupportSpanContracts()
    {
        ReadOnlySpan<char> value = "001".AsSpan();

        CodigoCompe parsed = CodigoCompe.Parse(value, CultureInfo.InvariantCulture);
        bool success = CodigoCompe.TryParse(value, CultureInfo.InvariantCulture, out CodigoCompe tryParsed);

        Assert.True(success);
        Assert.Equal("001", parsed.Value);
        Assert.Equal(parsed, tryParsed);
    }

    [Fact]
    public void DefaultCodigoCompeValueThrows()
    {
        CodigoCompe codigo = default;

        Assert.Throws<InvalidOperationException>(() => codigo.Value);
    }
}
