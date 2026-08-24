using Brazilian.PrimitivesTypes;
using Xunit;

namespace Brazilian.PrimitivesTypes.Tests;

public sealed class ContextFreeStateAwarePrimitiveTests
{
    [Theory]
    [InlineData("123456")]
    [InlineData("1234567")]
    [InlineData("12345678")]
    [InlineData("123456789")]
    [InlineData("00000005x")]
    [InlineData("1234567890")]
    public void ContextFreeRgAcceptsConservativeCanonicalShapes(string value)
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
    [InlineData("１２３４５６７８９")]
    public void ContextFreeRgRejectsUnsupportedShapes(string? value)
    {
        Assert.False(Rg.TryParse(value, out Rg result));
        Assert.Equal(default, result);
    }

    [Fact]
    public void ContextFreeRgDoesNotApplySaoPauloChecksumWithoutState()
    {
        const string invalidForSaoPaulo = "120300012";

        Assert.True(Rg.IsValid(invalidForSaoPaulo));
        Assert.False(Rg.IsValid(invalidForSaoPaulo, BrazilianState.SaoPaulo));
    }

    [Fact]
    public void RgEqualityPreservesKnownStateContext()
    {
        Rg contextFree = Rg.Parse("123456789");
        Rg amazonas = Rg.Parse("123456789", BrazilianState.Amazonas);
        Rg amapa = Rg.Parse("123456789", BrazilianState.Amapa);

        Assert.NotEqual(contextFree, amazonas);
        Assert.NotEqual(amazonas, amapa);
        Assert.Equal(contextFree.Value, amazonas.Value);
        Assert.True(amazonas.HasState);
    }

    [Theory]
    [InlineData("12345678")]
    [InlineData("012345678")]
    [InlineData("0012345678")]
    [InlineData("00012345678")]
    [InlineData("000012345678")]
    [InlineData("0000012345678")]
    [InlineData("00000012345678")]
    public void ContextFreeInscricaoEstadualAcceptsEightToFourteenAsciiDigits(string value)
    {
        InscricaoEstadual inscricao = InscricaoEstadual.Parse(value);

        Assert.Equal(value, inscricao.Value);
        Assert.Equal(BrazilianState.Unknown, inscricao.State);
        Assert.False(inscricao.HasState);
        Assert.Equal(value, inscricao.Formatted);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("1234567")]
    [InlineData("123456789012345")]
    [InlineData("ISENTO")]
    [InlineData("1234567A")]
    [InlineData("110.042.490.114")]
    [InlineData("１２３４５６７８")]
    public void ContextFreeInscricaoEstadualRejectsUnsupportedShapes(string? value)
    {
        Assert.False(InscricaoEstadual.TryParse(value, out InscricaoEstadual result));
        Assert.Equal(default, result);
    }

    [Fact]
    public void InscricaoEstadualEqualityPreservesKnownStateContext()
    {
        InscricaoEstadual contextFree = InscricaoEstadual.Parse("123456789");
        InscricaoEstadual amazonas = InscricaoEstadual.Parse("123456789", BrazilianState.Amazonas);
        InscricaoEstadual ceara = InscricaoEstadual.Parse("123456789", BrazilianState.Ceara);

        Assert.NotEqual(contextFree, amazonas);
        Assert.NotEqual(amazonas, ceara);
        Assert.Equal(contextFree.Value, amazonas.Value);
        Assert.True(amazonas.HasState);
    }
}
