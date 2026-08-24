using Brazilian.PrimitivesTypes;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Models;

internal sealed class OverrideRecord : IRecordWithId
{
    public int Id
    {
        get;
        set;
    }

    public Email? Email
    {
        get;
        set;
    }
}
