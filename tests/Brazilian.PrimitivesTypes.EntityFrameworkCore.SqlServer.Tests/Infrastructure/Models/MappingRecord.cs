using Brazilian.PrimitivesTypes;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Models;

internal sealed class MappingRecord : IRecordWithId
{
    public int Id
    {
        get;
        set;
    }

    public Cpf Cpf
    {
        get;
        set;
    }

    public Email? Email
    {
        get;
        set;
    }

    public Cep Cep
    {
        get;
        set;
    }
}
