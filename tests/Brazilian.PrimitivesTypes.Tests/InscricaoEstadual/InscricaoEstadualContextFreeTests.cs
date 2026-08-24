using Brazilian.PrimitivesTypes;
using Xunit;

namespace Brazilian.PrimitivesTypes.Tests;

public sealed class InscricaoEstadualContextFreeTests
{
    [Theory]
    [InlineData("12345678")]
    [InlineData("012345678")]
    [InlineData("0012345678")]
    [InlineData("00012345678")]
    [InlineData("000012345678")]
    [InlineData("0000012345678")]
    [InlineData("00000012345678")]
    public void ParseAcceptsEightToFourteenAsciiDigits(string value)
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
    [InlineData("\uFF11\uFF12\uFF13\uFF14\uFF15\uFF16\uFF17\uFF18")]
    public void TryParseRejectsUnsupportedShapes(string? value)
    {
        Assert.False(InscricaoEstadual.TryParse(value, out InscricaoEstadual result));
        Assert.Equal(default, result);
    }

    [Fact]
    public void EqualityPreservesKnownStateContext()
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
