using System.Globalization;
using Brazilian.Primitives;
using Xunit;

namespace Brazilian.Primitives.Tests;

public sealed class EmailTests
{
    [Theory]
    [InlineData("user@example.com", "user@example.com", "user", "example.com")]
    [InlineData("john.doe@example.com", "john.doe@example.com", "john.doe", "example.com")]
    [InlineData("john+notifications@example.com", "john+notifications@example.com", "john+notifications", "example.com")]
    [InlineData("first_last@example.com", "first_last@example.com", "first_last", "example.com")]
    [InlineData("o'hara@example.com", "o'hara@example.com", "o'hara", "example.com")]
    [InlineData("!#$%&'*+-/=?^_`{|}~@example.com", "!#$%&'*+-/=?^_`{|}~@example.com", "!#$%&'*+-/=?^_`{|}~", "example.com")]
    [InlineData("USER@Example.COM", "USER@example.com", "USER", "example.com")]
    [InlineData("user@sub.example.com", "user@sub.example.com", "user", "sub.example.com")]
    [InlineData("usuario@domínio.com", "usuario@xn--domnio-5va.com", "usuario", "xn--domnio-5va.com")]
    public void ParseNormalizesSupportedEmails(string value, string expectedValue, string expectedLocalPart, string expectedDomain)
    {
        Email email = Email.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal(expectedValue, email.Value);
        Assert.Equal(expectedLocalPart, email.LocalPart);
        Assert.Equal(expectedDomain, email.Domain);
        Assert.Equal(expectedValue, email.ToString());
    }

    [Fact]
    public void ParseAcceptsLocalPartAtMaximumLength()
    {
        string localPart = new('a', 64);

        Email email = Email.Parse(localPart + "@example.com", CultureInfo.InvariantCulture);

        Assert.Equal(localPart, email.LocalPart);
    }

    [Fact]
    public void ParseAcceptsCanonicalAddressAtMaximumLength()
    {
        string localPart = new('a', 64);
        string label63 = new('b', 63);
        string label57 = new('c', 57);
        string value = $"{localPart}@{label63}.{label63}.{label57}.com";

        Email email = Email.Parse(value, CultureInfo.InvariantCulture);

        Assert.Equal(254, email.Value.Length);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("user.example.com")]
    [InlineData("user@@example.com")]
    [InlineData("@example.com")]
    [InlineData("user@")]
    [InlineData(".user@example.com")]
    [InlineData("user.@example.com")]
    [InlineData("user..name@example.com")]
    [InlineData("\"user name\"@example.com")]
    [InlineData("user name@example.com")]
    [InlineData("us\ter@example.com")]
    [InlineData("usuário@example.com")]
    [InlineData("user@example..com")]
    [InlineData("user@-example.com")]
    [InlineData("user@example-.com")]
    [InlineData("user@example_com")]
    [InlineData("user@example.com.")]
    [InlineData("user@[127.0.0.1]")]
    [InlineData("John Doe <john@example.com>")]
    [InlineData("<john@example.com>")]
    [InlineData("mailto:john@example.com")]
    [InlineData("john@example.com;other@example.com")]
    [InlineData("abc john@example.com xyz")]
    [InlineData("ｕｓｅｒ@example.com")]
    public void TryParseReturnsFalseForInvalidEmails(string? value)
    {
        bool parsed = Email.TryParse(value, out Email result);

        Assert.False(parsed);
        Assert.Equal(default, result);
    }

    [Fact]
    public void TryParseRejectsLabelAboveMaximumLength()
    {
        string value = "user@" + new string('a', 64) + ".com";

        Assert.False(Email.TryParse(value, out _));
    }

    [Fact]
    public void TryParseRejectsLocalPartAboveMaximumLength()
    {
        string value = new string('a', 65) + "@example.com";

        Assert.False(Email.TryParse(value, out _));
    }

    [Fact]
    public void TryParseRejectsCanonicalAddressAboveMaximumLength()
    {
        string localPart = new('a', 64);
        string label63 = new('b', 63);
        string value = $"{localPart}@{label63}.{label63}.{label63}.com";

        Assert.False(Email.TryParse(value, out _));
    }

    [Theory]
    [InlineData("")]
    [InlineData("user.example.com")]
    [InlineData("user@@example.com")]
    [InlineData("usuário@example.com")]
    public void ParseThrowsFormatExceptionForInvalidInput(string value)
    {
        Assert.Throws<FormatException>(() => Email.Parse(value, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ParseThrowsFormatExceptionForNullInput()
    {
        Assert.Throws<FormatException>(() => Email.Parse(null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void EqualityUsesCanonicalDomainButPreservesLocalPartCase()
    {
        Email lowerDomain = Email.Parse("User@example.com", CultureInfo.InvariantCulture);
        Email upperDomain = Email.Parse("User@EXAMPLE.COM", CultureInfo.InvariantCulture);
        Email differentLocalPart = Email.Parse("user@example.com", CultureInfo.InvariantCulture);

        Assert.Equal(lowerDomain, upperDomain);
        Assert.Equal(lowerDomain.GetHashCode(), upperDomain.GetHashCode());
        Assert.NotEqual(lowerDomain, differentLocalPart);
    }

    [Fact]
    public void EqualityUsesPunycodeDomain()
    {
        Email unicode = Email.Parse("usuario@domínio.com", CultureInfo.InvariantCulture);
        Email punycode = Email.Parse("usuario@xn--domnio-5va.com", CultureInfo.InvariantCulture);

        Assert.Equal(unicode, punycode);
        Assert.Equal(unicode.GetHashCode(), punycode.GetHashCode());
    }

    [Fact]
    public void ParseAndTryParseSupportSpanContracts()
    {
        ReadOnlySpan<char> value = "User@Example.COM".AsSpan();

        Email parsed = Email.Parse(value, CultureInfo.InvariantCulture);
        bool success = Email.TryParse(value, CultureInfo.InvariantCulture, out Email tryParsed);

        Assert.True(success);
        Assert.Equal("User@example.com", parsed.Value);
        Assert.Equal(parsed, tryParsed);
    }

    [Fact]
    public void DefaultEmailValueThrows()
    {
        Email email = default;

        Assert.Throws<InvalidOperationException>(() => email.Value);
    }
}
