using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

internal static class SqlServerValueConverterMappingHints
{
    public static ConverterMappingHints Ascii(int size) => new(size: size, unicode: false);
}
