using System.Globalization;
using Brazilian.PrimitivesTypes;
using Xunit;

namespace Brazilian.PrimitivesTypes.Tests;

public sealed class PisPasepTests
{
    [Theory]
    [InlineData("12044529868")]
    [InlineData("01234567897")]
    [InlineData("12345678900")]
    [InlineData("98765432103")]
    [InlineData("00000000019")]
    [InlineData("11111111124")]
    public void IsValidReturnsTrueForKnownValidPisPasepValues(string value)
    {
        Assert.True(PisPasep.IsValid(value));
    }

    [Fact]
    public void ParsePreservesLeadingZeros()
    {
        PisPasep pisPasep = PisPasep.Parse("01234567897", CultureInfo.InvariantCulture);

        Assert.Equal("01234567897", pisPasep.Value);
        Assert.Equal("01234567897", pisPasep.ToString());
    }

    [Theory]
    [InlineData("12044529860")]
    [InlineData("01234567890")]
    [InlineData("98765432104")]
    public void IsValidReturnsFalseForInvalidCheckDigits(string value)
    {
        Assert.False(PisPasep.IsValid(value));
    }

    [Fact]
    public void IsValidReturnsFalseWhenBasePositionIsMutated()
    {
        string valid = "12044529868";

        for (int index = 0; index < 10; index++)
        {
            char[] mutated = valid.ToCharArray();
            mutated[index] = mutated[index] == '9' ? '0' : (char)(mutated[index] + 1);

            Assert.False(PisPasep.IsValid(new string(mutated)));
        }
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1234567890")]
    [InlineData("123456789012")]
    [InlineData("1204452986A")]
    [InlineData("120.44529.86-8")]
    [InlineData(" 12044529868")]
    [InlineData("12044529868 ")]
    [InlineData("PIS 12044529868")]
    [InlineData("１２３４５６７８９０１")]
    [InlineData("00000000000")]
    public void TryParseReturnsFalseForInvalidInput(string? value)
    {
        bool parsed = PisPasep.TryParse(value, out PisPasep result);

        Assert.False(parsed);
        Assert.Equal(default, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1234567890")]
    [InlineData("12044529860")]
    public void ParseThrowsFormatExceptionForInvalidInput(string value)
    {
        Assert.Throws<FormatException>(() => PisPasep.Parse(value, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ParseThrowsFormatExceptionForNullInput()
    {
        Assert.Throws<FormatException>(() => PisPasep.Parse(null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void EqualityUsesCanonicalValue()
    {
        PisPasep first = PisPasep.Parse("12044529868", CultureInfo.InvariantCulture);
        PisPasep second = PisPasep.Parse("12044529868", CultureInfo.InvariantCulture);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Fact]
    public void ParseAndTryParseSupportSpanContracts()
    {
        ReadOnlySpan<char> value = "12044529868".AsSpan();

        PisPasep parsed = PisPasep.Parse(value, CultureInfo.InvariantCulture);
        bool success = PisPasep.TryParse(value, CultureInfo.InvariantCulture, out PisPasep tryParsed);

        Assert.True(success);
        Assert.Equal("12044529868", parsed.Value);
        Assert.Equal(parsed, tryParsed);
    }

    [Fact]
    public void DefaultPisPasepValueThrows()
    {
        PisPasep pisPasep = default;

        Assert.Throws<InvalidOperationException>(() => pisPasep.Value);
    }
}
