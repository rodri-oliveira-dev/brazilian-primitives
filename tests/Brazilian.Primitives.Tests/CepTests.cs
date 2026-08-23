using System.Globalization;
using Brazilian.Primitives;
using Xunit;

namespace Brazilian.Primitives.Tests;

public sealed class CepTests
{
    [Theory]
    [InlineData("01311000")]
    [InlineData("01311-000")]
    public void ParseNormalizesSupportedRepresentations(string value)
    {
        Cep cep = Cep.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal("01311000", cep.Value);
        Assert.Equal("01311-000", cep.Formatted);
        Assert.Equal("01311000", cep.ToString());
        Assert.Equal("01311000", cep.ToString("G", formatProvider: null));
        Assert.Equal("01311-000", cep.ToString("F", formatProvider: null));
    }

    [Theory]
    [InlineData("00000000")]
    [InlineData("01001001")]
    [InlineData("01311000")]
    [InlineData("99999999")]
    public void IsValidAcceptsAnyEightAsciiDigitsStructurally(string value)
    {
        Assert.True(Cep.IsValid(value));
    }

    [Fact]
    public void LeadingZeroIsPreserved()
    {
        Cep cep = Cep.Parse("01001-001", CultureInfo.InvariantCulture);

        Assert.Equal("01001001", cep.Value);
        Assert.Equal("01001-001", cep.Formatted);
    }

    [Fact]
    public void StructuralValidityDoesNotRequireDneLookup()
    {
        Assert.True(Cep.IsValid("00000000"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("0131100")]
    [InlineData("013110000")]
    [InlineData("0131-1000")]
    [InlineData("01311 000")]
    [InlineData("01311.000")]
    [InlineData("01311--000")]
    [InlineData("01311-00A")]
    [InlineData("abc01311000")]
    [InlineData("01311-000 ")]
    [InlineData(" 01311-000")]
    [InlineData("０１３１１０００")]
    public void TryParseReturnsFalseForInvalidInput(string? value)
    {
        bool parsed = Cep.TryParse(value, out Cep result);

        Assert.False(parsed);
        Assert.Equal(default, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("0131100")]
    [InlineData("013110000")]
    [InlineData("0131-1000")]
    [InlineData("abc01311000")]
    public void ParseThrowsFormatExceptionForInvalidInput(string value)
    {
        Assert.Throws<FormatException>(() => Cep.Parse(value, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ParseThrowsFormatExceptionForNullInput()
    {
        Assert.Throws<FormatException>(() => Cep.Parse(null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void EqualityUsesCanonicalValue()
    {
        Cep unmasked = Cep.Parse("01311000", CultureInfo.InvariantCulture);
        Cep masked = Cep.Parse("01311-000", CultureInfo.InvariantCulture);

        Assert.Equal(unmasked, masked);
        Assert.Equal(unmasked.GetHashCode(), masked.GetHashCode());
    }

    [Fact]
    public void ParseAndTryParseSupportSpanContracts()
    {
        ReadOnlySpan<char> value = "01311-000".AsSpan();

        Cep parsed = Cep.Parse(value, CultureInfo.InvariantCulture);
        bool success = Cep.TryParse(value, CultureInfo.InvariantCulture, out Cep tryParsed);

        Assert.True(success);
        Assert.Equal("01311000", parsed.Value);
        Assert.Equal(parsed, tryParsed);
    }

    [Fact]
    public void ToStringThrowsFormatExceptionForUnsupportedFormat()
    {
        Cep cep = Cep.Parse("01311000", CultureInfo.InvariantCulture);

        Assert.Throws<FormatException>(() => cep.ToString("X", formatProvider: null));
    }

    [Fact]
    public void DefaultInstanceDoesNotExposeAValue()
    {
        Cep cep = default;

        Assert.Throws<InvalidOperationException>(() => cep.Value);
    }
}
