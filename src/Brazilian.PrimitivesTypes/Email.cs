using System.Diagnostics.CodeAnalysis;

namespace Brazilian.PrimitivesTypes;

/// <summary>
/// Represents an email address accepted by the library's strict interoperable syntax subset.
/// </summary>
/// <remarks>
/// Validation is syntactic only. A valid <see cref="Email"/> does not prove mailbox existence, DNS/MX availability,
/// deliverability, ownership, or whether a provider-specific aliasing rule applies. The local part is limited to
/// ASCII dot-atom syntax; internationalized domain names are normalized to ASCII/Punycode.
/// </remarks>
public readonly record struct Email : IParsable<Email>, ISpanParsable<Email>
{
    private readonly string? _value;

    private Email(string value, int atIndex)
    {
        _value = value;
        LocalPartLength = atIndex;
    }

    /// <summary>
    /// Gets the canonical email address, preserving the local part and normalizing only the domain.
    /// </summary>
    public string Value => _value ?? throw new InvalidOperationException("A default email instance does not contain a valid value.");

    /// <summary>
    /// Gets the local part exactly as supplied during parsing.
    /// </summary>
    public string LocalPart => Value[..LocalPartLength];

    /// <summary>
    /// Gets the canonical ASCII domain in lowercase.
    /// </summary>
    public string Domain => Value[(LocalPartLength + 1)..];

    private int LocalPartLength
    {
        get;
    }

    /// <summary>
    /// Parses an email address in the supported strict syntax subset.
    /// </summary>
    /// <param name="value">The email address to parse.</param>
    /// <returns>A validated email value object.</returns>
    /// <exception cref="FormatException">Thrown when <paramref name="value"/> is not a supported email address.</exception>
    public static Email Parse(string value)
    {
        return Parse(value, provider: null);
    }

    /// <inheritdoc />
    public static Email Parse(string s, IFormatProvider? provider)
    {
        if (s is null)
        {
            throw new FormatException("Email must be provided in the supported local-part@domain format.");
        }

        return Parse(s.AsSpan(), provider);
    }

    /// <inheritdoc />
    public static Email Parse(ReadOnlySpan<char> s, IFormatProvider? provider)
    {
        if (!EmailNormalizer.TryNormalize(s, out string normalized, out int atIndex))
        {
            throw new FormatException("Email must use an ASCII dot-atom local part and a valid DNS/IDN domain.");
        }

        return new Email(normalized, atIndex);
    }

    /// <summary>
    /// Attempts to parse an email address in the supported strict syntax subset.
    /// </summary>
    /// <param name="value">The email address to parse.</param>
    /// <param name="result">When successful, contains the validated email address.</param>
    /// <returns><see langword="true"/> when the value is a supported email address; otherwise, <see langword="false"/>.</returns>
    public static bool TryParse(string? value, out Email result)
    {
        return TryParse(value, provider: null, out result);
    }

    /// <inheritdoc />
    public static bool TryParse([NotNullWhen(true)] string? s, IFormatProvider? provider, out Email result)
    {
        if (s is null)
        {
            result = default;
            return false;
        }

        return TryParse(s.AsSpan(), provider, out result);
    }

    /// <inheritdoc />
    public static bool TryParse(ReadOnlySpan<char> s, IFormatProvider? provider, out Email result)
    {
        if (EmailNormalizer.TryNormalize(s, out string normalized, out int atIndex))
        {
            result = new Email(normalized, atIndex);
            return true;
        }

        result = default;
        return false;
    }

    /// <summary>
    /// Determines whether the supplied text is compatible with the supported email syntax.
    /// </summary>
    /// <param name="value">The email text to validate.</param>
    /// <returns><see langword="true"/> when the text can be represented as an <see cref="Email"/>.</returns>
    public static bool IsValid(string? value)
    {
        return TryParse(value, out _);
    }

    /// <summary>
    /// Returns the canonical email address.
    /// </summary>
    /// <returns>The canonical email address.</returns>
    public override string ToString()
    {
        return Value;
    }
}
