using System.Diagnostics.CodeAnalysis;

namespace Brazilian.PrimitivesTypes;

/// <summary>
/// Represents a Brazilian vehicle plate textual identifier.
/// </summary>
/// <remarks>
/// Validation only identifies whether the seven-character sequence matches the previous national pattern or the
/// Mercosur/PIV pattern. It does not infer vehicle category, visual color, physical dimensions, plate quantity,
/// assignment, existence, regularity, QR Code status, or relation to a RENAVAM/chassis.
/// </remarks>
public readonly record struct PlacaVeiculo : IParsable<PlacaVeiculo>, ISpanParsable<PlacaVeiculo>, IFormattable
{
    private const int CanonicalLength = 7;
    private const int PreviousFormattedLength = 8;

    private readonly string? _value;

    private PlacaVeiculo(string value, PadraoPlacaVeiculo padrao)
    {
        _value = value;
        Padrao = padrao;
    }

    /// <summary>
    /// Gets the canonical seven-character uppercase plate sequence without separators.
    /// </summary>
    public string Value => _value ?? throw new InvalidOperationException("A default vehicle plate instance does not contain a valid value.");

    /// <summary>
    /// Gets the supported plate sequence pattern.
    /// </summary>
    public PadraoPlacaVeiculo Padrao
    {
        get;
    }

    /// <summary>
    /// Gets the display format for the sequence pattern.
    /// </summary>
    public string Formatted => Padrao == PadraoPlacaVeiculo.NacionalAnterior ? string.Concat(Value[..3], "-", Value[3..]) : Value;

    /// <summary>
    /// Returns the algorithmic sequence equivalent in the Mercosur pattern for a previous national plate.
    /// </summary>
    /// <returns>The converted Mercosur-pattern sequence.</returns>
    /// <exception cref="InvalidOperationException">Thrown when this plate is already in the Mercosur pattern.</exception>
    public PlacaVeiculo ConverterParaPadraoMercosul()
    {
        if (Padrao != PadraoPlacaVeiculo.NacionalAnterior)
        {
            throw new InvalidOperationException("Only previous national plate sequences can be converted algorithmically to the Mercosur pattern.");
        }

        string converted = string.Create(CanonicalLength, Value, static (destination, source) =>
        {
            destination[0] = source[0];
            destination[1] = source[1];
            destination[2] = source[2];
            destination[3] = source[3];
            destination[4] = (char)('A' + (source[4] - '0'));
            destination[5] = source[5];
            destination[6] = source[6];
        });

        return new PlacaVeiculo(converted, PadraoPlacaVeiculo.Mercosul);
    }

    /// <summary>
    /// Parses a supported Brazilian vehicle plate sequence.
    /// </summary>
    /// <param name="value">The plate text.</param>
    /// <returns>A validated vehicle plate.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> is not a supported plate sequence.</exception>
    public static PlacaVeiculo Parse(string value)
    {
        return Parse(value, provider: null);
    }

    /// <inheritdoc />
    public static PlacaVeiculo Parse(string s, IFormatProvider? provider)
    {
        if (s is null)
        {
            throw new FormatException("Vehicle plate must be provided in a supported Brazilian plate format.");
        }

        return Parse(s.AsSpan(), provider);
    }

    /// <inheritdoc />
    public static PlacaVeiculo Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!TryNormalize(s, out string normalized, out PadraoPlacaVeiculo padrao))
        {
            throw new FormatException("Vehicle plate must match ABC1234, ABC-1234, or ABC1D23 using ASCII letters and digits.");
        }

        return new PlacaVeiculo(normalized, padrao);
    }

    /// <summary>
    /// Attempts to parse a supported Brazilian vehicle plate sequence.
    /// </summary>
    /// <param name="value">The plate text.</param>
    /// <param name="result">When successful, contains the vehicle plate.</param>
    /// <returns><see langword="true"/> when the value is a supported plate sequence; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string? value, out PlacaVeiculo result)
    {
        return TryParse(value, provider: null, out result);
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out PlacaVeiculo result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out PlacaVeiculo result)
    {
        if (TryNormalize(s, out string normalized, out PadraoPlacaVeiculo padrao))
        {
            result = new PlacaVeiculo(normalized, padrao);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Determines whether the supplied text matches a supported Brazilian vehicle plate sequence pattern.
    /// </summary>
    /// <param name="value">The plate text to validate.</param>
    /// <returns><see langword="true"/> when the value is a supported plate sequence.</returns>
    public static bool IsValid(string? value)
    {
        return TryParse(value, out _);
    }

    /// <summary>
    /// Returns the canonical seven-character plate sequence.
    /// </summary>
    /// <returns>The canonical plate sequence.</returns>
    public override string ToString()
    {
        return Value;
    }

    /// <summary>
    /// Formats this plate using <c>G</c> for canonical sequence or <c>F</c> for the supported display form.
    /// </summary>
    /// <param name="format"><c>G</c>, <c>F</c>, an empty string, or <see langword="null"/>.</param>
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

        throw new FormatException($"Unsupported vehicle plate format '{format}'. Use 'G' or 'F'.");
    }

    private static bool TryNormalize(ReadOnlySpan<char> input, out string normalized, out PadraoPlacaVeiculo padrao)
    {
        normalized = string.Empty;
        padrao = default;

        Span<char> canonical = stackalloc char[CanonicalLength];
        if (input.Length == CanonicalLength)
        {
            for (int index = 0; index < input.Length; index++)
            {
                if (!TryNormalizeCharacter(input[index], out canonical[index]))
                {
                    return false;
                }
            }
        }
        else if (input.Length == PreviousFormattedLength && input[3] == '-')
        {
            for (int sourceIndex = 0, targetIndex = 0; sourceIndex < input.Length; sourceIndex++)
            {
                if (sourceIndex == 3)
                {
                    continue;
                }

                if (!TryNormalizeCharacter(input[sourceIndex], out canonical[targetIndex]))
                {
                    return false;
                }

                targetIndex++;
            }
        }
        else
        {
            return false;
        }

        if (IsPreviousPattern(canonical))
        {
            normalized = new string(canonical);
            padrao = PadraoPlacaVeiculo.NacionalAnterior;
            return true;
        }

        if (input.Length == CanonicalLength && IsMercosurPattern(canonical))
        {
            normalized = new string(canonical);
            padrao = PadraoPlacaVeiculo.Mercosul;
            return true;
        }

        return false;
    }

    private static bool IsPreviousPattern(ReadOnlySpan<char> value)
    {
        return IsAsciiUpperLetter(value[0]) && IsAsciiUpperLetter(value[1]) && IsAsciiUpperLetter(value[2])
            && IsAsciiDigit(value[3]) && IsAsciiDigit(value[4]) && IsAsciiDigit(value[5]) && IsAsciiDigit(value[6]);
    }

    private static bool IsMercosurPattern(ReadOnlySpan<char> value)
    {
        return IsAsciiUpperLetter(value[0]) && IsAsciiUpperLetter(value[1]) && IsAsciiUpperLetter(value[2])
            && IsAsciiDigit(value[3]) && IsAsciiUpperLetter(value[4]) && IsAsciiDigit(value[5]) && IsAsciiDigit(value[6]);
    }

    private static bool TryNormalizeCharacter(char value, out char normalized)
    {
        if (IsAsciiDigit(value) || IsAsciiUpperLetter(value))
        {
            normalized = value;
            return true;
        }

        if (IsAsciiLowerLetter(value))
        {
            normalized = (char)(value - ('a' - 'A'));
            return true;
        }

        normalized = default;
        return false;
    }

    private static bool IsAsciiDigit(char value)
    {
        return (uint)(value - '0') <= 9;
    }

    private static bool IsAsciiUpperLetter(char value)
    {
        return (uint)(value - 'A') <= 'Z' - 'A';
    }

    private static bool IsAsciiLowerLetter(char value)
    {
        return (uint)(value - 'a') <= 'z' - 'a';
    }
}
