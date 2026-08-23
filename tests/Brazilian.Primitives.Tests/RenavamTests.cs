using System.Globalization;
using Brazilian.Primitives;
using Xunit;

namespace Brazilian.Primitives.Tests;

public sealed class RenavamTests
{
    [Theory]
    [InlineData("12016112273")]
    [InlineData("00123456789")]
    [InlineData("63988496290")]
    [InlineData("00504045105")]
    [InlineData("98765432103")]
    public void IsValidReturnsTrueForKnownValidRenavams(string value)
    {
        Assert.True(Renavam.IsValid(value));
    }

    [Fact]
    public void ParsePreservesLeadingZeros()
    {
        Renavam renavam = Renavam.Parse("00123456789", CultureInfo.InvariantCulture);

        Assert.Equal("00123456789", renavam.Value);
        Assert.Equal("00123456789", renavam.ToString());
    }

    [Theory]
    [InlineData("12016112270")]
    [InlineData("00123456780")]
    [InlineData("63988496293")]
    [InlineData("00504045104")]
    public void IsValidReturnsFalseForInvalidCheckDigits(string value)
    {
        Assert.False(Renavam.IsValid(value));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("123456789")]
    [InlineData("1234567890")]
    [InlineData("123456789012")]
    [InlineData("1201611227A")]
    [InlineData("120.161.122-73")]
    [InlineData("12016112273 ")]
    [InlineData("RENAVAM 12016112273")]
    [InlineData("１２３４５６７８９０１")]
    [InlineData("00000000000")]
    public void TryParseReturnsFalseForInvalidInput(string? value)
    {
        bool parsed = Renavam.TryParse(value, out Renavam result);

        Assert.False(parsed);
        Assert.Equal(default, result);
    }

    [Fact]
    public void IsValidReturnsFalseWhenBasePositionIsMutated()
    {
        string valid = "12016112273";

        for (int index = 0; index < 10; index++)
        {
            char[] mutated = valid.ToCharArray();
            mutated[index] = mutated[index] == '9' ? '0' : (char)(mutated[index] + 1);

            Assert.False(Renavam.IsValid(new string(mutated)));
        }
    }

    [Theory]
    [InlineData("")]
    [InlineData("123456789")]
    [InlineData("12016112270")]
    public void ParseThrowsFormatExceptionForInvalidInput(string value)
    {
        Assert.Throws<FormatException>(() => Renavam.Parse(value, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ParseThrowsFormatExceptionForNullInput()
    {
        Assert.Throws<FormatException>(() => Renavam.Parse(null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void EqualityUsesCanonicalValue()
    {
        Renavam first = Renavam.Parse("00123456789", CultureInfo.InvariantCulture);
        Renavam second = Renavam.Parse("00123456789", CultureInfo.InvariantCulture);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void ParseAndTryParseSupportSpanContracts()
    {
        ReadOnlySpan<char> value = "12016112273".AsSpan();

        Renavam parsed = Renavam.Parse(value, CultureInfo.InvariantCulture);
        bool success = Renavam.TryParse(value, CultureInfo.InvariantCulture, out Renavam tryParsed);

        Assert.True(success);
        Assert.Equal("12016112273", parsed.Value);
        Assert.Equal(parsed, tryParsed);
    }

    [Fact]
    public void DefaultRenavamValueThrows()
    {
        Renavam renavam = default;

        Assert.Throws<InvalidOperationException>(() => renavam.Value);
    }
}
