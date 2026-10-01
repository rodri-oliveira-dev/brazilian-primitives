using Brazilian.PrimitivesTypes;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Models;

internal sealed class Customer
{
    public long Id { get; set; }
    public Cpf Cpf { get; set; }
    public Email? Email { get; set; }
    public Cep Cep { get; set; }
}
