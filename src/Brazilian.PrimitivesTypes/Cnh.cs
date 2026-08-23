using System.Diagnostics.CodeAnalysis;

namespace Brazilian.PrimitivesTypes;

/// <summary>
/// Represents a structurally and mathematically valid Brazilian CNH National Registration Number.
/// </summary>
/// <remarks>
/// This value object represents only the 11-digit Número do Registro Nacional defined for the Carteira Nacional de
/// Habilitação (CNH): nine base digits followed by two security check digits. It does not represent the CNH mirror
/// number (Número do Espelho), the RENACH form number, the document validation/security code, or the driver's CPF.
/// Validation is local and deterministic and does not prove that a registration exists, is active, belongs to a
/// particular driver, or has a valid administrative status.
/// </remarks>
public readonly record struct Cnh : IParsable<Cnh>, ISpanParsable<Cnh>
{
    private const int DigitCount = 11;
    private const int BaseDigitCount = 9;

    private readonly string? _value;

    private Cnh(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the canonical CNH National Registration Number containing exactly 11 ASCII digits.
    /// </summary>
    public string Value => _value ?? throw new InvalidOperationException("A default CNH instance does not contain a valid value.");

    /// <summary>
    /// Parses an 11-digit CNH National Registration Number.
    /// </summary>
    /// <param name="value">The canonical 11-digit National Registration Number.</param>
    /// <returns>A validated CNH value object.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> is not a valid CNH National Registration Number.</exception>
    public static Cnh Parse(string value)
    {
        return Parse(value, provider: null);
    }

    /// <inheritdoc />
    public static Cnh Parse(string s, IFormatProvider? provider)
    {
        if (s is null)
        {
            throw new FormatException("CNH National Registration Number must be provided as exactly 11 ASCII digits.");
        }

        return Parse(s.AsSpan(), provider);
    }

    /// <inheritdoc />
    public static Cnh Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!TryNormalize(s, out string normalized))
        {
            throw new FormatException("CNH National Registration Number must contain exactly 11 ASCII digits with valid check digits.");
        }

        return new Cnh(normalized);
    }

    /// <summary>
    /// Attempts to parse an 11-digit CNH National Registration Number.
    /// </summary>
    /// <param name="value">The canonical 11-digit National Registration Number.</param>
    /// <param name="result">When successful, contains the validated CNH.</param>
    /// <returns><see langword="true"/> when the value is valid; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string? value, out Cnh result)
    {
        return TryParse(value, provider: null, out result);
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Cnh result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Cnh result)
    {
        if (TryNormalize(s, out string normalized))
        {
            result = new Cnh(normalized);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Determines whether the supplied text is a structurally and mathematically valid CNH National Registration Number.
    /// </summary>
    /// <param name="value">The CNH National Registration Number to validate.</param>
    /// <returns><see langword="true"/> when the value contains exactly 11 ASCII digits and valid check digits.</returns>
    public static bool IsValid(string? value)
    {
        return TryParse(value, out _);
    }

    /// <summary>
    /// Returns the canonical 11-digit CNH National Registration Number.
    /// </summary>
    /// <returns>The canonical CNH National Registration Number.</returns>
    public override string ToString()
    {
        return Value;
    }

    private static bool TryNormalize(ReadOnlySpan<char> input, out string normalized)
    {
        if (input.Length != DigitCount)
        {
            normalized = string.Empty;
            return false;
        }

        Span<char> digits = stackalloc char[DigitCount];
        for (int index = 0; index < input.Length; index++)
        {
            if (!IsAsciiDigit(input[index]))
            {
                normalized = string.Empty;
                return false;
            }

            digits[index] = input[index];
        }

        if (HasRepeatedDigits(digits) || !HasValidCheckDigits(digits))
        {
            normalized = string.Empty;
            return false;
        }

        normalized = new string(digits);
        return true;
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

    private static bool HasValidCheckDigits(ReadOnlySpan<char> digits)
    {
        ReadOnlySpan<char> baseDigits = digits[..BaseDigitCount];
        (int firstCheckDigit, int discount) = CalculateFirstCheckDigit(baseDigits);
        if (digits[BaseDigitCount] - '0' != firstCheckDigit)
        {
            return false;
        }

        int secondCheckDigit = CalculateSecondCheckDigit(baseDigits, discount);
        return digits[BaseDigitCount + 1] - '0' == secondCheckDigit;
    }

    /// <summary>
    /// Calculates the first security digit using weights 9..1. A modulo-11 remainder of 10 yields DV1 zero and
    /// activates the historical inter-DV discount of two used by the second calculation.
    /// </summary>
    private static (int CheckDigit, int Discount) CalculateFirstCheckDigit(ReadOnlySpan<char> baseDigits)
    {
        int sum = 0;
        for (int index = 0; index < BaseDigitCount; index++)
        {
            int weight = BaseDigitCount - index;
            sum += (baseDigits[index] - '0') * weight;
        }

        int remainder = sum % 11;
        return remainder == 10 ? (0, 2) : (remainder, 0);
    }

    /// <summary>
    /// Calculates the second security digit using weights 1..9. When DV1 activated the discount, two is subtracted
    /// from the second modulo-11 remainder; negative results wrap within modulo 11, and a resulting value of 10 maps
    /// to zero.
    /// </summary>
    private static int CalculateSecondCheckDigit(ReadOnlySpan<char> baseDigits, int discount)
    {
        int sum = 0;
        for (int index = 0; index < BaseDigitCount; index++)
        {
            int weight = index + 1;
            sum += (baseDigits[index] - '0') * weight;
        }

        int checkDigit = sum % 11;
        if (discount != 0)
        {
            checkDigit = checkDigit - discount < 0
                ? checkDigit + 9
                : checkDigit - discount;
        }

        return checkDigit > 9 ? 0 : checkDigit;
    }

    private static bool IsAsciiDigit(char value)
    {
        return (uint)(value - '0') <= 9;
    }
}
