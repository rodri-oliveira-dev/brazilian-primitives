using System.Globalization;

namespace Brazilian.PrimitivesTypes;

internal static class EmailDomainNormalizer
{
    private const int MaxDomainLabelLength = 63;

    private static readonly IdnMapping IdnMapping = new();

    internal static bool TryNormalize(ReadOnlySpan<char> domain, out string asciiDomain)
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

            if (!AsciiCharacters.IsLetterOrDigit(character) && character != '-')
            {
                return false;
            }

            labelLength++;
            previous = character;
        }

        return labelLength is > 0 and <= MaxDomainLabelLength && previous != '-';
    }
}
