using System.Globalization;
using Brazilian.PrimitivesTypes;
using Xunit;

namespace Brazilian.PrimitivesTypes.Tests;

public sealed class CpfTests
{
    [Theory]
    [InlineData("52998224725")]
    [InlineData("16899535009")]
    [InlineData("11144477735")]
    [InlineData("12345678909")]
    [InlineData("93541134780")]
    [InlineData("01234567890")]
    public void IsValidReturnsTrueForKnownValidCpfs(string value)
    {
        Assert.True(Cpf.IsValid(value));
    }

    [Theory]
    [InlineData("52998224725")]
    [InlineData("529.982.247-25")]
    public void ParseNormalizesSupportedRepresentations(string value)
    {
        Cpf cpf = Cpf.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal("52998224725", cpf.Value);
        Assert.Equal("529.982.247-25", cpf.Formatted);
        Assert.Equal("52998224725", cpf.ToString());
        Assert.Equal("52998224725", cpf.ToString("G", formatProvider: null));
        Assert.Equal("529.982.247-25", cpf.ToString("F", formatProvider: null));
    }

    [Theory]
    [InlineData("00000000000")]
    [InlineData("11111111111")]
    [InlineData("22222222222")]
    [InlineData("33333333333")]
    [InlineData("44444444444")]
    [InlineData("55555555555")]
    [InlineData("66666666666")]
    [InlineData("77777777777")]
    [InlineData("88888888888")]
    [InlineData("99999999999")]
    public void IsValidReturnsFalseForRepeatedDigits(string value)
    {
        Assert.False(Cpf.IsValid(value));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("5299822472")]
    [InlineData("529982247250")]
    [InlineData("52998224735")]
    [InlineData("52998224724")]
    [InlineData("529abc98224725")]
    [InlineData("529.982.247-2A")]
    [InlineData("529-982-247.25")]
    [InlineData("529 982 247 25")]
    [InlineData("５２９９８２２４７２５")]
    public void TryParseReturnsFalseForInvalidInput(string? value)
    {
        bool parsed = Cpf.TryParse(value, out Cpf result);

        Assert.False(parsed);
        Assert.Equal(default, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("5299822472")]
    [InlineData("52998224735")]
    [InlineData("529abc98224725")]
    public void ParseThrowsFormatExceptionForInvalidInput(string value)
    {
        Assert.Throws<FormatException>(() => Cpf.Parse(value, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ParseThrowsFormatExceptionForNullInput()
    {
        Assert.Throws<FormatException>(() => Cpf.Parse(null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void EqualityUsesNormalizedValue()
    {
        Cpf unmasked = Cpf.Parse("52998224725", CultureInfo.InvariantCulture);
        Cpf masked = Cpf.Parse("529.982.247-25", CultureInfo.InvariantCulture);

        Assert.Equal(unmasked, masked);
        Assert.Equal(unmasked.GetHashCode(), masked.GetHashCode());
    }

    [Fact]
    public void ParseAndTryParseSupportSpanContracts()
    {
        ReadOnlySpan<char> value = "529.982.247-25".AsSpan();

        Cpf parsed = Cpf.Parse(value, CultureInfo.InvariantCulture);
        bool success = Cpf.TryParse(value, CultureInfo.InvariantCulture, out Cpf tryParsed);

        Assert.True(success);
        Assert.Equal("52998224725", parsed.Value);
        Assert.Equal(parsed, tryParsed);
    }

    [Fact]
    public void ToStringThrowsFormatExceptionForUnsupportedFormat()
    {
        Cpf cpf = Cpf.Parse("52998224725", CultureInfo.InvariantCulture);

        Assert.Throws<FormatException>(() => cpf.ToString("X", formatProvider: null));
    }
}
