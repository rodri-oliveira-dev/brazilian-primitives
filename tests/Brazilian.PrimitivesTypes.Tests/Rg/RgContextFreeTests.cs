using Brazilian.PrimitivesTypes;
using Xunit;

namespace Brazilian.PrimitivesTypes.Tests;

public sealed class RgContextFreeTests
{
    [Theory]
    [InlineData("123456")]
    [InlineData("1234567")]
    [InlineData("12345678")]
    [InlineData("123456789")]
    [InlineData("00000005x")]
    [InlineData("1234567890")]
    public void ParseAcceptsConservativeCanonicalShapes(string value)
    {
        Rg rg = Rg.Parse(value);

        Assert.Equal(value.ToUpperInvariant(), rg.Value);
        Assert.Equal(BrazilianState.Unknown, rg.State);
        Assert.False(rg.HasState);
        Assert.Equal(rg.Value, rg.Formatted);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("12345")]
    [InlineData("12345678901")]
    [InlineData("12345X")]
    [InlineData("1234567X")]
    [InlineData("12345678A")]
    [InlineData("12.345.678-9")]
    [InlineData("\uFF11\uFF12\uFF13\uFF14\uFF15\uFF16\uFF17\uFF18\uFF19")]
    public void TryParseRejectsUnsupportedShapes(string? value)
    {
        Assert.False(Rg.TryParse(value, out Rg result));
        Assert.Equal(default, result);
    }

    [Fact]
    public void IsValidDoesNotApplySaoPauloChecksumWithoutState()
    {
        const string invalidForSaoPaulo = "120300012";

        Assert.True(Rg.IsValid(invalidForSaoPaulo));
        Assert.False(Rg.IsValid(invalidForSaoPaulo, BrazilianState.SaoPaulo));
    }

    [Fact]
    public void EqualityPreservesKnownStateContext()
    {
        Rg contextFree = Rg.Parse("123456789");
        Rg amazonas = Rg.Parse("123456789", BrazilianState.Amazonas);
        Rg amapa = Rg.Parse("123456789", BrazilianState.Amapa);

        Assert.NotEqual(contextFree, amazonas);
        Assert.NotEqual(amazonas, amapa);
        Assert.Equal(contextFree.Value, amazonas.Value);
        Assert.True(amazonas.HasState);
    }
}
