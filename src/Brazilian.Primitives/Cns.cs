using System.Diagnostics.CodeAnalysis;

namespace Brazilian.Primitives;

/// <summary>
/// Represents a structurally and mathematically valid Brazilian Cartao Nacional de Saude (CNS).
/// </summary>
/// <remarks>
/// Validation is local and deterministic for the supported CNS families starting with 1, 2, 7, 8, or 9. A valid
/// <see cref="Cns"/> does not prove CADSUS existence, ownership, whether it is the citizen's main CNS, duplicate
/// linkage, cadastral quality, CPF linkage, or entitlement to care or benefits.
/// </remarks>
public readonly record struct Cns : IParsable<Cns>, ISpanParsable<Cns>
{
    private const int DigitCount = 15;

    private readonly string? _value;

    private Cns(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the canonical 15-digit CNS value.
    /// </summary>
    public string Value => _value ?? throw new InvalidOperationException("A default CNS instance does not contain a valid value.");

    /// <summary>
    /// Parses a 15-digit CNS value.
    /// </summary>
    /// <param name="value">The CNS text.</param>
    /// <returns>A validated CNS value object.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> is not a valid CNS.</exception>
    public static Cns Parse(string value)
    {
        return Parse(value, provider: null);
    }

    /// <inheritdoc />
    public static Cns Parse(string s, IFormatProvider? provider)
    {
        if (s is null)
        {
            throw new FormatException("CNS must be provided as exactly 15 ASCII digits.");
        }

        return Parse(s.AsSpan(), provider);
    }

    /// <inheritdoc />
    public static Cns Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!TryNormalize(s, out string normalized))
        {
            throw new FormatException("CNS must contain exactly 15 ASCII digits and satisfy the supported family algorithm.");
        }

        return new Cns(normalized);
    }

    /// <summary>
    /// Attempts to parse a 15-digit CNS value.
    /// </summary>
    /// <param name="value">The CNS text.</param>
    /// <param name="result">When successful, contains the CNS.</param>
    /// <returns><see langword="true"/> when the value is valid.</returns>
    public static bool TryParse(string? value, out Cns result)
    {
        return TryParse(value, provider: null, out result);
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Cns result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Cns result)
    {
        if (TryNormalize(s, out string normalized))
        {
            result = new Cns(normalized);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Determines whether the supplied text is a structurally and mathematically valid CNS.
    /// </summary>
    /// <param name="value">The CNS text.</param>
    /// <returns><see langword="true"/> when the value is valid.</returns>
    public static bool IsValid(string? value)
    {
        return TryParse(value, out _);
    }

    /// <summary>
    /// Returns the canonical 15-digit CNS value.
    /// </summary>
    /// <returns>The canonical CNS value.</returns>
    public override string ToString()
    {
        return Value;
    }

    private static bool TryNormalize(ReadOnlySpan<char> input, out string normalized)
    {
        normalized = string.Empty;
        if (input.Length != DigitCount)
        {
            return false;
        }

        Span<char> digits = stackalloc char[DigitCount];
        bool allZero = true;
        for (int index = 0; index < input.Length; index++)
        {
            if (!IsAsciiDigit(input[index]))
            {
                return false;
            }

            digits[index] = input[index];
            allZero &= input[index] == '0';
        }

        if (allZero || !HasValidFamilyAlgorithm(digits))
        {
            return false;
        }

        normalized = new string(digits);
        return true;
    }

    private static bool HasValidFamilyAlgorithm(ReadOnlySpan<char> digits)
    {
        return digits[0] switch
        {
            '1' or '2' => HasValidBeneficiaryCns(digits),
            '7' or '8' or '9' => HasValidProvisionalCns(digits),
            _ => false,
        };
    }

    private static bool HasValidBeneficiaryCns(ReadOnlySpan<char> digits)
    {
        int sum = 0;
        for (int index = 0; index < 11; index++)
        {
            sum += (digits[index] - '0') * (15 - index);
        }

        int remainder = sum % 11;
        int checkDigit = 11 - remainder;
        Span<char> expected = stackalloc char[DigitCount];
        digits[..11].CopyTo(expected);

        if (checkDigit == 11)
        {
            expected[11] = '0';
            expected[12] = '0';
            expected[13] = '0';
            expected[14] = '0';
        }
        else if (checkDigit == 10)
        {
            sum += 2;
            remainder = sum % 11;
            checkDigit = 11 - remainder;
            expected[11] = '0';
            expected[12] = '0';
            expected[13] = '1';
            expected[14] = (char)('0' + checkDigit);
        }
        else
        {
            expected[11] = '0';
            expected[12] = '0';
            expected[13] = '0';
            expected[14] = (char)('0' + checkDigit);
        }

        return digits.SequenceEqual(expected);
    }

    private static bool HasValidProvisionalCns(ReadOnlySpan<char> digits)
    {
        int sum = 0;
        for (int index = 0; index < DigitCount; index++)
        {
            sum += (digits[index] - '0') * (15 - index);
        }

        return sum % 11 == 0;
    }

    private static bool IsAsciiDigit(char value)
    {
        return (uint)(value - '0') <= 9;
    }
}
