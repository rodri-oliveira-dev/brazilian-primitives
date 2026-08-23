using System.Diagnostics.CodeAnalysis;
using System.Globalization;

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
    private const int MaxLocalPartLength = 64;
    private const int MaxAddressLength = 254;
    private const int MaxDomainLabelLength = 63;

    private static readonly IdnMapping IdnMapping = new();

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
        if (!TryNormalize(s, out string normalized, out int atIndex))
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
        if (TryNormalize(s, out string normalized, out int atIndex))
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

    private static bool TryNormalize(ReadOnlySpan<char> input, out string normalized, out int atIndex)
    {
        normalized = string.Empty;
        atIndex = -1;

        int separator = FindSingleAt(input);
        if (separator <= 0 || separator == input.Length - 1)
        {
            return false;
        }

        ReadOnlySpan<char> localPart = input[..separator];
        ReadOnlySpan<char> domain = input[(separator + 1)..];
        if (!IsValidLocalPart(localPart) || !TryNormalizeDomain(domain, out string asciiDomain))
        {
            return false;
        }

        if (localPart.Length + 1 + asciiDomain.Length > MaxAddressLength)
        {
            return false;
        }

        normalized = string.Concat(localPart.ToString(), "@", asciiDomain);
        atIndex = localPart.Length;
        return true;
    }

    private static int FindSingleAt(ReadOnlySpan<char> input)
    {
        int atIndex = -1;
        for (int index = 0; index < input.Length; index++)
        {
            if (input[index] != '@')
            {
                continue;
            }

            if (atIndex >= 0)
            {
                return -1;
            }

            atIndex = index;
        }

        return atIndex;
    }

    private static bool IsValidLocalPart(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty || value.Length > MaxLocalPartLength || value[0] == '.' || value[^1] == '.')
        {
            return false;
        }

        bool previousWasDot = false;
        for (int index = 0; index < value.Length; index++)
        {
            char character = value[index];
            if (character == '.')
            {
                if (previousWasDot)
                {
                    return false;
                }

                previousWasDot = true;
                continue;
            }

            if (!IsAtext(character))
            {
                return false;
            }

            previousWasDot = false;
        }

        return true;
    }

    private static bool TryNormalizeDomain(ReadOnlySpan<char> domain, out string asciiDomain)
    {
        asciiDomain = string.Empty;

        if (domain.IsEmpty || domain[0] == '.' || domain[^1] == '.')
        {
            return false;
        }

        string input = domain.ToString();
        string ascii;
        try
        {
            ascii = IdnMapping.GetAscii(input);
        }
        catch (ArgumentException)
        {
            return false;
        }

        asciiDomain = ascii.ToLowerInvariant();
        return IsValidAsciiDomain(asciiDomain.AsSpan());
    }

    private static bool IsValidAsciiDomain(ReadOnlySpan<char> value)
    {
        int labelLength = 0;
        char previous = '\0';
        for (int index = 0; index < value.Length; index++)
        {
            char character = value[index];
            if (character == '.')
            {
                if (labelLength == 0 || labelLength > MaxDomainLabelLength || previous == '-')
                {
                    return false;
                }

                labelLength = 0;
                previous = character;
                continue;
            }

            if (labelLength == 0 && character == '-')
            {
                return false;
            }

            if (!IsAsciiLetterOrDigit(character) && character != '-')
            {
                return false;
            }

            labelLength++;
            previous = character;
        }

        return labelLength is > 0 and <= MaxDomainLabelLength && previous != '-';
    }

    private static bool IsAtext(char value)
    {
        return IsAsciiLetterOrDigit(value)
            || value is '!' or '#' or '$' or '%' or '&' or '\'' or '*' or '+' or '-' or '/'
                or '=' or '?' or '^' or '_' or '`' or '{' or '|' or '}' or '~';
    }

    private static bool IsAsciiLetterOrDigit(char value)
    {
        return IsAsciiDigit(value) || (uint)((value | 0x20) - 'a') <= 'z' - 'a';
    }

    private static bool IsAsciiDigit(char value)
    {
        return (uint)(value - '0') <= 9;
    }
}
