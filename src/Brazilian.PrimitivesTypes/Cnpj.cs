using System.Diagnostics.CodeAnalysis;

namespace Brazilian.PrimitivesTypes;

/// <summary>
/// Represents a structurally and mathematically valid Brazilian Cadastro Nacional da Pessoa Jurídica (CNPJ) number.
/// </summary>
/// <remarks>
/// Since 2026, numeric and alphanumeric CNPJ representations coexist. The first 12 canonical positions may contain
/// ASCII digits or letters, while the final two verification digits remain numeric. Validation is local and
/// deterministic. A valid <see cref="Cnpj"/> does not prove that the registration exists, is active at Receita
/// Federal, belongs to a specific legal entity, or has a particular cadastral status.
/// </remarks>
public readonly record struct Cnpj : IParsable<Cnpj>, ISpanParsable<Cnpj>, IFormattable
{
    private readonly string? _value;

    private Cnpj(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the canonical 14-character CNPJ representation without a mask and with uppercase letters.
    /// </summary>
    public string Value => _value ?? throw new InvalidOperationException("A default CNPJ instance does not contain a valid value.");

    /// <summary>
    /// Gets the CNPJ formatted with the canonical <c>AA.AAA.AAA/AAAA-00</c> mask.
    /// </summary>
    public string Formatted => CnpjFormatter.Format(Value);

    /// <summary>
    /// Parses an unmasked or canonically masked numeric or alphanumeric CNPJ.
    /// </summary>
    /// <param name="value">The CNPJ using 14 canonical characters or the canonical <c>AA.AAA.AAA/AAAA-00</c> mask.</param>
    /// <returns>A validated CNPJ value object.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> is not a valid CNPJ.</exception>
    public static Cnpj Parse(string value)
    {
        return Parse(value, provider: null);
    }

    /// <inheritdoc />
    public static Cnpj Parse(string s, IFormatProvider? provider)
    {
        if (s is null)
        {
            throw new FormatException("CNPJ must be provided in a supported format.");
        }

        return Parse(s.AsSpan(), provider);
    }

    /// <inheritdoc />
    public static Cnpj Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!CnpjNormalizer.TryNormalize(s, out string normalized))
        {
            throw new FormatException("CNPJ must contain 12 ASCII letters or digits followed by 2 numeric verification digits, optionally using the canonical mask.");
        }

        return new Cnpj(normalized);
    }

    /// <summary>
    /// Attempts to parse an unmasked or canonically masked numeric or alphanumeric CNPJ.
    /// </summary>
    /// <param name="value">The CNPJ using 14 canonical characters or the canonical <c>AA.AAA.AAA/AAAA-00</c> mask.</param>
    /// <param name="result">When this method returns <see langword="true"/>, contains the validated CNPJ.</param>
    /// <returns><see langword="true"/> when the value is a valid CNPJ; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string? value, out Cnpj result)
    {
        return TryParse(value, provider: null, out result);
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Cnpj result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Cnpj result)
    {
        if (CnpjNormalizer.TryNormalize(s, out string normalized))
        {
            result = new Cnpj(normalized);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Determines whether the supplied text has a supported CNPJ representation and valid verification digits.
    /// </summary>
    /// <param name="value">The CNPJ text to validate.</param>
    /// <returns><see langword="true"/> when the text represents a structurally and mathematically valid CNPJ.</returns>
    public static bool IsValid(string? value)
    {
        return TryParse(value, out _);
    }

    /// <summary>
    /// Returns the canonical unmasked representation of this CNPJ.
    /// </summary>
    /// <returns>The canonical CNPJ value.</returns>
    public override string ToString()
    {
        return Value;
    }

    /// <summary>
    /// Formats this CNPJ using <c>G</c> for the canonical unmasked representation or <c>F</c> for the canonical mask.
    /// </summary>
    /// <param name="format"><c>G</c>, <c>F</c>, an empty string, or <see langword="null"/>.</param>
    /// <param name="formatProvider">Ignored because CNPJ formatting is culture-invariant.</param>
    /// <returns>The requested CNPJ representation.</returns>
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

        throw new FormatException($"Unsupported CNPJ format '{format}'. Use 'G' or 'F'.");
    }
}
