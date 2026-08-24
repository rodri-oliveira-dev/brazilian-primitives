namespace Brazilian.PrimitivesTypes;

internal static class RgNormalizer
{
    private const int ContextFreeMinimumLength = 6;
    private const int ContextFreeMaximumLength = 10;
    private const int SaoPauloCanonicalLength = 9;
    private const int RioDeJaneiroCanonicalLength = 9;
    private const int MinasGeraisCanonicalLength = 8;
    private const int SantaCatarinaCanonicalLength = 9;

    internal static bool TryNormalizeContextFree(ReadOnlySpan<char> input, out string normalized)
    {
        if (input.Length < ContextFreeMinimumLength || input.Length > ContextFreeMaximumLength)
        {
            normalized = string.Empty;
            return false;
        }

        Span<char> canonical = stackalloc char[ContextFreeMaximumLength];
        Span<char> destination = canonical[..input.Length];

        for (int index = 0; index < input.Length; index++)
        {
            char value = input[index];
            bool isFinalSaoPauloStyleCheckDigit = input.Length == SaoPauloCanonicalLength
                && index == input.Length - 1
                && value is 'X' or 'x';

            if (!AsciiCharacters.IsDigit(value) && !isFinalSaoPauloStyleCheckDigit)
            {
                normalized = string.Empty;
                return false;
            }

            destination[index] = isFinalSaoPauloStyleCheckDigit ? 'X' : value;
        }

        normalized = new string(destination);
        return true;
    }

    internal static bool TryNormalize(ReadOnlySpan<char> input, BrazilianState state, out string normalized)
    {
        if (!RgStateRules.TryGet(state, out RgStateRule rule))
        {
            normalized = string.Empty;
            return false;
        }

        bool parsed = rule.MaskKind switch
        {
            RgMaskKind.SaoPaulo => TryNormalizeSaoPaulo(input, out normalized),
            RgMaskKind.RioDeJaneiro => TryNormalizeRioDeJaneiro(input, out normalized),
            RgMaskKind.MinasGerais => TryNormalizeMinasGerais(input, out normalized),
            RgMaskKind.SantaCatarina => TryNormalizeSantaCatarina(input, out normalized),
            _ => TryNormalizeDigits(input, rule.CanonicalLength, out normalized),
        };

        if (!parsed)
        {
            return false;
        }

        return !rule.ValidateSaoPauloCheckDigit || RgSaoPauloCheckDigit.IsValid(normalized);
    }

    private static bool TryNormalizeSaoPaulo(ReadOnlySpan<char> input, out string normalized)
    {
        return input.Length == SaoPauloCanonicalLength
            ? TryNormalizeSaoPauloCanonical(input, out normalized)
            : TryNormalizeSaoPauloFormatted(input, out normalized);
    }

    private static bool TryNormalizeSaoPauloCanonical(ReadOnlySpan<char> input, out string normalized)
    {
        Span<char> canonical = stackalloc char[SaoPauloCanonicalLength];
        if (!AsciiCharacters.TryCopyDigits(input[..^1], canonical[..^1])
            || !RgSaoPauloCheckDigit.TryNormalize(input[^1], out canonical[^1]))
        {
            normalized = string.Empty;
            return false;
        }

        normalized = new string(canonical);
        return true;
    }

    private static bool TryNormalizeSaoPauloFormatted(ReadOnlySpan<char> input, out string normalized)
    {
        if (input.Length != 12 || input[2] != '.' || input[6] != '.' || input[10] != '-')
        {
            normalized = string.Empty;
            return false;
        }

        ReadOnlySpan<int> digitIndexes = [0, 1, 3, 4, 5, 7, 8, 9];
        Span<char> canonical = stackalloc char[SaoPauloCanonicalLength];
        for (int index = 0; index < digitIndexes.Length; index++)
        {
            char value = input[digitIndexes[index]];
            if (!AsciiCharacters.IsDigit(value))
            {
                normalized = string.Empty;
                return false;
            }

            canonical[index] = value;
        }

        if (!RgSaoPauloCheckDigit.TryNormalize(input[^1], out canonical[^1]))
        {
            normalized = string.Empty;
            return false;
        }

        normalized = new string(canonical);
        return true;
    }

