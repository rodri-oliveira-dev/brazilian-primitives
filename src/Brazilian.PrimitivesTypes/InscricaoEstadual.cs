namespace Brazilian.PrimitivesTypes;

/// <summary>
/// Represents a Brazilian state tax registration (Inscricao Estadual), optionally with federative-unit context.
/// </summary>
/// <remarks>
/// There is no single national Inscricao Estadual format or check-digit algorithm. When a state is supplied, this type
/// validates the explicit structural strategy documented for that UF without inventing state check digits where a
/// sufficiently authoritative formula is not embedded in the library. When no state is supplied, validation is
/// deliberately structural and format-only: only canonical 8-to-14-digit values are accepted and no UF, state-specific
/// checksum, or display mask is inferred. Validation does not prove registration existence, taxpayer status, NF-e
/// authorization, tax regularity, or relation to a CPF/CNPJ.
/// </remarks>
public readonly record struct InscricaoEstadual
{
    private readonly string? _value;

    private InscricaoEstadual(string value, BrazilianState state)
    {
        bool parsed = state == BrazilianState.Unknown
            ? InscricaoEstadualNormalizer.TryNormalizeContextFree(value.AsSpan(), out string normalized)
            : InscricaoEstadualNormalizer.TryNormalize(value.AsSpan(), state, out normalized);

        if (!parsed)
        {
            throw new FormatException("Inscricao Estadual does not satisfy the selected validation context.");
        }

        _value = normalized;
        State = state;
    }

    /// <summary>
    /// Gets the canonical Inscricao Estadual value.
    /// </summary>
    public string Value => _value ?? throw new InvalidOperationException("A default state tax registration instance does not contain a valid value.");

    /// <summary>
    /// Gets the federative unit used as validation context, or <see cref="BrazilianState.Unknown"/> when no state was supplied.
    /// </summary>
    public BrazilianState State
    {
        get;
    }

    /// <summary>
    /// Gets a value indicating whether a federative-unit validation context was supplied.
    /// </summary>
    public bool HasState => State != BrazilianState.Unknown;

    /// <summary>
    /// Gets the display representation. Context-free and format-only strategies return the canonical value.
    /// </summary>
    public string Formatted => Value;

    /// <summary>
    /// Parses an Inscricao Estadual without federative-unit context.
    /// </summary>
    /// <remarks>
    /// Context-free parsing accepts only 8 to 14 ASCII digits. It is structural/format-only and does not infer a UF or
    /// apply a state-specific checksum.
    /// </remarks>
    /// <param name="value">The canonical registration text.</param>
    /// <returns>A structurally validated context-free state registration.</returns>
    /// <exception cref="FormatException">Thrown when the value does not satisfy the context-free structural rules.</exception>
    public static InscricaoEstadual Parse(string value)
    {
        if (!TryParse(value, out InscricaoEstadual result))
        {
            throw new FormatException("Inscricao Estadual without state context must contain 8 to 14 ASCII digits.");
        }

        return result;
    }

    /// <summary>
    /// Parses an Inscricao Estadual using the explicitly supplied state context.
    /// </summary>
    /// <param name="value">The registration text.</param>
    /// <param name="state">The federative unit context.</param>
    /// <returns>A validated state tax registration value object.</returns>
    /// <exception cref="FormatException">Thrown when the state is unknown or the value does not satisfy its state strategy.</exception>
    public static InscricaoEstadual Parse(string value, BrazilianState state)
    {
        if (!TryParse(value, state, out InscricaoEstadual result))
        {
            throw new FormatException("Inscricao Estadual must match the documented structural strategy for the explicitly supplied state.");
        }

        return result;
    }

    /// <summary>
    /// Attempts to parse an Inscricao Estadual without federative-unit context.
    /// </summary>
    /// <param name="value">The canonical registration text.</param>
    /// <param name="result">When successful, contains the structurally validated context-free registration.</param>
    /// <returns><see langword="true"/> when the value satisfies the context-free structural rules.</returns>
    public static bool TryParse(string? value, out InscricaoEstadual result)
    {
        if (value is null || !InscricaoEstadualNormalizer.TryNormalizeContextFree(value.AsSpan(), out string normalized))
        {
            result = default;
            return false;
        }

        result = new InscricaoEstadual(normalized, BrazilianState.Unknown);
        return true;
    }

    /// <summary>
    /// Attempts to parse an Inscricao Estadual using the explicitly supplied state context.
    /// </summary>
    /// <param name="value">The registration text.</param>
    /// <param name="state">The federative unit context.</param>
    /// <param name="result">When successful, contains the validated registration.</param>
    /// <returns><see langword="true"/> when the value satisfies the state strategy; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string? value, BrazilianState state, out InscricaoEstadual result)
    {
        if (state == BrazilianState.Unknown || value is null || !InscricaoEstadualNormalizer.TryNormalize(value.AsSpan(), state, out string normalized))
        {
            result = default;
            return false;
        }

        result = new InscricaoEstadual(normalized, state);
        return true;
    }

    /// <summary>
    /// Determines whether the value satisfies the context-free structural rules.
    /// </summary>
    /// <remarks>
    /// This validation is format-only. It does not infer a UF and does not apply a state-specific checksum.
    /// </remarks>
    /// <param name="value">The registration text.</param>
    /// <returns><see langword="true"/> when the value contains 8 to 14 ASCII digits.</returns>
    public static bool IsValid(string? value)
    {
        return TryParse(value, out _);
    }

    /// <summary>
    /// Determines whether the value satisfies the documented strategy for the supplied state.
    /// </summary>
    /// <param name="value">The registration text.</param>
    /// <param name="state">The federative unit context.</param>
    /// <returns><see langword="true"/> when the value satisfies the state strategy.</returns>
    public static bool IsValid(string? value, BrazilianState state)
    {
        return TryParse(value, state, out _);
    }

    /// <summary>
    /// Returns the canonical registration value.
    /// </summary>
    /// <returns>The canonical registration value.</returns>
    public override string ToString()
    {
        return Value;
    }
}
