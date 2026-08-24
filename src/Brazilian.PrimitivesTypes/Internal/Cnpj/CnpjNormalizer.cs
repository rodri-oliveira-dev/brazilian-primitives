namespace Brazilian.PrimitivesTypes;

internal static class CnpjNormalizer
{
    private const int CanonicalLength = 14;
    private const int BaseLength = 12;
    private const int FormattedLength = 18;

    internal static bool TryNormalize(ReadOnlySpan<char> input, out string normalized)
    {
        Span<char> canonical = stackalloc char[CanonicalLength];

        if (!TryExtractCanonical(input, canonical)
            || HasRepeatedCharacters(canonical)
            || !CnpjCheckDigitCalculator.HasValidCheckDigits(canonical))
        {
            normalized = string.Empty;
            return false;
        }

        normalized = new string(canonical);
        return true;
    }

    private static bool TryExtractCanonical(ReadOnlySpan<char> input, Span<char> canonical)
    {
        if (input.Length == CanonicalLength)
        {
            for (int index = 0; index < CanonicalLength; index++)
            {
                if (!TryNormalizeCanonicalCharacter(input[index], index, out canonical[index]))
                {
                    return false;
                }
            }

            return true;
        }

        if (input.Length != FormattedLength || input[2] != '.' || input[6] != '.' || input[10] != '/' || input[15] != '-')
        {
            return false;
        }

        int targetIndex = 0;
        for (int sourceIndex = 0; sourceIndex < FormattedLength; sourceIndex++)
        {
            if (sourceIndex is 2 or 6 or 10 or 15)
            {
                continue;
            }

            if (!TryNormalizeCanonicalCharacter(input[sourceIndex], targetIndex, out canonical[targetIndex]))
            {
                return false;
            }

            targetIndex++;
        }

        return targetIndex == CanonicalLength;
    }

    private static bool TryNormalizeCanonicalCharacter(char value, int index, out char normalized)
    {
        if (index >= BaseLength)
        {
            if (AsciiCharacters.IsDigit(value))
            {
                normalized = value;
                return true;
            }

            normalized = default;
            return false;
        }

        if (AsciiCharacters.IsDigit(value) || AsciiCharacters.IsUpperLetter(value))
        {
            normalized = value;
            return true;
        }

        if (AsciiCharacters.IsLowerLetter(value))
        {
            normalized = (char)(value - ('a' - 'A'));
            return true;
        }

        normalized = default;
        return false;
    }

    private static bool HasRepeatedCharacters(ReadOnlySpan<char> value)
    {
        char first = value[0];
        for (int index = 1; index < value.Length; index++)
        {
            if (value[index] != first)
            {
                return false;
            }
        }

        return true;
    }
}
