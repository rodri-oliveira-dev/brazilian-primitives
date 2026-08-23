using System.Globalization;
using Brazilian.Primitives;
using Xunit;

namespace Brazilian.Primitives.Tests;

public sealed class MobilePhoneTests
{
    [Theory]
    [InlineData("11987654321")]
    [InlineData("(11) 98765-4321")]
    [InlineData("+55 11 98765-4321")]
    [InlineData("+5511987654321")]
    public void ParseNormalizesSupportedRepresentations(string input)
    {
        MobilePhone phone = MobilePhone.Parse(input, CultureInfo.InvariantCulture);

        Assert.Equal("11987654321", phone.Value);
        Assert.Equal("11", phone.AreaCode);
        Assert.Equal("987654321", phone.SubscriberNumber);
        Assert.Equal("(11) 98765-4321", phone.Formatted);
        Assert.Equal("+5511987654321", phone.E164);
        Assert.Equal("11987654321", phone.ToString());
        Assert.Equal("11987654321", phone.ToString("G", formatProvider: null));
        Assert.Equal("(11) 98765-4321", phone.ToString("F", formatProvider: null));
        Assert.Equal("+5511987654321", phone.ToString("E", formatProvider: null));
    }

    [Theory]
    [InlineData("11")]
    [InlineData("12")]
    [InlineData("13")]
    [InlineData("14")]
    [InlineData("15")]
    [InlineData("16")]
    [InlineData("17")]
    [InlineData("18")]
    [InlineData("19")]
    [InlineData("21")]
    [InlineData("22")]
    [InlineData("24")]
    [InlineData("27")]
    [InlineData("28")]
    [InlineData("31")]
    [InlineData("32")]
    [InlineData("33")]
    [InlineData("34")]
    [InlineData("35")]
    [InlineData("37")]
    [InlineData("38")]
    [InlineData("41")]
    [InlineData("42")]
    [InlineData("43")]
    [InlineData("44")]
    [InlineData("45")]
    [InlineData("46")]
    [InlineData("47")]
    [InlineData("48")]
    [InlineData("49")]
    [InlineData("51")]
    [InlineData("53")]
    [InlineData("54")]
    [InlineData("55")]
    [InlineData("61")]
    [InlineData("62")]
    [InlineData("63")]
    [InlineData("64")]
    [InlineData("65")]
    [InlineData("66")]
    [InlineData("67")]
    [InlineData("68")]
    [InlineData("69")]
    [InlineData("71")]
    [InlineData("73")]
    [InlineData("74")]
    [InlineData("75")]
    [InlineData("77")]
    [InlineData("79")]
    [InlineData("81")]
    [InlineData("82")]
    [InlineData("83")]
    [InlineData("84")]
    [InlineData("85")]
    [InlineData("86")]
    [InlineData("87")]
    [InlineData("88")]
    [InlineData("89")]
    [InlineData("91")]
    [InlineData("92")]
    [InlineData("93")]
    [InlineData("94")]
    [InlineData("95")]
    [InlineData("96")]
    [InlineData("97")]
    [InlineData("98")]
    [InlineData("99")]
    public void IsValidAcceptsEveryOfficialAreaCode(string areaCode)
    {
        string value = string.Concat(areaCode, "987654321");

        Assert.True(MobilePhone.IsValid(value));
    }

    [Theory]
    [InlineData("10")]
    [InlineData("20")]
    [InlineData("23")]
    [InlineData("25")]
    [InlineData("26")]
    [InlineData("29")]
    [InlineData("30")]
    [InlineData("36")]
    [InlineData("39")]
    [InlineData("40")]
    [InlineData("50")]
    [InlineData("52")]
    [InlineData("56")]
    [InlineData("57")]
    [InlineData("58")]
    [InlineData("59")]
    [InlineData("60")]
    [InlineData("70")]
    [InlineData("72")]
    [InlineData("76")]
    [InlineData("78")]
    [InlineData("80")]
    [InlineData("90")]
    public void IsValidRejectsUnassignedAreaCodes(string areaCode)
    {
        string value = string.Concat(areaCode, "987654321");

        Assert.False(MobilePhone.IsValid(value));
    }

    [Fact]
    public void IsValidAcceptsNineDigitSubscriberStartingWithNine()
    {
        Assert.True(MobilePhone.IsValid("11900000000"));
        Assert.True(MobilePhone.IsValid("11999999999"));
    }

