using System.Diagnostics.CodeAnalysis;

namespace Brazilian.PrimitivesTypes;

/// <summary>
/// Represents a structurally and mathematically valid RENAVAM vehicle registry code.
/// </summary>
/// <remarks>
/// Validation is local and deterministic. A valid <see cref="Renavam"/> does not prove that a vehicle exists, is
/// licensed, has no restrictions, belongs to a person, or is associated with a specific plate or chassis.
/// </remarks>
public readonly record struct Renavam : IParsable<Renavam>, ISpanParsable<Renavam>
{
    private const int DigitCount = 11;
    private const int BaseDigitCount = 10;

    private readonly string? _value;

    private Renavam(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the canonical 11-digit RENAVAM code.
    /// </summary>
    public string Value => _value ?? throw new InvalidOperationException("A default RENAVAM instance does not contain a valid value.");

    /// <summary>
    /// Parses an 11-digit RENAVAM code.
    /// </summary>
    /// <param name="value">The RENAVAM code.</param>
    /// <returns>A validated RENAVAM value object.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> is not a valid RENAVAM code.</exception>
    public static Renavam Parse(string value)
    {
        return Parse(value, provider: null);
    }

    /// <inheritdoc />
    public static Renavam Parse(string s, IFormatProvider? provider)
    {
        if (s is null)
        {
            throw new FormatException("RENAVAM must be provided as exactly 11 ASCII digits.");
        }

        return Parse(s.AsSpan(), provider);
    }

    /// <inheritdoc />
    public static Renavam Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!TryNormalize(s, out string normalized))
        {
            throw new FormatException("RENAVAM must contain exactly 11 ASCII digits with a valid check digit.");
        }

        return new Renavam(normalized);
    }

    /// <summary>
    /// Attempts to parse an 11-digit RENAVAM code.
    /// </summary>
    /// <param name="value">The RENAVAM code.</param>
    /// <param name="result">When successful, contains the validated RENAVAM.</param>
    /// <returns><see langword="true"/> when the value is valid; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string? value, out Renavam result)
    {
        return TryParse(value, provider: null, out result);
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Renavam result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Renavam result)
    {
        if (TryNormalize(s, out string normalized))
        {
            result = new Renavam(normalized);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Determines whether the supplied text is a structurally and mathematically valid RENAVAM code.
    /// </summary>
    /// <param name="value">The RENAVAM text to validate.</param>
    /// <returns><see langword="true"/> when the text represents a valid RENAVAM code.</returns>
    public static bool IsValid(string? value)
    {
        return TryParse(value, out _);
    }

    /// <summary>
    /// Returns the canonical 11-digit RENAVAM code.
    /// </summary>
    /// <returns>The canonical RENAVAM code.</returns>
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

        int checkDigit = (sum * 10) % 11;
        return checkDigit == 10 ? 0 : checkDigit;
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