    private static bool TryNormalizeRioDeJaneiro(ReadOnlySpan<char> input, out string normalized)
    {
        Span<char> canonical = stackalloc char[RioDeJaneiroCanonicalLength];

        if (input.Length == RioDeJaneiroCanonicalLength)
        {
            if (!AsciiCharacters.TryCopyDigits(input, canonical))
            {
                normalized = string.Empty;
                return false;
            }

            normalized = new string(canonical);
            return true;
        }

        if (input.Length != 12 || input[2] != '.' || input[6] != '.' || input[10] != '-')
        {
            normalized = string.Empty;
            return false;
        }

        int targetIndex = 0;
        for (int sourceIndex = 0; sourceIndex < input.Length; sourceIndex++)
        {
            if (sourceIndex is 2 or 6 or 10)
            {
                continue;
            }

            if (!AsciiCharacters.IsDigit(input[sourceIndex]))
            {
                normalized = string.Empty;
                return false;
            }

            canonical[targetIndex++] = input[sourceIndex];
        }

        normalized = targetIndex == RioDeJaneiroCanonicalLength ? new string(canonical) : string.Empty;
        return targetIndex == RioDeJaneiroCanonicalLength;
    }

    private static bool TryNormalizeMinasGerais(ReadOnlySpan<char> input, out string normalized)
    {
        ReadOnlySpan<char> number = input;

        if (input.StartsWith("MG-", StringComparison.OrdinalIgnoreCase))
        {
            number = input[3..];
        }
        else if (input.StartsWith("M-", StringComparison.OrdinalIgnoreCase))
        {
            number = input[2..];
        }

        Span<char> canonical = stackalloc char[MinasGeraisCanonicalLength];

        if (number.Length == MinasGeraisCanonicalLength)
        {
            if (!AsciiCharacters.TryCopyDigits(number, canonical))
            {
                normalized = string.Empty;
                return false;
            }

            normalized = new string(canonical);
            return true;
        }

        if (number.Length != 10 || number[2] != '.' || number[6] != '.')
        {
            normalized = string.Empty;
            return false;
        }

        int targetIndex = 0;
        for (int sourceIndex = 0; sourceIndex < number.Length; sourceIndex++)
        {
            if (sourceIndex is 2 or 6)
            {
                continue;
            }

            if (!AsciiCharacters.IsDigit(number[sourceIndex]))
            {
                normalized = string.Empty;
                return false;
            }

            canonical[targetIndex++] = number[sourceIndex];
        }

        normalized = targetIndex == MinasGeraisCanonicalLength ? new string(canonical) : string.Empty;
        return targetIndex == MinasGeraisCanonicalLength;
    }

    private static bool TryNormalizeSantaCatarina(ReadOnlySpan<char> input, out string normalized)
    {
        Span<char> canonical = stackalloc char[SantaCatarinaCanonicalLength];

        if (input.Length == SantaCatarinaCanonicalLength)
        {
            if (!AsciiCharacters.TryCopyDigits(input, canonical))
            {
                normalized = string.Empty;
                return false;
            }

            normalized = new string(canonical);
            return true;
        }

        if (input.Length != 11 || input[3] != '.' || input[7] != '.')
        {
            normalized = string.Empty;
            return false;
        }

        int targetIndex = 0;
        for (int sourceIndex = 0; sourceIndex < input.Length; sourceIndex++)
        {
            if (sourceIndex is 3 or 7)
            {
                continue;
            }

            if (!AsciiCharacters.IsDigit(input[sourceIndex]))
            {
                normalized = string.Empty;
                return false;
            }

            canonical[targetIndex++] = input[sourceIndex];
        }

        normalized = targetIndex == SantaCatarinaCanonicalLength ? new string(canonical) : string.Empty;
        return targetIndex == SantaCatarinaCanonicalLength;
    }

    private static bool TryNormalizeDigits(ReadOnlySpan<char> input, int expectedLength, out string normalized)
    {
        if (input.Length != expectedLength)
        {
            normalized = string.Empty;
            return false;
        }

        Span<char> canonical = stackalloc char[ContextFreeMaximumLength];
        Span<char> destination = canonical[..expectedLength];
        if (!AsciiCharacters.TryCopyDigits(input, destination))
        {
            normalized = string.Empty;
            return false;
        }

        normalized = new string(destination);
        return true;
    }
}
