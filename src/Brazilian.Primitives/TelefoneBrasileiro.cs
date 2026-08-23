using System.Diagnostics.CodeAnalysis;

namespace Brazilian.Primitives;

/// <summary>
/// Represents a Brazilian phone number that can be either landline or mobile.
/// </summary>
/// <remarks>
/// This wrapper delegates validation and formatting to <see cref="LandlinePhone"/> and <see cref="MobilePhone"/>.
/// Validation follows the numbering plan only and does not prove existence, activation, ownership, carrier,
/// portability, SMS/WhatsApp capability, or current physical location.
/// </remarks>
public readonly record struct TelefoneBrasileiro : IParsable<TelefoneBrasileiro>, ISpanParsable<TelefoneBrasileiro>, IFormattable
{
    private readonly string? _value;
    private readonly string? _formatted;
    private readonly string? _e164;

    private TelefoneBrasileiro(string value, string formatted, string e164, TipoTelefoneBrasileiro tipo)
    {
        _value = value;
        _formatted = formatted;
        _e164 = e164;
        Tipo = tipo;
    }

    /// <summary>
    /// Gets the canonical national phone number delegated from the underlying primitive.
    /// </summary>
    public string Value => _value ?? throw new InvalidOperationException("A default Brazilian phone instance does not contain a valid value.");

    /// <summary>
    /// Gets the two-digit geographic area code.
    /// </summary>
    public string AreaCode => Value[..2];

    /// <summary>
    /// Gets the subscriber number.
    /// </summary>
    public string SubscriberNumber => Value[2..];

    /// <summary>
    /// Gets the delegated national display representation.
    /// </summary>
    public string Formatted => _formatted ?? throw new InvalidOperationException("A default Brazilian phone instance does not contain a valid value.");

    /// <summary>
    /// Gets the delegated E.164 representation.
    /// </summary>
    public string E164 => _e164 ?? throw new InvalidOperationException("A default Brazilian phone instance does not contain a valid value.");

    /// <summary>
    /// Gets whether the number is landline or mobile.
    /// </summary>
    public TipoTelefoneBrasileiro Tipo
    {
        get;
    }

    /// <summary>
    /// Creates a wrapper from a validated landline phone.
    /// </summary>
    /// <param name="telefone">The landline phone.</param>
    /// <returns>A Brazilian phone value with <see cref="Tipo"/> set to <see cref="TipoTelefoneBrasileiro.Fixo"/>.</returns>
    public static TelefoneBrasileiro From(LandlinePhone telefone)
    {
        return new TelefoneBrasileiro(telefone.Value, telefone.Formatted, telefone.E164, TipoTelefoneBrasileiro.Fixo);
    }

    /// <summary>
    /// Creates a wrapper from a validated mobile phone.
    /// </summary>
    /// <param name="celular">The mobile phone.</param>
    /// <returns>A Brazilian phone value with <see cref="Tipo"/> set to <see cref="TipoTelefoneBrasileiro.Celular"/>.</returns>
    public static TelefoneBrasileiro From(MobilePhone celular)
    {
        return new TelefoneBrasileiro(celular.Value, celular.Formatted, celular.E164, TipoTelefoneBrasileiro.Celular);
    }

    /// <summary>
    /// Attempts to recover the underlying landline phone.
    /// </summary>
    /// <param name="telefone">When successful, contains the landline phone.</param>
    /// <returns><see langword="true"/> when this instance represents a landline phone.</returns>
    public bool TryGetTelefoneFixo(out LandlinePhone telefone)
    {
        if (Tipo == TipoTelefoneBrasileiro.Fixo)
        {
            telefone = LandlinePhone.Parse(Value, provider: null);
            return true;
        }

        telefone = default;
        return false;
    }

    /// <summary>
    /// Attempts to recover the underlying mobile phone.
    /// </summary>
    /// <param name="celular">When successful, contains the mobile phone.</param>
    /// <returns><see langword="true"/> when this instance represents a mobile phone.</returns>
    public bool TryGetCelular(out MobilePhone celular)
    {
        if (Tipo == TipoTelefoneBrasileiro.Celular)
        {
            celular = MobilePhone.Parse(Value, provider: null);
            return true;
        }

        celular = default;
        return false;
    }

    /// <summary>
    /// Parses a supported Brazilian landline or mobile phone representation.
    /// </summary>
    /// <param name="value">The phone text.</param>
    /// <returns>A validated Brazilian phone value.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> is not accepted by either specific primitive.</exception>
    public static TelefoneBrasileiro Parse(string value)
    {
        return Parse(value, provider: null);
    }

    /// <inheritdoc />
    public static TelefoneBrasileiro Parse(string s, IFormatProvider? provider)
    {
        if (s is null)
        {
            throw new FormatException("Brazilian phone must be provided in a supported landline or mobile format.");
        }

        return Parse(s.AsSpan(), provider);
    }

    /// <inheritdoc />
    public static TelefoneBrasileiro Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!TryParse(s, provider, out TelefoneBrasileiro result))
        {
            throw new FormatException("Brazilian phone must be a valid landline or mobile phone representation.");
        }

        return result;
    }

    /// <summary>
    /// Attempts to parse a supported Brazilian landline or mobile phone representation.
    /// </summary>
    /// <param name="value">The phone text.</param>
    /// <param name="result">When successful, contains the Brazilian phone value.</param>
    /// <returns><see langword="true"/> when the value is accepted by one specific phone primitive.</returns>
    public static bool TryParse(string? value, out TelefoneBrasileiro result)
    {
        return TryParse(value, provider: null, out result);
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out TelefoneBrasileiro result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out TelefoneBrasileiro result)
    {
        bool isLandline = LandlinePhone.TryParse(s, provider, out LandlinePhone landline);
        bool isMobile = MobilePhone.TryParse(s, provider, out MobilePhone mobile);

        if (isLandline == isMobile)
        {
            result = default;
            return false;
        }

        result = isLandline ? From(landline) : From(mobile);
        return true;
    }

    /// <summary>
    /// Determines whether the supplied text is accepted by either the landline or mobile primitive.
    /// </summary>
    /// <param name="value">The phone text.</param>
    /// <returns><see langword="true"/> when the value can be represented as a Brazilian phone.</returns>
    public static bool IsValid(string? value)
    {
        return TryParse(value, out _);
    }

    /// <summary>
    /// Returns the canonical national phone number.
    /// </summary>
    /// <returns>The canonical phone number.</returns>
    public override string ToString()
    {
        return Value;
    }

    /// <summary>
    /// Formats this phone using <c>G</c> for canonical national, <c>F</c> for formatted national, or <c>E</c> for E.164.
    /// </summary>
    /// <param name="format"><c>G</c>, <c>F</c>, <c>E</c>, an empty string, or <see langword="null"/>.</param>
    /// <param name="formatProvider">Ignored because formatting is culture-invariant.</param>
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

        throw new FormatException($"Unsupported Brazilian phone format '{format}'. Use 'G', 'F', or 'E'.");
    }
}
