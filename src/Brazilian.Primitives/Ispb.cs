using System.Diagnostics.CodeAnalysis;

namespace Brazilian.Primitives;

/// <summary>
/// Represents an ISPB identifier for participants of the Brazilian Payments System/STR.
/// </summary>
/// <remarks>
/// Validation is structural only. A valid <see cref="Ispb"/> does not prove that the participant exists, is authorized,
/// currently participates in STR/Pix/COMPE, has an active settlement account, or has any specific CNPJ/status.
/// </remarks>
public readonly record struct Ispb : IParsable<Ispb>, ISpanParsable<Ispb>
{
    private const int DigitCount = 8;

    private readonly string? _value;

    private Ispb(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the canonical 8-digit ISPB value.
    /// </summary>
    public string Value => _value ?? throw new InvalidOperationException("A default ISPB instance does not contain a valid value.");

    /// <summary>
    /// Parses an 8-digit ISPB value.
    /// </summary>
    /// <param name="value">The ISPB text.</param>
    /// <returns>A validated ISPB value object.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> is not structurally valid.</exception>
    public static Ispb Parse(string value)
    {
        return Parse(value, provider: null);
    }

    /// <inheritdoc />
    public static Ispb Parse(string s, IFormatProvider? provider)
    {
        if (s is null)
        {
            throw new FormatException("ISPB must be provided as exactly 8 ASCII digits.");
        }

        return Parse(s.AsSpan(), provider);
    }

    /// <inheritdoc />
    public static Ispb Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!TryNormalize(s, out string normalized))
        {
            throw new FormatException("ISPB must contain exactly 8 ASCII digits.");
        }

        return new Ispb(normalized);
    }

    /// <summary>
    /// Attempts to parse an 8-digit ISPB value.
    /// </summary>
    /// <param name="value">The ISPB text.</param>
    /// <param name="result">When successful, contains the ISPB.</param>
    /// <returns><see langword="true"/> when the value has the supported ISPB structure.</returns>
    public static bool TryParse(string? value, out Ispb result)
    {
        return TryParse(value, provider: null, out result);
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Ispb result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Ispb result)
    {
        if (TryNormalize(s, out string normalized))
        {
            result = new Ispb(normalized);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Determines whether the supplied text matches the supported ISPB structure.
    /// </summary>
    /// <param name="value">The ISPB text.</param>
    /// <returns><see langword="true"/> when the value contains exactly 8 ASCII digits.</returns>
    public static bool IsValid(string? value)
    {
        return TryParse(value, out _);
    }

    /// <summary>
    /// Returns the canonical 8-digit ISPB value.
    /// </summary>
    /// <returns>The canonical ISPB value.</returns>
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

        normalized = new string(digits);
        return true;
    }

    private static bool IsAsciiDigit(char value)
    {
        return (uint)(value - '0') <= 9;
    }
}
