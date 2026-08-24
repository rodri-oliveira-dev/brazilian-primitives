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
    private const int ContextFreeMinimumLength = 8;
    private const int ContextFreeMaximumLength = 14;

    private readonly string? _value;

    private InscricaoEstadual(string value, BrazilianState state)
    {
        bool parsed = state == BrazilianState.Unknown
            ? TryNormalizeContextFree(value.AsSpan(), out string normalized)
            : TryNormalize(value.AsSpan(), state, out normalized);

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
    /// Attempts to parse an Inscricao Estadual without federative-unit context.
    /// </summary>
    /// <param name="value">The canonical registration text.</param>
    /// <param name="result">When successful, contains the structurally validated context-free registration.</param>
    /// <returns><see langword="true"/> when the value satisfies the context-free structural rules.</returns>
    public static bool TryParse(string? value, out InscricaoEstadual result)
    {
        if (value is null || !TryNormalizeContextFree(value.AsSpan(), out string normalized))
        {
            result = default;
            return false;
        }

        result = new InscricaoEstadual(normalized, BrazilianState.Unknown);
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
    /// Attempts to parse an Inscricao Estadual using the explicitly supplied state context.
    /// </summary>
    /// <param name="value">The registration text.</param>
    /// <param name="state">The federative unit context.</param>
    /// <param name="result">When successful, contains the validated registration.</param>
    /// <returns><see langword="true"/> when the value satisfies the state strategy; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string? value, BrazilianState state, out InscricaoEstadual result)
    {
        if (state == BrazilianState.Unknown || value is null || !TryNormalize(value.AsSpan(), state, out string normalized))
        {
            result = default;
            return false;
        }

        result = new InscricaoEstadual(normalized, state);
        return true;
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

    private static bool TryNormalizeContextFree(ReadOnlySpan<char> input, out string normalized)
    {
        normalized = string.Empty;
        if (input.Length < ContextFreeMinimumLength || input.Length > ContextFreeMaximumLength)
        {
            return false;
        }

        Span<char> canonical = stackalloc char[ContextFreeMaximumLength];
        Span<char> destination = canonical[..input.Length];
        if (!TryCopyAsciiDigits(input, destination))
        {
            return false;
        }

        normalized = new string(destination);
        return true;
    }

    private static bool TryNormalize(ReadOnlySpan<char> input, BrazilianState state, out string normalized)
    {
        normalized = string.Empty;
        if (!TryGetRule(state, out InscricaoEstadualRule rule) || IsIsento(input))
        {
            return false;
        }

        if (TryNormalizeWithLength(input, rule.FirstLength, out normalized))
        {
            return true;
        }

        return rule.SecondLength != 0 && TryNormalizeWithLength(input, rule.SecondLength, out normalized);
    }

    private static bool TryNormalizeWithLength(ReadOnlySpan<char> input, int length, out string normalized)
    {
        normalized = string.Empty;
        if (input.Length != length)
        {
            return false;
        }

        Span<char> canonical = stackalloc char[ContextFreeMaximumLength];
        Span<char> destination = canonical[..length];
        if (!TryCopyAsciiDigits(input, destination))
        {
            return false;
        }

        normalized = new string(destination);
        return true;
    }

    private static bool IsIsento(ReadOnlySpan<char> input)
    {
        return input.Equals("ISENTO".AsSpan(), StringComparison.OrdinalIgnoreCase);
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

    private static bool TryGetRule(BrazilianState state, out InscricaoEstadualRule rule)
    {
        rule = state switch
        {
            BrazilianState.Acre => new InscricaoEstadualRule(13),
            BrazilianState.Alagoas => new InscricaoEstadualRule(9),
            BrazilianState.Amapa => new InscricaoEstadualRule(9),
            BrazilianState.Amazonas => new InscricaoEstadualRule(9),
            BrazilianState.Bahia => new InscricaoEstadualRule(8, 9),
            BrazilianState.Ceara => new InscricaoEstadualRule(9),
            BrazilianState.DistritoFederal => new InscricaoEstadualRule(13),
            BrazilianState.EspiritoSanto => new InscricaoEstadualRule(9),
            BrazilianState.Goias => new InscricaoEstadualRule(9),
            BrazilianState.Maranhao => new InscricaoEstadualRule(9),
            BrazilianState.MatoGrosso => new InscricaoEstadualRule(11),
            BrazilianState.MatoGrossoDoSul => new InscricaoEstadualRule(9),
            BrazilianState.MinasGerais => new InscricaoEstadualRule(13),
            BrazilianState.Para => new InscricaoEstadualRule(9),
            BrazilianState.Paraiba => new InscricaoEstadualRule(9),
            BrazilianState.Parana => new InscricaoEstadualRule(10),
            BrazilianState.Pernambuco => new InscricaoEstadualRule(9, 14),
            BrazilianState.Piaui => new InscricaoEstadualRule(9),
            BrazilianState.RioDeJaneiro => new InscricaoEstadualRule(8),
            BrazilianState.RioGrandeDoNorte => new InscricaoEstadualRule(9, 10),
            BrazilianState.RioGrandeDoSul => new InscricaoEstadualRule(10),
            BrazilianState.Rondonia => new InscricaoEstadualRule(14),
            BrazilianState.Roraima => new InscricaoEstadualRule(9),
            BrazilianState.SantaCatarina => new InscricaoEstadualRule(9),
            BrazilianState.SaoPaulo => new InscricaoEstadualRule(12),
            BrazilianState.Sergipe => new InscricaoEstadualRule(9),
            BrazilianState.Tocantins => new InscricaoEstadualRule(11),
            _ => default,
        };

        return rule.FirstLength != 0;
    }

    private static bool IsAsciiDigit(char value)
    {
        return (uint)(value - '0') <= 9;
    }

    private readonly record struct InscricaoEstadualRule(int FirstLength, int SecondLength = 0);
}
