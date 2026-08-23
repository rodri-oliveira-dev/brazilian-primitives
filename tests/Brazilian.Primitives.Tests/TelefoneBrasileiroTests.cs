using System.Globalization;
using Brazilian.Primitives;
using Xunit;

namespace Brazilian.Primitives.Tests;

public sealed class TelefoneBrasileiroTests
{
    [Theory]
    [InlineData("1132345678")]
    [InlineData("(11) 3234-5678")]
    [InlineData("+551132345678")]
    [InlineData("+55 11 3234-5678")]
    public void ParseAcceptsLandlineRepresentations(string value)
    {
        TelefoneBrasileiro telefone = TelefoneBrasileiro.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal(TipoTelefoneBrasileiro.Fixo, telefone.Tipo);
        Assert.Equal("1132345678", telefone.Value);
        Assert.Equal("11", telefone.AreaCode);
        Assert.Equal("32345678", telefone.SubscriberNumber);
        Assert.Equal("(11) 3234-5678", telefone.Formatted);
        Assert.Equal("+551132345678", telefone.E164);
    }

    [Theory]
    [InlineData("11987654321")]
    [InlineData("(11) 98765-4321")]
    [InlineData("+5511987654321")]
    [InlineData("+55 11 98765-4321")]
    public void ParseAcceptsMobileRepresentations(string value)
    {
        TelefoneBrasileiro telefone = TelefoneBrasileiro.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal(TipoTelefoneBrasileiro.Celular, telefone.Tipo);
        Assert.Equal("11987654321", telefone.Value);
        Assert.Equal("11", telefone.AreaCode);
        Assert.Equal("987654321", telefone.SubscriberNumber);
        Assert.Equal("(11) 98765-4321", telefone.Formatted);
        Assert.Equal("+5511987654321", telefone.E164);
    }

    [Fact]
    public void FactoriesCreateTypeSafeValues()
    {
        LandlinePhone fixo = LandlinePhone.Parse("1132345678", CultureInfo.InvariantCulture);
        MobilePhone celular = MobilePhone.Parse("11987654321", CultureInfo.InvariantCulture);

        TelefoneBrasileiro fromFixo = TelefoneBrasileiro.From(fixo);
        TelefoneBrasileiro fromCelular = TelefoneBrasileiro.From(celular);

        Assert.True(fromFixo.TryGetTelefoneFixo(out LandlinePhone recoveredFixo));
        Assert.True(fromCelular.TryGetCelular(out MobilePhone recoveredCelular));
        Assert.False(fromFixo.TryGetCelular(out _));
        Assert.False(fromCelular.TryGetTelefoneFixo(out _));
        Assert.Equal(fixo, recoveredFixo);
        Assert.Equal(celular, recoveredCelular);
    }

    [Theory]
    [InlineData("(11) 3234-5678", "+55 11 3234-5678")]
    [InlineData("(11) 98765-4321", "+55 11 98765-4321")]
    public void EqualityUsesDelegatedCanonicalValue(string left, string right)
    {
        TelefoneBrasileiro first = TelefoneBrasileiro.Parse(left, CultureInfo.InvariantCulture);
        TelefoneBrasileiro second = TelefoneBrasileiro.Parse(right, CultureInfo.InvariantCulture);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1032345678")]
    [InlineData("32345678")]
    [InlineData("987654321")]
    [InlineData("1187654321")]
    [InlineData("+12125550123")]
    [InlineData("08001234567")]
    [InlineData("telefone 1132345678")]
    [InlineData("１１３２３４５６７８")]
    public void TryParseReturnsFalseWhenSpecificParsersRejectInput(string? value)
    {
        bool parsed = TelefoneBrasileiro.TryParse(value, out TelefoneBrasileiro result);

        Assert.False(parsed);
        Assert.Equal(default, result);
    }

    [Fact]
    public void ParseAndTryParseSupportSpanContracts()
    {
        ReadOnlySpan<char> value = "+55 11 98765-4321".AsSpan();

        TelefoneBrasileiro parsed = TelefoneBrasileiro.Parse(value, CultureInfo.InvariantCulture);
        bool success = TelefoneBrasileiro.TryParse(value, CultureInfo.InvariantCulture, out TelefoneBrasileiro tryParsed);

        Assert.True(success);
        Assert.Equal(TipoTelefoneBrasileiro.Celular, parsed.Tipo);
        Assert.Equal(parsed, tryParsed);
    }

    [Fact]
    public void ToStringFormatsAreDelegated()
    {
        TelefoneBrasileiro telefone = TelefoneBrasileiro.Parse("1132345678", CultureInfo.InvariantCulture);

        Assert.Equal("1132345678", telefone.ToString());
        Assert.Equal("1132345678", telefone.ToString("G", formatProvider: null));
        Assert.Equal("(11) 3234-5678", telefone.ToString("F", formatProvider: null));
        Assert.Equal("+551132345678", telefone.ToString("E", formatProvider: null));
        Assert.Throws<FormatException>(() => telefone.ToString("X", formatProvider: null));
    }

    [Fact]
    public void DefaultTelefoneBrasileiroValueThrows()
    {
        TelefoneBrasileiro telefone = default;

        Assert.Throws<InvalidOperationException>(() => telefone.Value);
        Assert.Throws<InvalidOperationException>(() => telefone.Formatted);
        Assert.Throws<InvalidOperationException>(() => telefone.E164);
    }
}
