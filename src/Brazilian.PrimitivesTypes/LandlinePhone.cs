using System.Diagnostics.CodeAnalysis;

namespace Brazilian.PrimitivesTypes;

/// <summary>
/// Represents a Brazilian geographic landline number under Anatel's numbering plan.
/// </summary>
/// <remarks>
/// Validation is structural and numbering-plan based. A valid <see cref="LandlinePhone"/> does not prove that the
/// number exists, is active, belongs to a specific subscriber, or is currently associated with a particular carrier.
/// Non-geographic codes, mobile numbers, service codes, PABX extensions, and carrier-selection dialing are outside
/// this type's scope.
/// </remarks>
public readonly record struct LandlinePhone : IParsable<LandlinePhone>, ISpanParsable<LandlinePhone>, IFormattable
{
    private const int NationalLength = 10;
    private const int SubscriberLength = 8;
    private const int FormattedLength = 14;
    private const int E164Length = 13;
    private const int InternationalFormattedLength = 16;

    private readonly string? _value;

    private LandlinePhone(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the canonical national representation containing DDD plus the eight-digit subscriber number.
    /// </summary>
    public string Value => _value ?? throw new InvalidOperationException("A default landline phone instance does not contain a valid value.");

    /// <summary>
    /// Gets the two-digit Anatel geographic area code (DDD).
    /// </summary>
    public string AreaCode => Value[..2];

    /// <summary>
    /// Gets the eight-digit fixed-line subscriber number.
    /// </summary>
    public string SubscriberNumber => Value[2..];

    /// <summary>
    /// Gets the national display representation using the <c>(00) 0000-0000</c> pattern.
    /// </summary>
    public string Formatted => Format(Value);

    /// <summary>
    /// Gets the international E.164 representation using Brazil's country code 55.
    /// </summary>
    public string E164 => string.Concat("+55", Value);

    /// <summary>
    /// Parses a supported Brazilian geographic landline representation.
    /// </summary>
    /// <param name="value">A canonical national, national formatted, E.164, or explicitly supported +55 display representation.</param>
    /// <returns>A validated landline phone value object.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> is not structurally valid under the supported numbering-plan rules.</exception>
    public static LandlinePhone Parse(string value)
    {
        return Parse(value, provider: null);
    }

    /// <inheritdoc />
    public static LandlinePhone Parse(string s, IFormatProvider? provider)
    {
        if (s is null)
        {
            throw new FormatException("Landline phone must be provided in a supported format.");
        }

        return Parse(s.AsSpan(), provider);
    }

    /// <inheritdoc />
    public static LandlinePhone Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!TryNormalize(s, out string normalized))
        {
            throw new FormatException("Landline phone must contain a valid Brazilian DDD and an eight-digit fixed-line subscriber number in a supported format.");
        }

        return new LandlinePhone(normalized);
    }

    /// <summary>
    /// Attempts to parse a supported Brazilian geographic landline representation.
    /// </summary>
    /// <param name="value">The phone text to parse.</param>
    /// <param name="result">When successful, contains the validated landline phone.</param>
    /// <returns><see langword="true"/> when the input is valid; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string? value, out LandlinePhone result)
    {
        return TryParse(value, provider: null, out result);
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out LandlinePhone result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out LandlinePhone result)
    {
        if (TryNormalize(s, out string normalized))
        {
            result = new LandlinePhone(normalized);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Determines whether the supplied text represents a structurally valid Brazilian geographic landline number.
    /// </summary>
    /// <param name="value">The phone text to validate.</param>
    /// <returns><see langword="true"/> when the input satisfies the supported formats and Anatel numbering-plan rules.</returns>
    public static bool IsValid(string? value)
    {
        return TryParse(value, out _);
    }

    /// <summary>
    /// Returns the canonical ten-digit national representation.
    /// </summary>
    /// <returns>The canonical DDD plus subscriber number.</returns>
    public override string ToString()
    {
        return Value;
    }

    /// <summary>
    /// Formats this phone using <c>G</c> for canonical national, <c>F</c> for formatted national, or <c>E</c> for E.164.
    /// </summary>
    /// <param name="format"><c>G</c>, <c>F</c>, <c>E</c>, an empty string, or <see langword="null"/>.</param>
    /// <param name="formatProvider">Ignored because phone formatting is culture-invariant.</param>
    /// <returns>The requested representation.</returns>
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

        if (string.Equals(format, "E", StringComparison.OrdinalIgnoreCase))
        {
            return E164;
        }

        throw new FormatException($"Unsupported landline phone format '{format}'. Use 'G', 'F', or 'E'.");
    }

    private static bool TryNormalize(ReadOnlySpan<char> input, out string normalized)
    {
        Span<char> canonical = stackalloc char[NationalLength];
        if (!TryExtractCanonical(input, canonical)
            || !BrazilianAreaCode.IsValid(canonical[..2])
            || !IsLandlineSubscriber(canonical[2..]))
        {
            normalized = string.Empty;
            return false;
        }

        normalized = new string(canonical);
        return true;
    }

    private static bool TryExtractCanonical(ReadOnlySpan<char> input, Span<char> destination)
    {
        if (input.Length == NationalLength)
        {
            return TryCopyAsciiDigits(input, destination);
        }

        if (input.Length == FormattedLength)
        {
            return TryExtractNationalFormatted(input, destination);
        }

        if (input.Length == E164Length)
        {
            return TryExtractE164(input, destination);
        }

        return input.Length == InternationalFormattedLength && TryExtractInternationalFormatted(input, destination);
    }

    private static bool TryExtractNationalFormatted(ReadOnlySpan<char> input, Span<char> destination)
    {
        if (input[0] != '(' || input[3] != ')' || input[4] != ' ' || input[9] != '-')
        {
            return false;
        }

        destination[0] = input[1];
        destination[1] = input[2];
        destination[2] = input[5];
        destination[3] = input[6];
        destination[4] = input[7];
        destination[5] = input[8];
        destination[6] = input[10];
        destination[7] = input[11];
        destination[8] = input[12];
        destination[9] = input[13];
        return ContainsOnlyAsciiDigits(destination);
    }

    private static bool TryExtractE164(ReadOnlySpan<char> input, Span<char> destination)
    {
        if (input[0] != '+' || input[1] != '5' || input[2] != '5')
        {
            return false;
        }

        return TryCopyAsciiDigits(input[3..], destination);
    }

    private static bool TryExtractInternationalFormatted(ReadOnlySpan<char> input, Span<char> destination)
    {
        if (input[0] != '+' || input[1] != '5' || input[2] != '5' || input[3] != ' '
            || input[6] != ' ' || input[11] != '-')
        {
            return false;
        }

        destination[0] = input[4];
        destination[1] = input[5];
        destination[2] = input[7];
        destination[3] = input[8];
        destination[4] = input[9];
        destination[5] = input[10];
        destination[6] = input[12];
        destination[7] = input[13];
        destination[8] = input[14];
        destination[9] = input[15];
        return ContainsOnlyAsciiDigits(destination);
    }

    private static bool TryCopyAsciiDigits(ReadOnlySpan<char> source, Span<char> destination)
    {
        if (source.Length != destination.Length)
        {
            return false;
        }

        for (int index = 0; index < source.Length; index++)
        {
            if (!IsAsciiDigit(source[index]))
            {
                return false;
            }

            destination[index] = source[index];
        }

        return true;
    }

    private static bool ContainsOnlyAsciiDigits(ReadOnlySpan<char> value)
    {
        for (int index = 0; index < value.Length; index++)
        {
            if (!IsAsciiDigit(value[index]))
            {
                return false;
            }
        }

        return true;
    }

    private static bool IsLandlineSubscriber(ReadOnlySpan<char> subscriber)
    {
        if (subscriber.Length != SubscriberLength)
        {
            return false;
        }

        // Anatel identifies STFC by first digit 2, 3, 4, or 5. Prefix 57 remains fixed-line rural numbering.
        return subscriber[0] is >= '2' and <= '5';
    }

    private static bool IsAsciiDigit(char value)
    {
        return (uint)(value - '0') <= 9;
    }

    private static string Format(string value)
    {
        return string.Create(FormattedLength, value, static (destination, source) =>
        {
            destination[0] = '(';
            destination[1] = source[0];
            destination[2] = source[1];
            destination[3] = ')';
            destination[4] = ' ';
            destination[5] = source[2];
            destination[6] = source[3];
            destination[7] = source[4];
            destination[8] = source[5];
            destination[9] = '-';
            destination[10] = source[6];
            destination[11] = source[7];
            destination[12] = source[8];
            destination[13] = source[9];
        });
    }
}
