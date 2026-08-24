namespace Brazilian.PrimitivesTypes;

internal static class RgSaoPauloCheckDigit
{
    private const int CanonicalLength = 9;

    internal static bool TryNormalize(char value, out char normalized)
    {
        if (AsciiCharacters.IsDigit(value))
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

    internal static bool IsValid(string canonical)
    {
        int sum = 0;
        for (int index = 0; index < CanonicalLength - 1; index++)
        {
            int weight = 9 - index;
            sum += (canonical[index] - '0') * weight;
        }

        int remainder = sum % 11;
        char expected = remainder == 10 ? 'X' : (char)('0' + remainder);
        return canonical[^1] == expected;
    }
}
