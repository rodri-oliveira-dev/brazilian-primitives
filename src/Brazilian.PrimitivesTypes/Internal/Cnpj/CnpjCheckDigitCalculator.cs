namespace Brazilian.PrimitivesTypes;

internal static class CnpjCheckDigitCalculator
{
    private const int BaseLength = 12;

    internal static bool HasValidCheckDigits(ReadOnlySpan<char> value)
    {
        int firstCheckDigit = Calculate(value[..BaseLength]);
        if (value[BaseLength] - '0' != firstCheckDigit)
        {
            return false;
        }

        int secondCheckDigit = Calculate(value[..(BaseLength + 1)]);
        return value[BaseLength + 1] - '0' == secondCheckDigit;
    }

    private static int Calculate(ReadOnlySpan<char> value)
    {
        int sum = 0;
        for (int index = 0; index < value.Length; index++)
        {
            int weight = ((value.Length - index - 1) % 8) + 2;
            sum += GetVerificationValue(value[index]) * weight;
        }

        int remainder = sum % 11;
        return remainder < 2 ? 0 : 11 - remainder;
    }

    private static int GetVerificationValue(char value)
    {
        // Receita Federal defines the value as the uppercase ASCII code minus 48.
        return value - '0';
    }
}
