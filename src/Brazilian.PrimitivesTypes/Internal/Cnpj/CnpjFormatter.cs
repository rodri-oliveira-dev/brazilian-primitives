namespace Brazilian.PrimitivesTypes;

internal static class CnpjFormatter
{
    internal static string Format(string value)
    {
        return string.Create(18, value, static (destination, source) =>
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
            destination[10] = '/';
            destination[11] = source[8];
            destination[12] = source[9];
            destination[13] = source[10];
            destination[14] = source[11];
            destination[15] = '-';
            destination[16] = source[12];
            destination[17] = source[13];
        });
    }
}
