using System.Diagnostics.CodeAnalysis;

namespace Brazilian.Primitives;

/// <summary>
/// Represents a structurally and mathematically valid PIS/PASEP registration number.
/// </summary>
/// <remarks>
/// Validation is local and deterministic. A valid <see cref="PisPasep"/> does not prove registration existence,
/// ownership, labor relationship, CNIS linkage, benefit eligibility, or status before Caixa or Banco do Brasil.
/// </remarks>
public readonly record struct PisPasep : IParsable<PisPasep>, ISpanParsable<PisPasep>
{
    private const int DigitCount = 11;
    private const int BaseDigitCount = 10;

    private readonly string? _value;

    private PisPasep(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the canonical 11-digit PIS/PASEP value.
    /// </summary>
    public string Value => _value ?? throw new InvalidOperationException("A default PIS/PASEP instance does not contain a valid value.");

    /// <summary>
    /// Parses an 11-digit PIS/PASEP value.
    /// </summary>
    /// <param name="value">The PIS/PASEP text.</param>
    /// <returns>A validated PIS/PASEP value object.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> is not a valid PIS/PASEP value.</exception>
    public static PisPasep Parse(string value)
    {
        return Parse(value, provider: null);
    }

    /// <inheritdoc />
    public static PisPasep Parse(string s, IFormatProvider? provider)
    {
        if (s is null)
        {
            throw new FormatException("PIS/PASEP must be provided as exactly 11 ASCII digits.");
        }

        return Parse(s.AsSpan(), provider);
    }

    /// <inheritdoc />
    public static PisPasep Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!TryNormalize(s, out string normalized))
        {
            throw new FormatException("PIS/PASEP must contain exactly 11 ASCII digits with a valid check digit.");
        }

        return new PisPasep(normalized);
    }

    /// <summary>
    /// Attempts to parse an 11-digit PIS/PASEP value.
    /// </summary>
    /// <param name="value">The PIS/PASEP text.</param>
    /// <param name="result">When successful, contains the PIS/PASEP value.</param>
    /// <returns><see langword="true"/> when the value is valid.</returns>
    public static bool TryParse(string? value, out PisPasep result)
    {
        return TryParse(value, provider: null, out result);
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out PisPasep result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out PisPasep result)
    {
        if (TryNormalize(s, out string normalized))
        {
            result = new PisPasep(normalized);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Determines whether the supplied text is a structurally and mathematically valid PIS/PASEP value.
    /// </summary>
    /// <param name="value">The PIS/PASEP text.</param>
    /// <returns><see langword="true"/> when the value contains 11 ASCII digits and a valid check digit.</returns>
    public static bool IsValid(string? value)
    {
        return TryParse(value, out _);
    }

    /// <summary>
    /// Returns the canonical 11-digit PIS/PASEP value.
    /// </summary>
    /// <returns>The canonical PIS/PASEP value.</returns>
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
        for (int index = 0; index < input.Length; index++)
        {
            if (!IsAsciiDigit(input[index]))
            {
                return false;
            }

            digits[index] = input[index];
        }

        if (HasRepeatedDigits(digits) || !HasValidCheckDigit(digits))
        {
            return false;
        }

        normalized = new string(digits);
        return true;
    }

    private static bool HasValidCheckDigit(ReadOnlySpan<char> digits)
    {
        return digits[BaseDigitCount] - '0' == CalculateCheckDigit(digits[..BaseDigitCount]);
    }

    private static int CalculateCheckDigit(ReadOnlySpan<char> baseDigits)
    {
        ReadOnlySpan<int> weights = [3, 2, 9, 8, 7, 6, 5, 4, 3, 2];

        int sum = 0;
        for (int index = 0; index < BaseDigitCount; index++)
        {
            sum += (baseDigits[index] - '0') * weights[index];
        }

        int remainder = sum % 11;
        int checkDigit = 11 - remainder;
        return checkDigit >= 10 ? 0 : checkDigit;
    }

    private static bool HasRepeatedDigits(ReadOnlySpan<char> digits)
    {
        char first = digits[0];
        for (int index = 1; index < digits.Length; index++)
        {
            if (digits[index] != first)
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsAsciiDigit(char value)
    {
        return (uint)(value - '0') <= 9;
    }
}
