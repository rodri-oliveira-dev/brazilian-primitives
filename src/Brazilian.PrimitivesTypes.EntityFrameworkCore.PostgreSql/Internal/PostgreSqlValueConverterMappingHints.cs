using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;

internal static class PostgreSqlValueConverterMappingHints
{
    public static ConverterMappingHints String(int size) => new(size: size);
}
