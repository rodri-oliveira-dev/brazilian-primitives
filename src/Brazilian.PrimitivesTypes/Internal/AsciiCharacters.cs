namespace Brazilian.PrimitivesTypes;

internal static class AsciiCharacters
{
    internal static bool IsDigit(char value)
    {
        return (uint)(value - '0') <= 9;
    }

    internal static bool IsUpperLetter(char value)
    {
        return (uint)(value - 'A') <= 'Z' - 'A';
    }

    internal static bool IsLowerLetter(char value)
    {
        return (uint)(value - 'a') <= 'z' - 'a';
    }

    internal static bool IsLetterOrDigit(char value)
    {
        return IsDigit(value) || (uint)((value | 0x20) - 'a') <= 'z' - 'a';
    }

    internal static bool TryCopyDigits(ReadOnlySpan<char> source, Span<char> destination)
    {
        if (source.Length != destination.Length)
        {
            return false;
        }

        for (int index = 0; index < source.Length; index++)
        {
            if (!IsDigit(source[index]))
            {
                return false;
            }

            destination[index] = source[index];
        }

        return true;
    }
}
