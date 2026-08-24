using System.Diagnostics.CodeAnalysis;

namespace Brazilian.PrimitivesTypes;

/// <summary>
/// Represents a local Pix key value according to the supported DICT key kinds.
/// </summary>
/// <remarks>
/// Validation is local and deterministic. A valid <see cref="ChavePix"/> does not prove that the key is registered,
/// active, owned by a person, associated with an account, portable, claimable, or usable for a Pix transaction.
/// Untyped parsing rejects values that match more than one supported key kind; use the explicit factory methods when
/// the key kind is already known.
/// </remarks>
public readonly record struct ChavePix : IParsable<ChavePix>, ISpanParsable<ChavePix>
{
    private const int MaxPixEmailLength = 77;

    private readonly string? _value;

    private ChavePix(string value, TipoChavePix tipo)
    {
        _value = value;
        Tipo = tipo;
    }

    internal static ChavePix Create(string value, TipoChavePix tipo)
    {
        return new ChavePix(value, tipo);
    }

    /// <summary>
    /// Gets the canonical Pix key value.
    /// </summary>
    public string Value => _value ?? throw new InvalidOperationException("A default Pix key instance does not contain a valid value.");

    /// <summary>
    /// Gets the Pix key type.
    /// </summary>
    public TipoChavePix Tipo
    {
        get;
    }

    /// <summary>
    /// Creates a Pix CPF key from a validated CPF.
    /// </summary>
    /// <param name="cpf">The CPF value object.</param>
    /// <returns>A CPF Pix key.</returns>
    public static ChavePix From(Cpf cpf)
    {
        return new ChavePix(cpf.Value, TipoChavePix.Cpf);
    }

    /// <summary>
    /// Creates a Pix CNPJ key from a validated CNPJ.
    /// </summary>
    /// <param name="cnpj">The CNPJ value object.</param>
    /// <returns>A CNPJ Pix key.</returns>
    public static ChavePix From(Cnpj cnpj)
    {
        return new ChavePix(cnpj.Value, TipoChavePix.Cnpj);
    }

    /// <summary>
    /// Creates a Pix mobile-phone key from a validated Brazilian mobile phone.
    /// </summary>
    /// <param name="celular">The mobile phone value object.</param>
    /// <returns>A mobile-phone Pix key in E.164 format.</returns>
    public static ChavePix From(MobilePhone celular)
    {
        return new ChavePix(celular.E164, TipoChavePix.Celular);
    }

    /// <summary>
    /// Creates a Pix email key from a validated email, applying the Pix lowercase and length rules.
    /// </summary>
    /// <param name="email">The email value object.</param>
    /// <returns>An email Pix key.</returns>
    /// <exception cref="FormatException">Thrown when the email is not compatible with Pix email-key limits.</exception>
    public static ChavePix From(Email email)
    {
        string value = email.Value.ToLowerInvariant();
        if (value.Length > MaxPixEmailLength)
        {
            throw new FormatException("Pix email key must contain at most 77 characters.");
        }

        return new ChavePix(value, TipoChavePix.Email);
    }

    /// <summary>
    /// Creates a Pix random-key (EVP) from a canonical UUID textual representation.
    /// </summary>
    /// <param name="value">The random key UUID text.</param>
    /// <returns>A random Pix key.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> is not a canonical UUID text.</exception>
    public static ChavePix FromChaveAleatoria(string value)
    {
        if (value is null || !PixRandomKeyNormalizer.TryNormalize(value.AsSpan(), out string normalized))
        {
            throw new FormatException("Pix random key must be a canonical UUID text.");
        }

        return new ChavePix(normalized, TipoChavePix.Aleatoria);
    }

    /// <summary>
    /// Parses a supported Pix key representation.
    /// </summary>
    /// <param name="value">The Pix key text.</param>
    /// <returns>A validated Pix key.</returns>
    /// <exception cref="FormatException">
    /// Thrown when <paramref name="value"/> is not a supported Pix key or matches more than one supported key kind.
    /// </exception>
    public static ChavePix Parse(string value)
    {
        return Parse(value, provider: null);
    }

    /// <inheritdoc />
    public static ChavePix Parse(string s, IFormatProvider? provider)
    {
        if (s is null)
        {
            throw new FormatException("Pix key must be provided in a supported format.");
        }

        return Parse(s.AsSpan(), provider);
    }

    /// <inheritdoc />
    public static ChavePix Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!TryParse(s, provider, out ChavePix result))
        {
            throw new FormatException("Pix key must be a valid CPF, CNPJ, mobile phone, email, or random key.");
        }

        return result;
    }

    /// <summary>
    /// Attempts to parse a supported Pix key representation.
    /// </summary>
    /// <param name="value">The Pix key text.</param>
    /// <param name="result">When successful, contains the Pix key.</param>
    /// <returns>
    /// <see langword="true"/> when the value matches exactly one supported Pix key kind; otherwise,
    /// <see langword="false"/>.
    /// </returns>
    public static bool TryParse(string? value, out ChavePix result)
    {
        return TryParse(value, provider: null, out result);
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out ChavePix result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out ChavePix result)
    {
        return PixKeyClassifier.TryClassify(s, provider, out result);
    }

    /// <summary>
    /// Determines whether the supplied text can be represented as a supported Pix key.
    /// </summary>
    /// <param name="value">The Pix key text.</param>
    /// <returns><see langword="true"/> when the value matches exactly one supported Pix key kind.</returns>
    public static bool IsValid(string? value)
    {
        return TryParse(value, out _);
    }

    /// <summary>
    /// Returns the canonical Pix key value.
    /// </summary>
    /// <returns>The canonical Pix key value.</returns>
    public override string ToString()
    {
        return Value;
    }

    internal static bool TryCreateFromEmail(Email email, out ChavePix result)
    {
        string value = email.Value.ToLowerInvariant();
        if (value.Length <= MaxPixEmailLength)
        {
            result = new ChavePix(value, TipoChavePix.Email);
            return true;
        }

        result = default;
        return false;
    }
}
