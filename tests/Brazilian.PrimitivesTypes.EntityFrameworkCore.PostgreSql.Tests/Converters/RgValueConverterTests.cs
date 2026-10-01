using Brazilian.PrimitivesTypes;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Converters;

public sealed class RgValueConverterTests
{
    [Fact]
    public void ConverterRoundTripsContextFreeRg()
    {
        RgValueConverter converter = new();
        Rg contextFreeRg = Rg.Parse("00000005x");

        Assert.Equal("00000005X", converter.ConvertToProvider(contextFreeRg));
        Rg roundTrip = Assert.IsType<Rg>(converter.ConvertFromProvider("00000005X"));
        Assert.Equal(contextFreeRg, roundTrip);
        Assert.False(roundTrip.HasState);
    }

    [Fact]
    public void ConverterRefusesToDiscardKnownState()
    {
        RgValueConverter converter = new();
        Rg stateAwareRg = Rg.Parse("123456789", BrazilianState.Amazonas);

        Assert.Throws<InvalidOperationException>(() => converter.ConvertToProvider(stateAwareRg));
    }

    [Fact]
    public void InvalidPersistedValueFailsThroughContextFreeParser()
    {
        RgValueConverter converter = new();

        Assert.Throws<FormatException>(() => converter.ConvertFromProvider("invalid-rg"));
    }
}
