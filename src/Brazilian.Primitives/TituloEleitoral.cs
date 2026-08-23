using System.Diagnostics.CodeAnalysis;

namespace Brazilian.Primitives;

/// <summary>
/// Represents a Brazilian voter registration number (Titulo Eleitoral).
/// </summary>
/// <remarks>
/// Validation is structural and mathematical according to the supported 12-digit canonical representation:
/// eight sequential digits, two origin digits, and two modulo-11 check digits, including the first-check-digit
/// remainder-10 exception for Sao Paulo and Minas Gerais origin codes. A valid <see cref="TituloEleitoral"/> does not
/// prove current electoral regularity, discharge, domicile, polling section, biometric status, or ownership.
/// </remarks>
public readonly record struct TituloEleitoral : IParsable<TituloEleitoral>, ISpanParsable<TituloEleitoral>
{
    private const int DigitCount = 12;
    private const int SequentialLength = 8;

    private readonly string? _value;

    private TituloEleitoral(string value)
    {
        _value = value;
    }

    /// <summary>
    /// Gets the canonical 12-digit voter registration number.
    /// </summary>
    public string Value => _value ?? throw new InvalidOperationException("A default voter registration instance does not contain a valid value.");

    /// <summary>
    /// Gets the eight-digit sequential portion.
    /// </summary>
    public string NumeroSequencial => Value[..SequentialLength];

    /// <summary>
    /// Gets the two-digit origin code.
    /// </summary>
    public string CodigoOrigem => Value.Substring(8, 2);

    /// <summary>
    /// Gets whether the origin code is <c>28</c>, representing Exterior (ZZ).
    /// </summary>
    public bool IsExterior => CodigoOrigem == "28";

    /// <summary>
    /// Attempts to map national origin codes <c>01</c> through <c>27</c> to <see cref="BrazilianState"/>.
    /// </summary>
    /// <param name="state">When successful, contains the Brazilian state.</param>
    /// <returns><see langword="true"/> for national origin codes; <see langword="false"/> for Exterior.</returns>
    public bool TryGetState(out BrazilianState state)
    {
        return TryGetState(CodigoOrigem.AsSpan(), out state);
    }

    /// <summary>
    /// Parses a canonical 12-digit voter registration number.
    /// </summary>
    /// <param name="value">The voter registration text.</param>
    /// <returns>A validated voter registration value object.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> is not a valid voter registration number.</exception>
    public static TituloEleitoral Parse(string value)
    {
        return Parse(value, provider: null);
    }

    /// <inheritdoc />
    public static TituloEleitoral Parse(string s, IFormatProvider? provider)
    {
        if (s is null)
        {
            throw new FormatException("Voter registration must be provided as exactly 12 ASCII digits.");
        }

        return Parse(s.AsSpan(), provider);
    }

    /// <inheritdoc />
    public static TituloEleitoral Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!TryNormalize(s, out string normalized))
        {
            throw new FormatException("Voter registration must contain 12 ASCII digits, a valid origin code, and valid check digits.");
        }

        return new TituloEleitoral(normalized);
    }

    /// <summary>
    /// Attempts to parse a canonical 12-digit voter registration number.
    /// </summary>
    /// <param name="value">The voter registration text.</param>
    /// <param name="result">When successful, contains the voter registration.</param>
    /// <returns><see langword="true"/> when the value is valid.</returns>
    public static bool TryParse(string? value, out TituloEleitoral result)
    {
        return TryParse(value, provider: null, out result);
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out TituloEleitoral result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out TituloEleitoral result)
    {
        if (TryNormalize(s, out string normalized))
        {
            result = new TituloEleitoral(normalized);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Determines whether the supplied text is a valid canonical voter registration number.
    /// </summary>
    /// <param name="value">The voter registration text.</param>
    /// <returns><see langword="true"/> when the value has a valid structure, origin, and check digits.</returns>
    public static bool IsValid(string? value)
    {
        return TryParse(value, out _);
    }

    /// <summary>
    /// Returns the canonical 12-digit voter registration number.
    /// </summary>
    /// <returns>The canonical voter registration number.</returns>
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

        if (!IsValidOrigin(digits.Slice(8, 2)) || !HasValidCheckDigits(digits))
        {
            return false;
        }

        normalized = new string(digits);
        return true;
    }

    private static bool HasValidCheckDigits(ReadOnlySpan<char> digits)
    {
        ReadOnlySpan<char> origin = digits.Slice(8, 2);
        int firstCheckDigit = CalculateFirstCheckDigit(digits[..SequentialLength], origin);
        if (digits[10] - '0' != firstCheckDigit)
        {
            return false;
        }

        int secondCheckDigit = CalculateSecondCheckDigit(origin, firstCheckDigit);
        return digits[11] - '0' == secondCheckDigit;
    }

    private static int CalculateFirstCheckDigit(ReadOnlySpan<char> sequential, ReadOnlySpan<char> origin)
    {
        int sum = 0;
        for (int index = 0; index < SequentialLength; index++)
        {
            sum += (sequential[index] - '0') * (index + 2);
        }

        int checkDigit = sum % 11;
        if (checkDigit != 10)
        {
            return checkDigit;
        }

        return IsSaoPauloOrMinasGerais(origin) ? 1 : 0;
    }

    private static int CalculateSecondCheckDigit(ReadOnlySpan<char> origin, int firstCheckDigit)
    {
        int sum = ((origin[0] - '0') * 7) + ((origin[1] - '0') * 8) + (firstCheckDigit * 9);
        int checkDigit = sum % 11;
        return checkDigit == 10 ? 0 : checkDigit;
    }

    private static bool IsValidOrigin(ReadOnlySpan<char> origin)
    {
        int value = ((origin[0] - '0') * 10) + (origin[1] - '0');
        return value is >= 1 and <= 28;
    }

    private static bool IsSaoPauloOrMinasGerais(ReadOnlySpan<char> origin)
    {
        return origin is "01" or "02";
    }

    private static bool TryGetState(ReadOnlySpan<char> origin, out BrazilianState state)
    {
        state = origin switch
        {
            "01" => BrazilianState.SaoPaulo,
            "02" => BrazilianState.MinasGerais,
            "03" => BrazilianState.RioDeJaneiro,
            "04" => BrazilianState.RioGrandeDoSul,
            "05" => BrazilianState.Bahia,
            "06" => BrazilianState.Parana,
            "07" => BrazilianState.Ceara,
            "08" => BrazilianState.Pernambuco,
            "09" => BrazilianState.SantaCatarina,
            "10" => BrazilianState.Goias,
            "11" => BrazilianState.Maranhao,
            "12" => BrazilianState.Paraiba,
            "13" => BrazilianState.Para,
            "14" => BrazilianState.EspiritoSanto,
            "15" => BrazilianState.Piaui,
            "16" => BrazilianState.RioGrandeDoNorte,
            "17" => BrazilianState.Alagoas,
            "18" => BrazilianState.MatoGrosso,
            "19" => BrazilianState.MatoGrossoDoSul,
            "20" => BrazilianState.DistritoFederal,
            "21" => BrazilianState.Sergipe,
            "22" => BrazilianState.Amazonas,
            "23" => BrazilianState.Rondonia,
            "24" => BrazilianState.Acre,
            "25" => BrazilianState.Amapa,
            "26" => BrazilianState.Roraima,
            "27" => BrazilianState.Tocantins,
            _ => BrazilianState.Unknown,
        };

        return state != BrazilianState.Unknown;
    }

    private static bool IsAsciiDigit(char value)
    {
        return (uint)(value - '0') <= 9;
    }
}
