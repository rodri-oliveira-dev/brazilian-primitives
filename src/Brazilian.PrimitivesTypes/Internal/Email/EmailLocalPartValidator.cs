namespace Brazilian.PrimitivesTypes;

internal static class EmailLocalPartValidator
{
    private const int MaxLocalPartLength = 64;

    internal static bool IsValid(ReadOnlySpan<char> value)
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

    private static bool IsAtext(char value)
    {
        return AsciiCharacters.IsLetterOrDigit(value)
            || value is '!' or '#' or '$' or '%' or '&' or '\'' or '*' or '+' or '-' or '/'
                or '=' or '?' or '^' or '_' or '`' or '{' or '|' or '}' or '~';
    }
}
