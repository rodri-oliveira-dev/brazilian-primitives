using Brazilian.PrimitivesTypes;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Converters;

public sealed class InscricaoEstadualValueConverterTests
{
    [Fact]
    public void ConverterRoundTripsContextFreeInscricaoEstadual()
    {
        InscricaoEstadualValueConverter converter = new();
        InscricaoEstadual contextFreeInscricao = InscricaoEstadual.Parse("0012345678");

        Assert.Equal("0012345678", converter.ConvertToProvider(contextFreeInscricao));
        InscricaoEstadual roundTrip = Assert.IsType<InscricaoEstadual>(
            converter.ConvertFromProvider("0012345678"));
        Assert.Equal(contextFreeInscricao, roundTrip);
        Assert.False(roundTrip.HasState);
    }

    [Fact]
    public void ConverterRefusesToDiscardKnownState()
    {
        InscricaoEstadualValueConverter converter = new();
        InscricaoEstadual stateAwareInscricao =
            InscricaoEstadual.Parse("110042490114", BrazilianState.SaoPaulo);

        Assert.Throws<InvalidOperationException>(() => converter.ConvertToProvider(stateAwareInscricao));
    }

    [Fact]
    public void InvalidPersistedValueFailsThroughContextFreeParser()
    {
        InscricaoEstadualValueConverter converter = new();

        Assert.Throws<FormatException>(() => converter.ConvertFromProvider("123"));
    }
}
