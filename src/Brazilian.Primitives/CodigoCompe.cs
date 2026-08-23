using System.Diagnostics.CodeAnalysis;

namespace Brazilian.Primitives;

/// <summary>
/// Represents a Brazilian COMPE financial-institution code in the current supported numeric contract.
/// </summary>
/// <remarks>
/// Validation is structural only and excludes known absence sentinels such as <c>999</c>. A valid
/// <see cref="CodigoCompe"/> does not prove code assignment, institution existence, current COMPE participation,
/// account validity, association with an ISPB, or operational status.
/// </remarks>
public readonly record struct CodigoCompe : IParsable<CodigoCompe>, ISpanParsable<CodigoCompe>
{
    private const int DigitCount = 3;

    private readonly string? _value;

    private CodigoCompe(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the canonical 3-digit COMPE code.
    /// </summary>
    public string Value => _value ?? throw new InvalidOperationException("A default COMPE code instance does not contain a valid value.");

    /// <summary>
    /// Parses a 3-digit COMPE code.
    /// </summary>
    /// <param name="value">The COMPE code text.</param>
    /// <returns>A validated COMPE code.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> is not structurally valid or is a known sentinel.</exception>
    public static CodigoCompe Parse(string value)
    {
        return Parse(value, provider: null);
    }

    /// <inheritdoc />
    public static CodigoCompe Parse(string s, IFormatProvider? provider)
    {
        if (s is null)
        {
            throw new FormatException("COMPE code must be provided as exactly 3 ASCII digits.");
        }

        return Parse(s.AsSpan(), provider);
    }

    /// <inheritdoc />
    public static CodigoCompe Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!TryNormalize(s, out string normalized))
        {
            throw new FormatException("COMPE code must contain exactly 3 ASCII digits and must not be a known absence sentinel.");
        }

        return new CodigoCompe(normalized);
    }

    /// <summary>
    /// Attempts to parse a 3-digit COMPE code.
    /// </summary>
    /// <param name="value">The COMPE code text.</param>
    /// <param name="result">When successful, contains the COMPE code.</param>
    /// <returns><see langword="true"/> when the value has the supported structure and is not a known sentinel.</returns>
    public static bool TryParse(string? value, out CodigoCompe result)
    {
        return TryParse(value, provider: null, out result);
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out CodigoCompe result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out CodigoCompe result)
    {
        if (TryNormalize(s, out string normalized))
        {
            result = new CodigoCompe(normalized);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Determines whether the supplied text matches the supported COMPE code structure and is not a known sentinel.
    /// </summary>
    /// <param name="value">The COMPE code text.</param>
    /// <returns><see langword="true"/> when the value can be represented as a COMPE code.</returns>
    public static bool IsValid(string? value)
    {
        return TryParse(value, out _);
    }

    /// <summary>
    /// Returns the canonical 3-digit COMPE code.
    /// </summary>
    /// <returns>The canonical COMPE code.</returns>
    public override string ToString()
    {
        return Value;
    }

    private static bool TryNormalize(ReadOnlySpan<char> input, out string normalized)
    {
        normalized = string.Empty;
        if (input.Length != DigitCount || input.SequenceEqual("999".AsSpan()))
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

        normalized = new string(digits);
        return true;
    }

    private static bool IsAsciiDigit(char value)
    {
        return (uint)(value - '0') <= 9;
    }
}
