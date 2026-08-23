using System.Globalization;
using Brazilian.Primitives;
using Xunit;

namespace Brazilian.Primitives.Tests;

public sealed class ChavePixTests
{
    [Theory]
    [InlineData("52998224725")]
    [InlineData("529.982.247-25")]
    public void ParseAcceptsCpfKeys(string value)
    {
        ChavePix chave = ChavePix.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal(TipoChavePix.Cpf, chave.Tipo);
        Assert.Equal("52998224725", chave.Value);
        Assert.Equal(chave.Value, chave.ToString());
    }

    [Theory]
    [InlineData("11222333000181", "11222333000181")]
    [InlineData("11.222.333/0001-81", "11222333000181")]
    [InlineData("00000000e08g12", "00000000E08G12")]
    public void ParseAcceptsCnpjKeys(string value, string expected)
    {
        ChavePix chave = ChavePix.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal(TipoChavePix.Cnpj, chave.Tipo);
        Assert.Equal(expected, chave.Value);
    }

    [Theory]
    [InlineData("11987654321")]
    [InlineData("(11) 98765-4321")]
    [InlineData("+5511987654321")]
    [InlineData("+55 11 98765-4321")]
    public void ParseAcceptsMobilePhoneKeysAsE164(string value)
    {
        ChavePix chave = ChavePix.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal(TipoChavePix.Celular, chave.Tipo);
        Assert.Equal("+5511987654321", chave.Value);
    }

