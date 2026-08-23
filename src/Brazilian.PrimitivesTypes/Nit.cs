using System.Diagnostics.CodeAnalysis;

namespace Brazilian.PrimitivesTypes;

/// <summary>
/// Represents a Brazilian NIT (Numero de Identificacao do Trabalhador) in the supported structural contract.
/// </summary>
/// <remarks>
/// This implementation validates the official 11-ASCII-digit structure only. It intentionally does not invent a
/// check-digit algorithm for NIT without an authoritative source describing weights, modulo, and remainder handling.
/// A valid <see cref="Nit"/> does not prove CNIS existence, social-security affiliation, ownership, contributions,
/// benefit rights, or active cadastral status.
/// </remarks>
public readonly record struct Nit : IParsable<Nit>, ISpanParsable<Nit>
{
    private const int DigitCount = 11;

    private readonly string? _value;

    private Nit(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the canonical 11-digit NIT value.
    /// </summary>
    public string Value => _value ?? throw new InvalidOperationException("A default NIT instance does not contain a valid value.");

    /// <summary>
    /// Parses an 11-digit NIT value.
    /// </summary>
    /// <param name="value">The NIT text.</param>
    /// <returns>A validated NIT value object.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> does not match the supported NIT structure.</exception>
    public static Nit Parse(string value)
    {
        return Parse(value, provider: null);
    }

    /// <inheritdoc />
    public static Nit Parse(string s, IFormatProvider? provider)
    {
        if (s is null)
        {
            throw new FormatException("NIT must be provided as exactly 11 ASCII digits.");
        }

        return Parse(s.AsSpan(), provider);
    }

    /// <inheritdoc />
    public static Nit Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!TryNormalize(s, out string normalized))
        {
            throw new FormatException("NIT must contain exactly 11 ASCII digits.");
        }

        return new Nit(normalized);
    }

    /// <summary>
    /// Attempts to parse an 11-digit NIT value.
    /// </summary>
    /// <param name="value">The NIT text.</param>
    /// <param name="result">When successful, contains the NIT.</param>
    /// <returns><see langword="true"/> when the value matches the supported structure.</returns>
    public static bool TryParse(string? value, out Nit result)
    {
        return TryParse(value, provider: null, out result);
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Nit result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Nit result)
    {
        if (TryNormalize(s, out string normalized))
        {
            result = new Nit(normalized);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Determines whether the supplied text matches the supported NIT structure.
    /// </summary>
    /// <param name="value">The NIT text.</param>
    /// <returns><see langword="true"/> when the value contains exactly 11 ASCII digits.</returns>
    public static bool IsValid(string? value)
    {
        return TryParse(value, out _);
    }

    /// <summary>
    /// Returns the canonical 11-digit NIT value.
    /// </summary>
    /// <returns>The canonical NIT value.</returns>
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
