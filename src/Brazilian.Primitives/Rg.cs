namespace Brazilian.Primitives;

/// <summary>
/// Represents a legacy Brazilian Registro Geral (RG) together with its issuing federative unit.
/// </summary>
/// <remarks>
/// Legacy RG numbers do not have a single national format or check-digit algorithm. The issuing state is therefore
/// part of this value object's identity and validation context. São Paulo uses a documented local check-digit rule;
/// states without a sufficiently reliable published algorithm are validated structurally only. Validation never proves
/// that a document exists, is authentic, belongs to a person, or is active in an issuing authority's registry.
/// This type does not represent the Carteira de Identidade Nacional (CIN), whose national registration number is CPF.
/// </remarks>
public readonly record struct Rg
{
    private const int SaoPauloCanonicalLength = 9;
    private const int RioStyleCanonicalLength = 8;
    private const int SantaCatarinaCanonicalLength = 9;

    private readonly string? _value;

    private Rg(string value, BrazilianState state)
    {
        _value = value;
        State = state;
    }

    /// <summary>
    /// Gets the canonical legacy RG representation for the issuing state.
    /// </summary>
    public string Value => _value ?? throw new InvalidOperationException("A default RG instance does not contain a valid value.");

    /// <summary>
    /// Gets the issuing federative unit.
    /// </summary>
    public BrazilianState State { get; }

    /// <summary>
    /// Gets the known display representation for the issuing state. States without a supported mask return <see cref="Value"/>.
    /// </summary>
    public string Formatted => Format(Value, State);

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
    /// Attempts to parse a legacy RG using the explicitly supplied issuing state.
    /// </summary>
    /// <param name="value">The RG in a canonical or explicitly supported state mask.</param>
    /// <param name="state">The issuing federative unit.</param>
    /// <param name="result">When successful, contains the validated legacy RG.</param>
    /// <returns><see langword="true"/> when the value satisfies the known rule for the state; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string? value, BrazilianState state, out Rg result)
    {
        if (value is null || !TryNormalize(value.AsSpan(), state, out string normalized))
        {
            result = default;
            return false;
        }

        result = new Rg(normalized, state);
        return true;
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

    private static bool TryNormalize(ReadOnlySpan<char> input, BrazilianState state, out string normalized)
    {
        if (!TryGetRule(state, out RgStateRule rule))
        {
            normalized = string.Empty;
            return false;
        }

        bool parsed = rule.MaskKind switch
        {
            RgMaskKind.SaoPaulo => TryNormalizeSaoPaulo(input, out normalized),
            RgMaskKind.RioStyle => TryNormalizeRioStyle(input, rule.AllowsMPrefix, out normalized),
            RgMaskKind.SantaCatarina => TryNormalizeSantaCatarina(input, out normalized),
            _ => TryNormalizeDigits(input, rule.CanonicalLength, out normalized),
        };

        if (!parsed)
        {
            return false;
        }

        return !rule.ValidateSaoPauloCheckDigit || HasValidSaoPauloCheckDigit(normalized);
    }

    private static bool TryNormalizeSaoPaulo(ReadOnlySpan<char> input, out string normalized)
    {
        Span<char> canonical = stackalloc char[SaoPauloCanonicalLength];

        if (input.Length == SaoPauloCanonicalLength)
        {
            for (int index = 0; index < SaoPauloCanonicalLength - 1; index++)
            {
                if (!IsAsciiDigit(input[index]))
                {
                    normalized = string.Empty;
                    return false;
                }

                canonical[index] = input[index];
            }

            if (!TryNormalizeSaoPauloCheckDigit(input[^1], out canonical[^1]))
            {
                normalized = string.Empty;
                return false;
            }

            normalized = new string(canonical);
            return true;
        }

        if (input.Length != 12 || input[2] != '.' || input[6] != '.' || input[10] != '-')
        {
            normalized = string.Empty;
            return false;
        }

        int targetIndex = 0;
        for (int sourceIndex = 0; sourceIndex < input.Length; sourceIndex++)
        {
            if (sourceIndex is 2 or 6 or 10)
            {
                continue;
            }

            if (targetIndex == SaoPauloCanonicalLength - 1)
            {
                if (!TryNormalizeSaoPauloCheckDigit(input[sourceIndex], out canonical[targetIndex]))
                {
                    normalized = string.Empty;
                    return false;
                }
            }
            else
            {
                if (!IsAsciiDigit(input[sourceIndex]))
                {
                    normalized = string.Empty;
                    return false;
                }

                canonical[targetIndex] = input[sourceIndex];
            }

            targetIndex++;
        }

        normalized = targetIndex == SaoPauloCanonicalLength ? new string(canonical) : string.Empty;
        return targetIndex == SaoPauloCanonicalLength;
    }

    private static bool TryNormalizeRioStyle(ReadOnlySpan<char> input, bool allowsMPrefix, out string normalized)
    {
        bool prefixed = allowsMPrefix && input.Length > 0 && (input[0] is 'M' or 'm');
        ReadOnlySpan<char> number = prefixed ? input[1..] : input;
        Span<char> canonical = stackalloc char[RioStyleCanonicalLength];

        if (number.Length == RioStyleCanonicalLength)
        {
            if (!TryCopyAsciiDigits(number, canonical))
            {
                normalized = string.Empty;
                return false;
            }
        }
        else if (number.Length == 11 && number[1] == '.' && number[5] == '.' && number[9] == '-')
        {
            int targetIndex = 0;
            for (int sourceIndex = 0; sourceIndex < number.Length; sourceIndex++)
            {
                if (sourceIndex is 1 or 5 or 9)
                {
                    continue;
                }

                if (!IsAsciiDigit(number[sourceIndex]))
                {
                    normalized = string.Empty;
                    return false;
                }

                canonical[targetIndex++] = number[sourceIndex];
            }

            if (targetIndex != RioStyleCanonicalLength)
            {
                normalized = string.Empty;
                return false;
            }
        }
        else
        {
            normalized = string.Empty;
            return false;
        }

        normalized = prefixed ? string.Concat("M", new string(canonical)) : new string(canonical);
        return true;
    }

    private static bool TryNormalizeSantaCatarina(ReadOnlySpan<char> input, out string normalized)
    {
        Span<char> canonical = stackalloc char[SantaCatarinaCanonicalLength];

        if (input.Length == SantaCatarinaCanonicalLength)
        {
            if (!TryCopyAsciiDigits(input, canonical))
            {
                normalized = string.Empty;
                return false;
            }

            normalized = new string(canonical);
            return true;
        }

        if (input.Length != 11 || input[3] != '.' || input[7] != '.')
        {
            normalized = string.Empty;
            return false;
        }

        int targetIndex = 0;
        for (int sourceIndex = 0; sourceIndex < input.Length; sourceIndex++)
        {
            if (sourceIndex is 3 or 7)
            {
                continue;
            }

            if (!IsAsciiDigit(input[sourceIndex]))
            {
                normalized = string.Empty;
                return false;
            }

            canonical[targetIndex++] = input[sourceIndex];
        }

        normalized = targetIndex == SantaCatarinaCanonicalLength ? new string(canonical) : string.Empty;
        return targetIndex == SantaCatarinaCanonicalLength;
    }

    private static bool TryNormalizeDigits(ReadOnlySpan<char> input, int expectedLength, out string normalized)
    {
        if (input.Length != expectedLength)
        {
            normalized = string.Empty;
            return false;
        }

        Span<char> canonical = stackalloc char[10];
        Span<char> destination = canonical[..expectedLength];
        if (!TryCopyAsciiDigits(input, destination))
        {
            normalized = string.Empty;
            return false;
        }

        normalized = new string(destination);
        return true;
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

    private static bool TryNormalizeSaoPauloCheckDigit(char value, out char normalized)
    {
        if (IsAsciiDigit(value))
        {
            normalized = value;
            return true;
        }

        if (value is 'X' or 'x')
        {
            normalized = 'X';
            return true;
        }

        normalized = default;
        return false;
    }

    private static bool HasValidSaoPauloCheckDigit(string canonical)
    {
        int sum = 0;
        for (int index = 0; index < SaoPauloCanonicalLength - 1; index++)
        {
            int weight = 9 - index;
            sum += (canonical[index] - '0') * weight;
        }

        int remainder = sum % 11;
        char expected = remainder == 10 ? 'X' : (char)('0' + remainder);
        return canonical[^1] == expected;
    }

    private static bool IsAsciiDigit(char value)
    {
        return (uint)(value - '0') <= 9;
    }

    private static string Format(string value, BrazilianState state)
    {
        return state switch
        {
            BrazilianState.SaoPaulo => string.Create(12, value, static (destination, source) =>
            {
                destination[0] = source[0];
                destination[1] = source[1];
                destination[2] = '.';
                destination[3] = source[2];
                destination[4] = source[3];
                destination[5] = source[4];
                destination[6] = '.';
                destination[7] = source[5];
                destination[8] = source[6];
                destination[9] = source[7];
                destination[10] = '-';
                destination[11] = source[8];
            }),
            BrazilianState.RioDeJaneiro => FormatRioStyle(value),
            BrazilianState.MinasGerais => value[0] == 'M'
                ? string.Concat("M", FormatRioStyle(value[1..]))
                : FormatRioStyle(value),
            BrazilianState.SantaCatarina => string.Create(11, value, static (destination, source) =>
            {
                destination[0] = source[0];
                destination[1] = source[1];
                destination[2] = source[2];
                destination[3] = '.';
                destination[4] = source[3];
                destination[5] = source[4];
                destination[6] = source[5];
                destination[7] = '.';
                destination[8] = source[6];
                destination[9] = source[7];
                destination[10] = source[8];
            }),
            _ => value,
        };
    }

    private static string FormatRioStyle(string value)
    {
        return string.Create(11, value, static (destination, source) =>
        {
            destination[0] = source[0];
            destination[1] = '.';
            destination[2] = source[1];
            destination[3] = source[2];
            destination[4] = source[3];
            destination[5] = '.';
            destination[6] = source[4];
            destination[7] = source[5];
            destination[8] = source[6];
            destination[9] = '-';
            destination[10] = source[7];
        });
    }

    private static bool TryGetRule(BrazilianState state, out RgStateRule rule)
    {
        rule = state switch
        {
            BrazilianState.Acre => new RgStateRule(6, RgMaskKind.None, false, false),
            BrazilianState.Alagoas => new RgStateRule(7, RgMaskKind.None, false, false),
            BrazilianState.Amapa => new RgStateRule(9, RgMaskKind.None, false, false),
            BrazilianState.Amazonas => new RgStateRule(9, RgMaskKind.None, false, false),
            BrazilianState.Bahia => new RgStateRule(10, RgMaskKind.None, false, false),
            BrazilianState.Ceara => new RgStateRule(9, RgMaskKind.None, false, false),
            BrazilianState.DistritoFederal => new RgStateRule(7, RgMaskKind.None, false, false),
            BrazilianState.EspiritoSanto => new RgStateRule(9, RgMaskKind.None, false, false),
            BrazilianState.Goias => new RgStateRule(9, RgMaskKind.None, false, false),
            BrazilianState.Maranhao => new RgStateRule(9, RgMaskKind.None, false, false),
            BrazilianState.MatoGrosso => new RgStateRule(9, RgMaskKind.None, false, false),
            BrazilianState.MatoGrossoDoSul => new RgStateRule(9, RgMaskKind.None, false, false),
            BrazilianState.MinasGerais => new RgStateRule(8, RgMaskKind.RioStyle, false, true),
            BrazilianState.Para => new RgStateRule(9, RgMaskKind.None, false, false),
            BrazilianState.Paraiba => new RgStateRule(9, RgMaskKind.None, false, false),
            BrazilianState.Parana => new RgStateRule(8, RgMaskKind.None, false, false),
            BrazilianState.Pernambuco => new RgStateRule(9, RgMaskKind.None, false, false),
            BrazilianState.Piaui => new RgStateRule(9, RgMaskKind.None, false, false),
            BrazilianState.RioDeJaneiro => new RgStateRule(8, RgMaskKind.RioStyle, false, false),
            BrazilianState.RioGrandeDoNorte => new RgStateRule(9, RgMaskKind.None, false, false),
            BrazilianState.RioGrandeDoSul => new RgStateRule(10, RgMaskKind.None, false, false),
            BrazilianState.Rondonia => new RgStateRule(9, RgMaskKind.None, false, false),
            BrazilianState.Roraima => new RgStateRule(9, RgMaskKind.None, false, false),
            BrazilianState.SantaCatarina => new RgStateRule(9, RgMaskKind.SantaCatarina, false, false),
            BrazilianState.SaoPaulo => new RgStateRule(9, RgMaskKind.SaoPaulo, true, false),
            BrazilianState.Sergipe => new RgStateRule(9, RgMaskKind.None, false, false),
            BrazilianState.Tocantins => new RgStateRule(9, RgMaskKind.None, false, false),
            _ => default,
        };

        return state is >= BrazilianState.Acre and <= BrazilianState.Tocantins;
    }

    private enum RgMaskKind
    {
        None,
        SaoPaulo,
        RioStyle,
        SantaCatarina,
    }

    private readonly record struct RgStateRule(
        int CanonicalLength,
        RgMaskKind MaskKind,
        bool ValidateSaoPauloCheckDigit,
        bool AllowsMPrefix);
}
