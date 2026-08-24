namespace Brazilian.PrimitivesTypes;

internal static class RgFormatter
{
    internal static string Format(string value, BrazilianState state)
    {
        return state switch
        {
            BrazilianState.SaoPaulo => FormatSaoPaulo(value),
            BrazilianState.RioDeJaneiro => FormatRioDeJaneiro(value),
            BrazilianState.MinasGerais => FormatMinasGerais(value),
            BrazilianState.SantaCatarina => FormatSantaCatarina(value),
            _ => value,
        };
    }

    private static string FormatSaoPaulo(string value)
    {
        return string.Create(12, value, static (destination, source) =>
        {
            destination[0] = source[0];
            destination[1] = source[1];
            destination[2] = '.';
            destination[3] = source[2];
            destination[4] = source[3];
            destination[5] = source[4];
            destination[6] = '.';
            destination[7] = source[5];
            destination[8] = source[6];
            destination[9] = source[7];
            destination[10] = '-';
            destination[11] = source[8];
        });
    }

    private static string FormatRioDeJaneiro(string value)
    {
        return string.Create(12, value, static (destination, source) =>
        {
            destination[0] = source[0];
            destination[1] = source[1];
            destination[2] = '.';
            destination[3] = source[2];
            destination[4] = source[3];
            destination[5] = source[4];
            destination[6] = '.';
            destination[7] = source[5];
            destination[8] = source[6];
            destination[9] = source[7];
            destination[10] = '-';
            destination[11] = source[8];
        });
    }

    private static string FormatMinasGerais(string value)
    {
        return string.Create(13, value, static (destination, source) =>
        {
            destination[0] = 'M';
            destination[1] = 'G';
            destination[2] = '-';
            destination[3] = source[0];
            destination[4] = source[1];
            destination[5] = '.';
            destination[6] = source[2];
            destination[7] = source[3];
            destination[8] = source[4];
            destination[9] = '.';
            destination[10] = source[5];
            destination[11] = source[6];
            destination[12] = source[7];
        });
    }

    private static string FormatSantaCatarina(string value)
    {
        return string.Create(11, value, static (destination, source) =>
        {
            destination[0] = source[0];
            destination[1] = source[1];
            destination[2] = source[2];
            destination[3] = '.';
            destination[4] = source[3];
            destination[5] = source[4];
            destination[6] = source[5];
            destination[7] = '.';
            destination[8] = source[6];
            destination[9] = source[7];
            destination[10] = source[8];
        });
    }
}
