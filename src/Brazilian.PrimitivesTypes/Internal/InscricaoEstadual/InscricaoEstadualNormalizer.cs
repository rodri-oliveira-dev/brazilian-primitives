namespace Brazilian.PrimitivesTypes;

internal static class InscricaoEstadualNormalizer
{
    private const int ContextFreeMinimumLength = 8;
    private const int ContextFreeMaximumLength = 14;

    internal static bool TryNormalizeContextFree(ReadOnlySpan<char> input, out string normalized)
    {
        normalized = string.Empty;
        if (input.Length < ContextFreeMinimumLength || input.Length > ContextFreeMaximumLength)
        {
            return false;
        }

        Span<char> canonical = stackalloc char[ContextFreeMaximumLength];
        Span<char> destination = canonical[..input.Length];
        if (!AsciiCharacters.TryCopyDigits(input, destination))
        {
            return false;
        }

        normalized = new string(destination);
        return true;
    }

    internal static bool TryNormalize(ReadOnlySpan<char> input, BrazilianState state, out string normalized)
    {
        normalized = string.Empty;
        if (!InscricaoEstadualStateRules.TryGet(state, out InscricaoEstadualRule rule) || IsIsento(input))
        {
            return false;
        }

        if (TryNormalizeWithLength(input, rule.FirstLength, out normalized))
        {
            return true;
        }

        return rule.SecondLength != 0 && TryNormalizeWithLength(input, rule.SecondLength, out normalized);
    }

    private static bool TryNormalizeWithLength(ReadOnlySpan<char> input, int length, out string normalized)
    {
        normalized = string.Empty;
        if (input.Length != length)
        {
            return false;
        }

        Span<char> canonical = stackalloc char[ContextFreeMaximumLength];
        Span<char> destination = canonical[..length];
        if (!AsciiCharacters.TryCopyDigits(input, destination))
        {
            return false;
        }

        normalized = new string(destination);
        return true;
    }

    private static bool IsIsento(ReadOnlySpan<char> input)
    {
        return input.Equals("ISENTO".AsSpan(), StringComparison.OrdinalIgnoreCase);
    }
}
