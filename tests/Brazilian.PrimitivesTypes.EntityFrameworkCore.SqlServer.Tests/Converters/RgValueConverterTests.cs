using Brazilian.PrimitivesTypes;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Converters;

public sealed class RgValueConverterTests
{
    [Fact]
    public void ConverterRoundTripsContextFreeRg()
    {
        RgValueConverter converter = new();
        Rg contextFreeRg = Rg.Parse("00000005x");

        Assert.Equal("00000005X", converter.ConvertToProvider(contextFreeRg));
        Assert.Equal(contextFreeRg, converter.ConvertFromProvider("00000005X"));
    }

    [Fact]
    public void ConverterRefusesToDiscardKnownState()
    {
        RgValueConverter converter = new();
        Rg stateAwareRg = Rg.Parse("123456789", BrazilianState.Amazonas);

        Assert.Throws<InvalidOperationException>(() => converter.ConvertToProvider(stateAwareRg));
    }
}
