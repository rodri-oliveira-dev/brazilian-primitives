using System.Diagnostics.CodeAnalysis;

namespace Brazilian.PrimitivesTypes;

/// <summary>
/// Represents the structural form of a Brazilian Código de Endereçamento Postal (CEP).
/// </summary>
/// <remarks>
/// Validation is local and structural. A valid <see cref="Cep"/> contains exactly eight ASCII digits, optionally
/// supplied with the canonical <c>00000-000</c> mask. Structural validity does not prove that the CEP is currently
/// assigned by Correios, exists in the Diretório Nacional de Endereços (DNE), or belongs to a specific address.
/// </remarks>
public readonly record struct Cep : IParsable<Cep>, ISpanParsable<Cep>, IFormattable
{
    private const int DigitCount = 8;
    private const int FormattedLength = 9;

    private readonly string? _value;

    private Cep(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the canonical CEP representation containing exactly eight ASCII digits and no mask.
    /// </summary>
    public string Value => _value ?? throw new InvalidOperationException("A default CEP instance does not contain a valid value.");

    /// <summary>
    /// Gets the CEP formatted with the canonical <c>00000-000</c> mask.
    /// </summary>
    public string Formatted => Format(Value);

    /// <summary>
    /// Parses an unmasked or canonically masked CEP.
    /// </summary>
    /// <param name="value">The CEP in <c>00000000</c> or <c>00000-000</c> format.</param>
    /// <returns>A structurally valid CEP value object.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> is not in a supported CEP format.</exception>
    public static Cep Parse(string value)
    {
        return Parse(value, provider: null);
    }

    /// <inheritdoc />
    public static Cep Parse(string s, IFormatProvider? provider)
    {
        if (s is null)
        {
            throw new FormatException("CEP must be provided in a supported format.");
        }

        return Parse(s.AsSpan(), provider);
    }

    /// <inheritdoc />
    public static Cep Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!TryNormalize(s, out string normalized))
        {
            throw new FormatException("CEP must contain exactly eight ASCII digits, optionally using the canonical 00000-000 mask.");
        }

        return new Cep(normalized);
    }

    /// <summary>
    /// Attempts to parse an unmasked or canonically masked CEP.
    /// </summary>
    /// <param name="value">The CEP in <c>00000000</c> or <c>00000-000</c> format.</param>
    /// <param name="result">When this method returns <see langword="true"/>, contains the structurally valid CEP.</param>
    /// <returns><see langword="true"/> when the value has a supported CEP structure; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string? value, out Cep result)
    {
        return TryParse(value, provider: null, out result);
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Cep result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Cep result)
    {
        if (TryNormalize(s, out string normalized))
        {
            result = new Cep(normalized);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Determines whether the supplied text has a supported CEP structure.
    /// </summary>
    /// <param name="value">The CEP text to validate.</param>
    /// <returns>
    /// <see langword="true"/> when the text contains exactly eight ASCII digits in a supported representation;
    /// otherwise, <see langword="false"/>. This result does not imply existence in the Correios DNE.
    /// </returns>
    public static bool IsValid(string? value)
    {
        return TryParse(value, out _);
    }

    /// <summary>
    /// Returns the canonical eight-digit representation of this CEP.
    /// </summary>
    /// <returns>The canonical CEP value.</returns>
    public override string ToString()
    {
        return Value;
    }

    /// <summary>
    /// Formats this CEP using <c>G</c> for the canonical unmasked representation or <c>F</c> for the canonical mask.
    /// </summary>
    /// <param name="format"><c>G</c>, <c>F</c>, an empty string, or <see langword="null"/>.</param>
    /// <param name="formatProvider">Ignored because CEP formatting is culture-invariant.</param>
    /// <returns>The requested CEP representation.</returns>
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

        throw new FormatException($"Unsupported CEP format '{format}'. Use 'G' or 'F'.");
    }

    private static bool TryNormalize(ReadOnlySpan<char> input, out string normalized)
    {
        Span<char> digits = stackalloc char[DigitCount];

        if (!TryExtractDigits(input, digits))
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

        if (input.Length != FormattedLength || input[5] != '-')
        {
            return false;
        }

        int digitIndex = 0;
        for (int sourceIndex = 0; sourceIndex < FormattedLength; sourceIndex++)
        {
            if (sourceIndex == 5)
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
            destination[3] = source[3];
            destination[4] = source[4];
            destination[5] = '-';
            destination[6] = source[5];
            destination[7] = source[6];
            destination[8] = source[7];
        });
    }
}
