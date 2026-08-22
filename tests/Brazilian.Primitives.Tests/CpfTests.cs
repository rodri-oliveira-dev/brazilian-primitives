using Brazilian.Primitives;
using Xunit;

namespace Brazilian.Primitives.Tests;

public sealed class CpfTests
{
    [Theory]
    [InlineData("52998224725")]
    [InlineData("16899535009")]
    [InlineData("11144477735")]
    [InlineData("12345678909")]
    [InlineData("93541134780")]
    [InlineData("01234567890")]
    public void IsValid_ReturnsTrue_ForKnownValidCpfs(string value)
    {
        Assert.True(Cpf.IsValid(value));
    }

    [Theory]
    [InlineData("52998224725")]
    [InlineData("529.982.247-25")]
    public void Parse_NormalizesSupportedRepresentations(string value)
    {
        Cpf cpf = Cpf.Parse(value);

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
    public void IsValid_ReturnsFalse_ForRepeatedDigits(string value)
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
    public void TryParse_ReturnsFalse_ForInvalidInput(string? value)
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
    public void Parse_ThrowsFormatException_ForInvalidInput(string value)
    {
        Assert.Throws<FormatException>(() => Cpf.Parse(value));
    }

    [Fact]
    public void Parse_ThrowsFormatException_ForNullInput()
    {
        Assert.Throws<FormatException>(() => Cpf.Parse(null!));
    }

    [Fact]
    public void Equality_UsesNormalizedValue()
    {
        Cpf unmasked = Cpf.Parse("52998224725");
        Cpf masked = Cpf.Parse("529.982.247-25");

        Assert.Equal(unmasked, masked);
        Assert.Equal(unmasked.GetHashCode(), masked.GetHashCode());
    }

    [Fact]
    public void Parse_AndTryParse_SupportSpanContracts()
    {
        ReadOnlySpan<char> value = "529.982.247-25".AsSpan();

        Cpf parsed = Cpf.Parse(value, provider: null);
        bool success = Cpf.TryParse(value, provider: null, out Cpf tryParsed);

        Assert.True(success);
        Assert.Equal("52998224725", parsed.Value);
        Assert.Equal(parsed, tryParsed);
    }

    [Fact]
    public void ToString_ThrowsFormatException_ForUnsupportedFormat()
    {
        Cpf cpf = Cpf.Parse("52998224725");

        Assert.Throws<FormatException>(() => cpf.ToString("X", formatProvider: null));
    }
}
