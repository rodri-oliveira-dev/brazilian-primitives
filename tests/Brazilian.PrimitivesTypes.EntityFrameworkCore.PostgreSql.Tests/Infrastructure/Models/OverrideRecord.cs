using Brazilian.PrimitivesTypes;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Models;

internal sealed class OverrideRecord
{
    public int Id { get; set; }
    public Email? Email { get; set; }
}
