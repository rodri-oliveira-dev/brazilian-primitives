using System.Diagnostics.CodeAnalysis;

namespace Brazilian.PrimitivesTypes;

/// <summary>
/// Represents a value that can be either a CPF or a CNPJ, preserving the selected domain explicitly.
/// </summary>
/// <remarks>
/// This value object delegates validation, normalization, formatting, equality inputs, and check-digit rules to
/// <see cref="Cpf"/> and <see cref="Cnpj"/>. A valid <see cref="CpfCnpj"/> does not prove Receita Federal existence,
/// cadastral status, ownership, or fiscal authorization.
/// </remarks>
public readonly record struct CpfCnpj : IParsable<CpfCnpj>, ISpanParsable<CpfCnpj>, IFormattable
{
    private readonly string? _value;
    private readonly string? _formatted;

    private CpfCnpj(string value, string formatted, TipoCpfCnpj tipo)
    {
        _value = value;
        _formatted = formatted;
        Tipo = tipo;
    }

    /// <summary>
    /// Gets the canonical CPF or CNPJ value delegated from the underlying primitive.
    /// </summary>
    public string Value => _value ?? throw new InvalidOperationException("A default CPF/CNPJ instance does not contain a valid value.");

    /// <summary>
    /// Gets the canonical formatted representation delegated from the underlying primitive.
    /// </summary>
    public string Formatted => _formatted ?? throw new InvalidOperationException("A default CPF/CNPJ instance does not contain a valid value.");

    /// <summary>
    /// Gets the specific tax registration type represented by this instance.
    /// </summary>
    public TipoCpfCnpj Tipo
    {
        get;
    }

    /// <summary>
    /// Creates a CPF/CNPJ union value from a validated CPF.
    /// </summary>
    /// <param name="cpf">The CPF value object.</param>
    /// <returns>A CPF/CNPJ value with <see cref="Tipo"/> set to <see cref="TipoCpfCnpj.Cpf"/>.</returns>
    public static CpfCnpj From(Cpf cpf)
    {
        return new CpfCnpj(cpf.Value, cpf.Formatted, TipoCpfCnpj.Cpf);
    }

    /// <summary>
    /// Creates a CPF/CNPJ union value from a validated CNPJ.
    /// </summary>
    /// <param name="cnpj">The CNPJ value object.</param>
    /// <returns>A CPF/CNPJ value with <see cref="Tipo"/> set to <see cref="TipoCpfCnpj.Cnpj"/>.</returns>
    public static CpfCnpj From(Cnpj cnpj)
    {
        return new CpfCnpj(cnpj.Value, cnpj.Formatted, TipoCpfCnpj.Cnpj);
    }

    /// <summary>
    /// Attempts to recover the underlying CPF.
    /// </summary>
    /// <param name="cpf">When successful, contains the CPF.</param>
    /// <returns><see langword="true"/> when this instance represents a CPF.</returns>
    public bool TryGetCpf(out Cpf cpf)
    {
        if (Tipo == TipoCpfCnpj.Cpf)
        {
            cpf = Cpf.Parse(Value, provider: null);
            return true;
        }

        cpf = default;
        return false;
    }

    /// <summary>
    /// Attempts to recover the underlying CNPJ.
    /// </summary>
    /// <param name="cnpj">When successful, contains the CNPJ.</param>
    /// <returns><see langword="true"/> when this instance represents a CNPJ.</returns>
    public bool TryGetCnpj(out Cnpj cnpj)
    {
        if (Tipo == TipoCpfCnpj.Cnpj)
        {
            cnpj = Cnpj.Parse(Value, provider: null);
            return true;
        }

        cnpj = default;
        return false;
    }

    /// <summary>
    /// Parses a value accepted by either <see cref="Cpf"/> or <see cref="Cnpj"/>.
    /// </summary>
    /// <param name="value">The CPF or CNPJ text.</param>
    /// <returns>A validated CPF/CNPJ value object.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> is not a valid CPF or CNPJ, or is ambiguous.</exception>
    public static CpfCnpj Parse(string value)
    {
        return Parse(value, provider: null);
    }

    /// <inheritdoc />
    public static CpfCnpj Parse(string s, IFormatProvider? provider)
    {
        if (s is null)
        {
            throw new FormatException("CPF/CNPJ must be provided in a supported CPF or CNPJ format.");
        }

        return Parse(s.AsSpan(), provider);
    }

    /// <inheritdoc />
    public static CpfCnpj Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!TryParse(s, provider, out CpfCnpj result))
        {
            throw new FormatException("CPF/CNPJ must be a valid CPF or CNPJ representation.");
        }

        return result;
    }

    /// <summary>
    /// Attempts to parse a value accepted by either <see cref="Cpf"/> or <see cref="Cnpj"/>.
    /// </summary>
    /// <param name="value">The CPF or CNPJ text.</param>
    /// <param name="result">When successful, contains the CPF/CNPJ value.</param>
    /// <returns><see langword="true"/> when the value is a valid CPF or CNPJ; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string? value, out CpfCnpj result)
    {
        return TryParse(value, provider: null, out result);
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out CpfCnpj result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out CpfCnpj result)
    {
        bool isCpf = Cpf.TryParse(s, provider, out Cpf cpf);
        bool isCnpj = Cnpj.TryParse(s, provider, out Cnpj cnpj);

        if (isCpf == isCnpj)
        {
            result = default;
            return false;
        }

        result = isCpf ? From(cpf) : From(cnpj);
        return true;
    }

    /// <summary>
    /// Determines whether the supplied text can be represented as either a CPF or CNPJ.
    /// </summary>
    /// <param name="value">The text to validate.</param>
    /// <returns><see langword="true"/> when the value can be represented as CPF or CNPJ.</returns>
    public static bool IsValid(string? value)
    {
        return TryParse(value, out _);
    }

    /// <summary>
    /// Returns the canonical unmasked CPF or CNPJ value.
    /// </summary>
    /// <returns>The canonical value.</returns>
    public override string ToString()
    {
        return Value;
    }

    /// <summary>
    /// Formats this value using <c>G</c> for canonical unmasked representation or <c>F</c> for the delegated mask.
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

        throw new FormatException($"Unsupported CPF/CNPJ format '{format}'. Use 'G' or 'F'.");
    }
}
