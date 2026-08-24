namespace Brazilian.PrimitivesTypes;

/// <summary>
/// Represents a legacy Brazilian Registro Geral (RG), optionally with issuing federative-unit context.
/// </summary>
/// <remarks>
/// Legacy RG numbers do not have a single national format or check-digit algorithm. When an issuing state is supplied,
/// it is part of this value object's identity and validation context. São Paulo uses a documented local check-digit rule;
/// states without a sufficiently reliable published algorithm are validated structurally only. When no state is supplied,
/// validation is intentionally conservative and format-only: only canonical, unmasked legacy shapes are accepted and no
/// state, state-specific mask, or checksum is inferred. Validation never proves that a document exists, is authentic,
/// belongs to a person, or is active in an issuing authority's registry. This type does not represent the Carteira de
/// Identidade Nacional (CIN), whose national registration number is CPF.
/// </remarks>
public readonly record struct Rg
{
    private readonly string? _value;

    private Rg(string value, BrazilianState state)
    {
        bool parsed = state == BrazilianState.Unknown
            ? RgNormalizer.TryNormalizeContextFree(value.AsSpan(), out string normalized)
            : RgNormalizer.TryNormalize(value.AsSpan(), state, out normalized);

        if (!parsed)
        {
            throw new FormatException("RG does not satisfy the selected validation context.");
        }

        _value = normalized;
        State = state;
    }

    /// <summary>
    /// Gets the canonical legacy RG representation.
    /// </summary>
    public string Value => _value ?? throw new InvalidOperationException("A default RG instance does not contain a valid value.");

    /// <summary>
    /// Gets the issuing federative unit, or <see cref="BrazilianState.Unknown"/> when no state context was supplied.
    /// </summary>
    public BrazilianState State
    {
        get;
    }

    /// <summary>
    /// Gets a value indicating whether an issuing federative unit was supplied.
    /// </summary>
    public bool HasState => State != BrazilianState.Unknown;

    /// <summary>
    /// Gets the known display representation for the issuing state. Context-free values and states without a supported
    /// mask return <see cref="Value"/>.
    /// </summary>
    public string Formatted => HasState ? RgFormatter.Format(Value, State) : Value;

    /// <summary>
    /// Parses a legacy RG without issuing-state context.
    /// </summary>
    /// <remarks>
    /// Context-free parsing is structural and format-only. It accepts only canonical unmasked values with 6 to 10
    /// characters, ASCII digits, and an optional final <c>X</c> only in a nine-character value. It does not infer a UF,
    /// a state-specific mask, or a state-specific checksum rule.
    /// </remarks>
    /// <param name="value">The canonical unmasked RG text.</param>
    /// <returns>A structurally validated context-free RG value object.</returns>
    /// <exception cref="FormatException">Thrown when the value does not satisfy the context-free structural rules.</exception>
    public static Rg Parse(string value)
    {
        if (!TryParse(value, out Rg result))
        {
            throw new FormatException("RG without state context must use a supported canonical legacy shape.");
        }

        return result;
    }

    /// <summary>
    /// Parses a legacy RG using the explicitly supplied issuing state.
    /// </summary>
    /// <param name="value">The RG in a canonical or explicitly supported state mask.</param>
    /// <param name="state">The issuing federative unit.</param>
    /// <returns>A validated legacy RG value object.</returns>
    /// <exception cref="FormatException">Thrown when the state is unknown or the value does not satisfy its state rule.</exception>
    public static Rg Parse(string value, BrazilianState state)
    {
        if (!TryParse(value, state, out Rg result))
        {
            throw new FormatException("RG must match a supported legacy format for the explicitly supplied issuing state.");
        }

        return result;
    }

    /// <summary>
    /// Attempts to parse a legacy RG without issuing-state context.
    /// </summary>
    /// <param name="value">The canonical unmasked RG text.</param>
    /// <param name="result">When successful, contains the structurally validated context-free RG.</param>
    /// <returns><see langword="true"/> when the value satisfies the context-free structural rules.</returns>
    public static bool TryParse(string? value, out Rg result)
    {
        if (value is null || !RgNormalizer.TryNormalizeContextFree(value.AsSpan(), out string normalized))
        {
            result = default;
            return false;
        }

        result = new Rg(normalized, BrazilianState.Unknown);
        return true;
    }

    /// <summary>
    /// Attempts to parse a legacy RG using the explicitly supplied issuing state.
    /// </summary>
    /// <param name="value">The RG in a canonical or explicitly supported state mask.</param>
    /// <param name="state">The issuing federative unit.</param>
    /// <param name="result">When successful, contains the validated legacy RG.</param>
    /// <returns><see langword="true"/> when the value satisfies the known rule for the state; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string? value, BrazilianState state, out Rg result)
    {
        if (state == BrazilianState.Unknown || value is null || !RgNormalizer.TryNormalize(value.AsSpan(), state, out string normalized))
        {
            result = default;
            return false;
        }

        result = new Rg(normalized, state);
        return true;
    }

    /// <summary>
    /// Determines whether a legacy RG satisfies the context-free structural rules.
    /// </summary>
    /// <remarks>
    /// This validation is format-only. It does not infer a UF and does not apply a state-specific checksum.
    /// </remarks>
    /// <param name="value">The canonical unmasked RG text to validate.</param>
    /// <returns><see langword="true"/> when the value satisfies the context-free structural rules.</returns>
    public static bool IsValid(string? value)
    {
        return TryParse(value, out _);
    }

    /// <summary>
    /// Determines whether a legacy RG satisfies the known validation rule for its issuing state.
    /// </summary>
    /// <remarks>
    /// For São Paulo this includes the legacy SSP/IIRGD check digit. For states documented as format-only, this method
    /// verifies only the supported local structure and characters and must not be interpreted as mathematical,
    /// cadastral, or authenticity validation.
    /// </remarks>
    /// <param name="value">The RG text to validate.</param>
    /// <param name="state">The issuing federative unit.</param>
    /// <returns><see langword="true"/> when the value satisfies the known rule for the state.</returns>
    public static bool IsValid(string? value, BrazilianState state)
    {
        return TryParse(value, state, out _);
    }

    /// <summary>
    /// Returns the canonical legacy RG representation.
    /// </summary>
    /// <returns>The canonical RG value.</returns>
    public override string ToString()
    {
        return Value;
    }
}
