namespace Brazilian.PrimitivesTypes;

internal static class EmailNormalizer
{
    private const int MaxAddressLength = 254;

    internal static bool TryNormalize(ReadOnlySpan<char> input, out string normalized, out int atIndex)
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
        if (!EmailLocalPartValidator.IsValid(localPart) || !EmailDomainNormalizer.TryNormalize(domain, out string asciiDomain))
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
}
