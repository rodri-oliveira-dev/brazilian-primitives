using System.Globalization;
using Brazilian.Primitives;
using Xunit;

namespace Brazilian.Primitives.Tests;

public sealed class LandlinePhoneTests
{
    [Theory]
    [InlineData("1132345678")]
    [InlineData("(11) 3234-5678")]
    [InlineData("+55 11 3234-5678")]
    [InlineData("+551132345678")]
    public void ParseNormalizesSupportedRepresentations(string input)
    {
        LandlinePhone phone = LandlinePhone.Parse(input, CultureInfo.InvariantCulture);

        Assert.Equal("1132345678", phone.Value);
        Assert.Equal("11", phone.AreaCode);
        Assert.Equal("32345678", phone.SubscriberNumber);
        Assert.Equal("(11) 3234-5678", phone.Formatted);
        Assert.Equal("+551132345678", phone.E164);
        Assert.Equal("1132345678", phone.ToString());
        Assert.Equal("1132345678", phone.ToString("G", formatProvider: null));
        Assert.Equal("(11) 3234-5678", phone.ToString("F", formatProvider: null));
        Assert.Equal("+551132345678", phone.ToString("E", formatProvider: null));
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
        string value = string.Concat(areaCode, "22345678");

        Assert.True(LandlinePhone.IsValid(value));
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
        string value = string.Concat(areaCode, "22345678");

        Assert.False(LandlinePhone.IsValid(value));
    }

    [Theory]
    [InlineData("1123456789")]
    [InlineData("1133456789")]
    [InlineData("1143456789")]
    [InlineData("1153456789")]
    public void IsValidAcceptsFixedSubscriberFirstDigits(string value)
    {
        Assert.True(LandlinePhone.IsValid(value));
    }

    [Fact]
    public void IsValidAcceptsRural57Prefix()
    {
        Assert.True(LandlinePhone.IsValid("1157123456"));
    }

    [Theory]
    [InlineData("1163456789")]
    [InlineData("1173456789")]
    [InlineData("1183456789")]
    [InlineData("1193456789")]
    [InlineData("1198765432")]
    public void IsValidRejectsNonFixedSubscriberRanges(string value)
    {
        Assert.False(LandlinePhone.IsValid(value));
    }

    [Theory]
    [InlineData("0300123456")]
    [InlineData("0500123456")]
    [InlineData("0800123456")]
    [InlineData("0900123456")]
    public void IsValidRejectsNonGeographicNumbers(string value)
    {
        Assert.False(LandlinePhone.IsValid(value));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("113234567")]
    [InlineData("11323456789")]
    [InlineData("32345678")]
    [InlineData("+54 11 3234-5678")]
    [InlineData("+541132345678")]
    [InlineData("(11)3234-5678")]
    [InlineData("11 3234-5678")]
    [InlineData("+55 (11) 3234-5678")]
    [InlineData("(11) 32345-678")]
    [InlineData("(11) 3234.5678")]
    [InlineData("abc11xyz3234-5678")]
    [InlineData("11A2345678")]
    [InlineData("１１３２３４５６７８")]
    public void TryParseReturnsFalseForInvalidInput(string? value)
    {
        bool parsed = LandlinePhone.TryParse(value, out LandlinePhone result);

        Assert.False(parsed);
        Assert.Equal(default, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("113234567")]
    [InlineData("1198765432")]
    [InlineData("+54 11 3234-5678")]
    [InlineData("abc11xyz3234-5678")]
    public void ParseThrowsFormatExceptionForInvalidInput(string value)
    {
        Assert.Throws<FormatException>(() => LandlinePhone.Parse(value, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ParseThrowsFormatExceptionForNullInput()
    {
        Assert.Throws<FormatException>(() => LandlinePhone.Parse(null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void EqualityUsesCanonicalNationalValue()
    {
        LandlinePhone raw = LandlinePhone.Parse("1132345678", CultureInfo.InvariantCulture);
        LandlinePhone formatted = LandlinePhone.Parse("(11) 3234-5678", CultureInfo.InvariantCulture);
        LandlinePhone international = LandlinePhone.Parse("+55 11 3234-5678", CultureInfo.InvariantCulture);
        LandlinePhone e164 = LandlinePhone.Parse("+551132345678", CultureInfo.InvariantCulture);

        Assert.Equal(raw, formatted);
        Assert.Equal(raw, international);
        Assert.Equal(raw, e164);
        Assert.Equal(raw.GetHashCode(), formatted.GetHashCode());
    }

    [Fact]
    public void ParseAndTryParseSupportSpanContracts()
    {
        ReadOnlySpan<char> value = "+55 11 3234-5678".AsSpan();

        LandlinePhone parsed = LandlinePhone.Parse(value, CultureInfo.InvariantCulture);
        bool success = LandlinePhone.TryParse(value, CultureInfo.InvariantCulture, out LandlinePhone tryParsed);

        Assert.True(success);
        Assert.Equal("1132345678", parsed.Value);
        Assert.Equal(parsed, tryParsed);
    }

    [Fact]
    public void ToStringThrowsFormatExceptionForUnsupportedFormat()
    {
        LandlinePhone phone = LandlinePhone.Parse("1132345678", CultureInfo.InvariantCulture);

        Assert.Throws<FormatException>(() => phone.ToString("X", formatProvider: null));
    }

    [Fact]
    public void DefaultInstanceDoesNotExposeAValue()
    {
        LandlinePhone phone = default;

        Assert.Throws<InvalidOperationException>(() => phone.Value);
    }
}
