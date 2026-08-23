using System.Globalization;
using Brazilian.PrimitivesTypes;
using Xunit;

namespace Brazilian.PrimitivesTypes.Tests;

public sealed class CnhTests
{
    [Theory]
    [InlineData("62472927637")]
    [InlineData("69044271146")]
    [InlineData("02650306461")]
    [InlineData("04397322870")]
    [InlineData("04375701302")]
    [InlineData("02996843266")]
    [InlineData("04375700501")]
    public void IsValidReturnsTrueForIndependentKnownVectors(string value)
    {
        Assert.True(Cnh.IsValid(value));
    }

    [Theory]
    [InlineData("00000001801")]
    [InlineData("00000009309")]
    [InlineData("00000018200")]
    public void IsValidExercisesInterCheckDigitDiscountCases(string value)
    {
        Assert.True(Cnh.IsValid(value));
    }

    [Theory]
    [InlineData("00000000000")]
    [InlineData("11111111111")]
    [InlineData("22222222222")]
    [InlineData("33333333333")]
    [InlineData("44444444444")]
    [InlineData("55555555555")]
    [InlineData("66666666666")]
    [InlineData("77777777777")]
    [InlineData("88888888888")]
    [InlineData("99999999999")]
    public void IsValidReturnsFalseForRepeatedDigits(string value)
    {
        Assert.False(Cnh.IsValid(value));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("6247292763")]
    [InlineData("624729276370")]
    [InlineData("62472927647")]
    [InlineData("62472927638")]
    [InlineData("62472927A37")]
    [InlineData("624.729.276-37")]
    [InlineData("624 729 276 37")]
    [InlineData(" 62472927637")]
    [InlineData("62472927637 ")]
    [InlineData("６２４７２９２７６３７")]
    public void TryParseReturnsFalseForInvalidOrNonCanonicalInput(string? value)
    {
        bool parsed = Cnh.TryParse(value, out Cnh result);

        Assert.False(parsed);
        Assert.Equal(default, result);
    }

    [Theory]
    [InlineData("")]
    [InlineData("6247292763")]
    [InlineData("62472927647")]
    [InlineData("62472927638")]
    [InlineData("624.729.276-37")]
    public void ParseThrowsFormatExceptionForInvalidInput(string value)
    {
        Assert.Throws<FormatException>(() => Cnh.Parse(value, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ParseThrowsFormatExceptionForNullInput()
    {
        Assert.Throws<FormatException>(() => Cnh.Parse(null!, CultureInfo.InvariantCulture));
    }

    [Fact]
    public void ParsePreservesLeadingZerosAndUsesCanonicalDisplay()
    {
        Cnh cnh = Cnh.Parse("02650306461", CultureInfo.InvariantCulture);

        Assert.Equal("02650306461", cnh.Value);
        Assert.Equal("02650306461", cnh.ToString());
    }

    [Fact]
    public void EqualityAndHashingUseCanonicalValue()
    {
        Cnh first = Cnh.Parse("62472927637", CultureInfo.InvariantCulture);
        Cnh second = Cnh.Parse("62472927637", CultureInfo.InvariantCulture);
        Cnh other = Cnh.Parse("69044271146", CultureInfo.InvariantCulture);

        Assert.Equal(first, second);
        Assert.Equal(first.GetHashCode(), second.GetHashCode());
        Assert.NotEqual(first, other);
    }

    [Fact]
    public void ParseAndTryParseSupportSpanContracts()
    {
        ReadOnlySpan<char> value = "62472927637".AsSpan();

        Cnh parsed = Cnh.Parse(value, CultureInfo.InvariantCulture);
        bool success = Cnh.TryParse(value, CultureInfo.InvariantCulture, out Cnh tryParsed);

        Assert.True(success);
        Assert.Equal("62472927637", parsed.Value);
        Assert.Equal(parsed, tryParsed);
    }

    [Fact]
    public void ChangingEitherCheckDigitMakesKnownVectorInvalid()
    {
        const string valid = "62472927637";

        for (int checkDigitIndex = 9; checkDigitIndex <= 10; checkDigitIndex++)
        {
            for (char replacement = '0'; replacement <= '9'; replacement++)
            {
                if (replacement == valid[checkDigitIndex])
                {
                    continue;
                }

                char[] mutated = valid.ToCharArray();
                mutated[checkDigitIndex] = replacement;

                Assert.False(Cnh.IsValid(new string(mutated)));
            }
        }
    }

    [Fact]
    public void DefaultValueDoesNotExposeAnInvalidRegistration()
    {
        Cnh cnh = default;

        Assert.Throws<InvalidOperationException>(() => cnh.Value);
    }
}