    [Theory]
    [InlineData("11612345678")]
    [InlineData("11712345678")]
    [InlineData("11812345678")]
    public void IsValidRejectsLegacyMobileStartingRanges(string value)
    {
        Assert.False(MobilePhone.IsValid(value));
    }

    [Theory]
    [InlineData("11212345678")]
    [InlineData("11312345678")]
    [InlineData("11412345678")]
    [InlineData("11512345678")]
    public void IsValidRejectsLandlineSubscriberRanges(string value)
    {
        Assert.False(MobilePhone.IsValid(value));
    }

    [Theory]
    [InlineData("1187654321")]
    [InlineData("(11) 8765-4321")]
    public void IsValidRejectsEightDigitLegacyMobileFormat(string value)
    {
        Assert.False(MobilePhone.IsValid(value));
    }

    [Theory]
    [InlineData("03001234567")]
    [InlineData("05001234567")]
    [InlineData("08001234567")]
    [InlineData("09001234567")]
    public void IsValidRejectsNonGeographicNumbers(string value)
    {
        Assert.False(MobilePhone.IsValid(value));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("1198765432")]
    [InlineData("119876543210")]
    [InlineData("987654321")]
    [InlineData("+54 11 98765-4321")]
    [InlineData("+5411987654321")]
    [InlineData("(11)98765-4321")]
    [InlineData("11 98765-4321")]
    [InlineData("+55 (11) 98765-4321")]
    [InlineData("(11) 9876-54321")]
    [InlineData("(11) 98765.4321")]
    [InlineData("abc11xyz98765-4321")]
    [InlineData("11A87654321")]
    [InlineData("１１９８７６５４３２１")]
    [InlineData(" 11987654321")]
    [InlineData("11987654321 ")]
    public void TryParseReturnsFalseForInvalidInput(string? value)
    {
        bool parsed = MobilePhone.TryParse(value, out MobilePhone result);

        Assert.False(parsed);
        Assert.Equal(default, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("1187654321")]
    [InlineData("11612345678")]
    [InlineData("11323456789")]
    [InlineData("+54 11 98765-4321")]
    [InlineData("abc11xyz98765-4321")]
    public void ParseThrowsFormatExceptionForInvalidInput(string value)
    {
        Assert.Throws<FormatException>(() => MobilePhone.Parse(value, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ParseThrowsFormatExceptionForNullInput()
    {
        Assert.Throws<FormatException>(() => MobilePhone.Parse(null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void EqualityUsesCanonicalNationalValue()
    {
        MobilePhone raw = MobilePhone.Parse("11987654321", CultureInfo.InvariantCulture);
        MobilePhone formatted = MobilePhone.Parse("(11) 98765-4321", CultureInfo.InvariantCulture);
        MobilePhone international = MobilePhone.Parse("+55 11 98765-4321", CultureInfo.InvariantCulture);
        MobilePhone e164 = MobilePhone.Parse("+5511987654321", CultureInfo.InvariantCulture);

        Assert.Equal(raw, formatted);
        Assert.Equal(raw, international);
        Assert.Equal(raw, e164);
        Assert.Equal(raw.GetHashCode(), formatted.GetHashCode());
    }

    [Fact]
    public void ParseAndTryParseSupportSpanContracts()
    {
        ReadOnlySpan<char> value = "+55 11 98765-4321".AsSpan();

        MobilePhone parsed = MobilePhone.Parse(value, CultureInfo.InvariantCulture);
        bool success = MobilePhone.TryParse(value, CultureInfo.InvariantCulture, out MobilePhone tryParsed);

        Assert.True(success);
        Assert.Equal("11987654321", parsed.Value);
        Assert.Equal(parsed, tryParsed);
    }

    [Theory]
    [InlineData("g", "11987654321")]
    [InlineData("f", "(11) 98765-4321")]
    [InlineData("e", "+5511987654321")]
    public void ToStringAcceptsCaseInsensitiveFormats(string format, string expected)
    {
        MobilePhone phone = MobilePhone.Parse("11987654321", CultureInfo.InvariantCulture);

        Assert.Equal(expected, phone.ToString(format, formatProvider: null));
    }

    [Fact]
    public void ToStringThrowsFormatExceptionForUnsupportedFormat()
    {
        MobilePhone phone = MobilePhone.Parse("11987654321", CultureInfo.InvariantCulture);

        Assert.Throws<FormatException>(() => phone.ToString("X", formatProvider: null));
    }

    [Fact]
    public void DefaultInstanceDoesNotExposeAValue()
    {
        MobilePhone phone = default;

        Assert.Throws<InvalidOperationException>(() => phone.Value);
    }
}
