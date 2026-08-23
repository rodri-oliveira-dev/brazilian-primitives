using System.Diagnostics.CodeAnalysis;

namespace Brazilian.Primitives;

/// <summary>
/// Represents a structurally and mathematically valid Brazilian Cadastro de Pessoas Físicas (CPF) number.
/// </summary>
/// <remarks>
/// Validation is local and deterministic. A valid <see cref="Cpf"/> does not prove that the number exists,
/// is registered with Receita Federal, belongs to a specific person, or has a particular cadastral status.
/// </remarks>
public readonly record struct Cpf : IParsable<Cpf>, ISpanParsable<Cpf>, IFormattable
{
    private const int DigitCount = 11;
    private const int FormattedLength = 14;

    private readonly string? _value;

    private Cpf(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the canonical CPF representation containing exactly 11 ASCII digits and no mask.
    /// </summary>
    public string Value => _value ?? throw new InvalidOperationException("A default CPF instance does not contain a valid value.");

    /// <summary>
    /// Gets the CPF formatted with the canonical <c>000.000.000-00</c> mask.
    /// </summary>
    public string Formatted => Format(Value);

    /// <summary>
    /// Parses an unmasked or canonically masked CPF.
    /// </summary>
    /// <param name="value">The CPF in <c>00000000000</c> or <c>000.000.000-00</c> format.</param>
    /// <returns>A validated CPF value object.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> is not a valid CPF.</exception>
    public static Cpf Parse(string value)
    {
        return Parse(value, provider: null);
    }

    /// <inheritdoc />
    public static Cpf Parse(string s, IFormatProvider? provider)
    {
        if (s is null)
        {
            throw new FormatException("CPF must be provided in a supported format.");
        }

        return Parse(s.AsSpan(), provider);
    }

    /// <inheritdoc />
    public static Cpf Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!TryNormalize(s, out string normalized))
        {
            throw new FormatException("CPF must contain 11 valid digits, optionally using the canonical 000.000.000-00 mask.");
        }

        return new Cpf(normalized);
    }

    /// <summary>
    /// Attempts to parse an unmasked or canonically masked CPF.
    /// </summary>
    /// <param name="value">The CPF in <c>00000000000</c> or <c>000.000.000-00</c> format.</param>
    /// <param name="result">When this method returns <see langword="true"/>, contains the validated CPF.</param>
    /// <returns><see langword="true"/> when the value is a valid CPF; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string? value, out Cpf result)
    {
        return TryParse(value, provider: null, out result);
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Cpf result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Cpf result)
    {
        if (TryNormalize(s, out string normalized))
        {
            result = new Cpf(normalized);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Determines whether the supplied text has a supported CPF representation and valid verification digits.
    /// </summary>
    /// <param name="value">The CPF text to validate.</param>
    /// <returns><see langword="true"/> when the text represents a structurally and mathematically valid CPF.</returns>
    public static bool IsValid(string? value)
    {
        return TryParse(value, out _);
    }

    /// <summary>
    /// Returns the canonical 11-digit representation of this CPF.
    /// </summary>
    /// <returns>The canonical CPF value.</returns>
    public override string ToString()
    {
        return Value;
    }

    /// <summary>
    /// Formats this CPF using <c>G</c> for the canonical unmasked representation or <c>F</c> for the canonical mask.
    /// </summary>
    /// <param name="format"><c>G</c>, <c>F</c>, an empty string, or <see langword="null"/>.</param>
    /// <param name="formatProvider">Ignored because CPF formatting is culture-invariant.</param>
    /// <returns>The requested CPF representation.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="format"/> is unsupported.</exception>
    public string ToString(string? format, IFormatProvider? formatProvider)
    {
        if (string.IsNullOrEmpty(format) || string.Equals(format, "G", StringComparison.OrdinalIgnoreCase))
        {
            return Value;
        }

        if (string.Equals(format, "F", StringComparison.OrdinalIgnoreCase))
        {
            return Formatted;
        }

        throw new FormatException($"Unsupported CPF format '{format}'. Use 'G' or 'F'.");
    }

    private static bool TryNormalize(ReadOnlySpan<char> input, out string normalized)
    {
        Span<char> digits = stackalloc char[DigitCount];

        if (!TryExtractDigits(input, digits) || HasRepeatedDigits(digits) || !HasValidCheckDigits(digits))
        {
            normalized = string.Empty;
            return false;
        }

        normalized = new string(digits);
        return true;
    }

    private static bool TryExtractDigits(ReadOnlySpan<char> input, Span<char> digits)
    {
        if (input.Length == DigitCount)
        {
            for (int index = 0; index < DigitCount; index++)
            {
                char character = input[index];
                if (!IsAsciiDigit(character))
                {
                    return false;
                }

                digits[index] = character;
            }

            return true;
        }

        if (input.Length != FormattedLength || input[3] != '.' || input[7] != '.' || input[11] != '-')
        {
            return false;
        }

        int digitIndex = 0;
        for (int sourceIndex = 0; sourceIndex < FormattedLength; sourceIndex++)
        {
            if (sourceIndex is 3 or 7 or 11)
            {
                continue;
            }

            char character = input[sourceIndex];
            if (!IsAsciiDigit(character))
            {
                return false;
            }

            digits[digitIndex++] = character;
        }

        return digitIndex == DigitCount;
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
        int firstCheckDigit = CalculateCheckDigit(digits[..9]);
        if (digits[9] - '0' != firstCheckDigit)
        {
            return false;
        }

        int secondCheckDigit = CalculateCheckDigit(digits[..10]);
        return digits[10] - '0' == secondCheckDigit;
    }

    private static int CalculateCheckDigit(ReadOnlySpan<char> digits)
    {
        int sum = 0;
        for (int index = 0; index < digits.Length; index++)
        {
            int weight = digits.Length + 1 - index;
            sum += (digits[index] - '0') * weight;
        }

        int remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }

    private static bool IsAsciiDigit(char value)
    {
        return (uint)(value - '0') <= 9;
    }

    private static string Format(string value)
    {
        return string.Create(FormattedLength, value, static (destination, source) =>
        {
            destination[0] = source[0];
            destination[1] = source[1];
            destination[2] = source[2];
            destination[3] = '.';
            destination[4] = source[3];
            destination[5] = source[4];
            destination[6] = source[5];
            destination[7] = '.';
            destination[8] = source[6];
            destination[9] = source[7];
            destination[10] = source[8];
            destination[11] = '-';
            destination[12] = source[9];
            destination[13] = source[10];
        });
    }
}
