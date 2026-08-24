namespace Brazilian.PrimitivesTypes;

internal static class PixRandomKeyNormalizer
{
    private const int EpiLength = 36;

    internal static bool TryNormalize(ReadOnlySpan<char> input, out string normalized)
    {
        normalized = string.Empty;
        if (input.Length != EpiLength)
        {
            return false;
        }

        Span<char> canonical = stackalloc char[EpiLength];
        for (int index = 0; index < input.Length; index++)
        {
            char character = input[index];
            if (index is 8 or 13 or 18 or 23)
            {
                if (character != '-')
                {
                    return false;
                }

                canonical[index] = character;
                continue;
            }

            if (!TryNormalizeHex(character, out canonical[index]))
            {
                return false;
            }
        }

        normalized = new string(canonical);
        return true;
    }

    private static bool TryNormalizeHex(char value, out char normalized)
    {
        if ((uint)(value - '0') <= 9 || (uint)(value - 'a') <= 'f' - 'a')
        {
            normalized = value;
            return true;
        }

        if ((uint)(value - 'A') <= 'F' - 'A')
        {
            normalized = (char)(value + ('a' - 'A'));
            return true;
        }

        normalized = default;
        return false;
    }
}
