namespace Brazilian.Primitives;

/// <summary>
/// Centralizes the current Brazilian geographic area codes (Códigos Nacionais / DDDs) assigned by Anatel.
/// </summary>
/// <remarks>
/// This component is intentionally internal so phone value objects can share the official geographic-area rule
/// without exposing allocation details as public API. The current set is cross-checked against Anatel's Código
/// Nacional publication and the official SMP numbering table:
/// https://www.gov.br/anatel/pt-br/regulado/numeracao/codigos-nacionais
/// https://www.gov.br/anatel/pt-br/regulado/numeracao/tabela-servico-movel-celular
/// </remarks>
internal static class BrazilianAreaCode
{
    public static bool IsValid(ReadOnlySpan<char> value)
    {
        if (value.Length != 2 || !IsAsciiDigit(value[0]) || !IsAsciiDigit(value[1]))
        {
            return false;
        }

        int code = ((value[0] - '0') * 10) + (value[1] - '0');
        return code switch
        {
            >= 11 and <= 19 => true,
            21 or 22 or 24 or 27 or 28 => true,
            31 or 32 or 33 or 34 or 35 or 37 or 38 => true,
            >= 41 and <= 49 => true,
            51 or 53 or 54 or 55 => true,
            >= 61 and <= 69 => true,
            71 or 73 or 74 or 75 or 77 or 79 => true,
            >= 81 and <= 89 => true,
            >= 91 and <= 99 => true,
            _ => false,
        };
    }

    private static bool IsAsciiDigit(char value)
    {
        return (uint)(value - '0') <= 9;
    }
}
