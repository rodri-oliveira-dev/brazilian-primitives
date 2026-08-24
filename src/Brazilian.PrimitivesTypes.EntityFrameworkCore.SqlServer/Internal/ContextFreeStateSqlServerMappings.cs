using Brazilian.PrimitivesTypes;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

internal static class ContextFreeStateSqlServerMappings
{
    public static readonly ScalarPrimitiveSqlServerMapping<Rg, RgValueConverter> Rg = new(10);
    public static readonly ScalarPrimitiveSqlServerMapping<InscricaoEstadual, InscricaoEstadualValueConverter> InscricaoEstadual = new(14);
}
