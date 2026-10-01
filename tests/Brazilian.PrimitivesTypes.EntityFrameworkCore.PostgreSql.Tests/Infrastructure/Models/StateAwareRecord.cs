using Brazilian.PrimitivesTypes;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Models;

internal sealed class StateAwareRecord
{
    public int Id
    {
        get;
        set;
    }
    public Rg Rg
    {
        get;
        set;
    }
    public Rg? OptionalRg
    {
        get;
        set;
    }
    public InscricaoEstadual InscricaoEstadual
    {
        get;
        set;
    }
    public InscricaoEstadual? OptionalInscricaoEstadual
    {
        get;
        set;
    }
}