    [Fact]
    public void TryParseRejectsAmbiguousCpfAndMobilePhoneKey()
    {
        const string value = "11900000083";

        Assert.True(Cpf.IsValid(value));
        Assert.True(MobilePhone.IsValid(value));
        Assert.False(ChavePix.TryParse(value, out ChavePix result));
        Assert.Equal(default, result);
        Assert.Throws<FormatException>(() => ChavePix.Parse(value, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void FactoriesAllowExplicitAmbiguousCpfAndMobilePhoneKeys()
    {
        const string value = "11900000083";

        ChavePix cpf = ChavePix.From(Cpf.Parse(value, CultureInfo.InvariantCulture));
        ChavePix celular = ChavePix.From(MobilePhone.Parse(value, CultureInfo.InvariantCulture));

        Assert.Equal(TipoChavePix.Cpf, cpf.Tipo);
        Assert.Equal(value, cpf.Value);
        Assert.Equal(TipoChavePix.Celular, celular.Tipo);
        Assert.Equal("+5511900000083", celular.Value);
    }

    [Theory]
    [InlineData("User@Example.COM", "user@example.com")]
    [InlineData("usuario@domínio.com", "usuario@xn--domnio-5va.com")]
    public void ParseAcceptsEmailKeysWithPixLowercaseCanonicalization(string value, string expected)
    {
        ChavePix chave = ChavePix.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal(TipoChavePix.Email, chave.Tipo);
        Assert.Equal(expected, chave.Value);
    }

    [Fact]
    public void ParseAcceptsPixEmailAtMaximumLength()
    {
        string value = new string('a', 64) + "@example.info";

        ChavePix chave = ChavePix.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal(77, chave.Value.Length);
        Assert.Equal(TipoChavePix.Email, chave.Tipo);
    }

    [Fact]
    public void TryParseRejectsPixEmailAboveMaximumLengthEvenWhenEmailIsValid()
    {
        string value = new string('a', 64) + "@example.email";

        Assert.True(Email.IsValid(value));
        Assert.False(ChavePix.TryParse(value, out _));
    }

    [Theory]
    [InlineData("550e8400-e29b-41d4-a716-446655440000")]
    [InlineData("550E8400-E29B-41D4-A716-446655440000")]
    public void ParseAcceptsCanonicalRandomKeyAndNormalizesHexCase(string value)
    {
        ChavePix chave = ChavePix.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal(TipoChavePix.Aleatoria, chave.Tipo);
        Assert.Equal("550e8400-e29b-41d4-a716-446655440000", chave.Value);
    }

    [Fact]
    public void FactoriesCreateTypedKeys()
    {
        ChavePix cpf = ChavePix.From(Cpf.Parse("52998224725", CultureInfo.InvariantCulture));
        ChavePix cnpj = ChavePix.From(Cnpj.Parse("11222333000181", CultureInfo.InvariantCulture));
        ChavePix celular = ChavePix.From(MobilePhone.Parse("11987654321", CultureInfo.InvariantCulture));
        ChavePix email = ChavePix.From(Email.Parse("User@Example.COM", CultureInfo.InvariantCulture));
        ChavePix aleatoria = ChavePix.FromChaveAleatoria("550e8400-e29b-41d4-a716-446655440000");

        Assert.Equal(TipoChavePix.Cpf, cpf.Tipo);
        Assert.Equal(TipoChavePix.Cnpj, cnpj.Tipo);
        Assert.Equal(TipoChavePix.Celular, celular.Tipo);
        Assert.Equal(TipoChavePix.Email, email.Tipo);
        Assert.Equal(TipoChavePix.Aleatoria, aleatoria.Tipo);
        Assert.Equal("user@example.com", email.Value);
    }

    [Theory]
    [InlineData("52998224724")]
    [InlineData("11222333000180")]
    [InlineData("1132345678")]
    [InlineData("+12125550123")]
    [InlineData("1187654321")]
    [InlineData("User Name@example.com")]
    [InlineData("{550e8400-e29b-41d4-a716-446655440000}")]
    [InlineData("urn:uuid:550e8400-e29b-41d4-a716-446655440000")]
    [InlineData("550e8400e29b41d4a716446655440000")]
    [InlineData("550e8400-e29b-41d4-a716-44665544000z")]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData(null)]
    public void TryParseReturnsFalseForInvalidKeys(string? value)
    {
        bool parsed = ChavePix.TryParse(value, out ChavePix result);

        Assert.False(parsed);
        Assert.Equal(default, result);
    }

    [Fact]
    public void FromEmailThrowsWhenPixEmailIsTooLong()
    {
        Email email = Email.Parse(new string('a', 64) + "@example.email", CultureInfo.InvariantCulture);

        Assert.Throws<FormatException>(() => ChavePix.From(email));
    }

    [Fact]
    public void FromChaveAleatoriaThrowsForInvalidUuidText()
    {
        Assert.Throws<FormatException>(() => ChavePix.FromChaveAleatoria("{550e8400-e29b-41d4-a716-446655440000}"));
    }

    [Fact]
    public void EqualityUsesCanonicalPixValueAndType()
    {
        ChavePix formattedCpf = ChavePix.Parse("529.982.247-25", CultureInfo.InvariantCulture);
        ChavePix rawCpf = ChavePix.Parse("52998224725", CultureInfo.InvariantCulture);
        ChavePix email = ChavePix.Parse("52998224725@example.com", CultureInfo.InvariantCulture);

        Assert.Equal(rawCpf, formattedCpf);
        Assert.Equal(rawCpf.GetHashCode(), formattedCpf.GetHashCode());
        Assert.NotEqual(rawCpf, email);
    }

    [Fact]
    public void ParseAndTryParseSupportSpanContracts()
    {
        ReadOnlySpan<char> value = "+55 11 98765-4321".AsSpan();

        ChavePix parsed = ChavePix.Parse(value, CultureInfo.InvariantCulture);
        bool success = ChavePix.TryParse(value, CultureInfo.InvariantCulture, out ChavePix tryParsed);

        Assert.True(success);
        Assert.Equal("+5511987654321", parsed.Value);
        Assert.Equal(parsed, tryParsed);
    }

    [Fact]
    public void DefaultChavePixValueThrows()
    {
        ChavePix chave = default;

        Assert.Throws<InvalidOperationException>(() => chave.Value);
    }
}
