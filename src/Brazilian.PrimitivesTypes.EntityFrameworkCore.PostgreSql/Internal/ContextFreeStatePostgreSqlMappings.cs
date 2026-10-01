using Brazilian.PrimitivesTypes;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;

internal static class ContextFreeStatePostgreSqlMappings
{
    public static readonly ScalarPrimitivePostgreSqlMapping<Rg, RgValueConverter> Rg = new(10);
    public static readonly ScalarPrimitivePostgreSqlMapping<InscricaoEstadual, InscricaoEstadualValueConverter> InscricaoEstadual = new(14);
}
