using Brazilian.PrimitivesTypes;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

internal static class ContextFreeStatePersistence
{
    public static string GetRgValue(Rg value)
    {
        if (value.HasState)
        {
            throw new InvalidOperationException(
                "A state-aware RG cannot be persisted with the single-column converter because that would discard its UF context.");
        }

        return value.Value;
    }

    public static string GetInscricaoEstadualValue(InscricaoEstadual value)
    {
        if (value.HasState)
        {
            throw new InvalidOperationException(
                "A state-aware Inscricao Estadual cannot be persisted with the single-column converter because that would discard its UF context.");
        }

        return value.Value;
    }
}
