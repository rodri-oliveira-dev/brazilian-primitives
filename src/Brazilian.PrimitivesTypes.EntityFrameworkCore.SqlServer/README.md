# Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer

Entity Framework Core + SQL Server integration for `Brazilian.PrimitivesTypes`.

Use this package when your domain model uses Brazilian primitive value objects and your persistence layer needs provider-aware EF Core mappings for SQL Server.

## Install

```bash
dotnet add package Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer
```

The package references `Brazilian.PrimitivesTypes` and `Microsoft.EntityFrameworkCore.SqlServer`.

## Quick start

Your domain model remains strongly typed:

```csharp
using Brazilian.PrimitivesTypes;

public sealed class Customer
{
    public long Id { get; set; }

    public Cpf Cpf { get; set; }

    public Email? Email { get; set; }

    public Cep Cep { get; set; }
}
```

Register the model-wide conventions in your `DbContext`:

```csharp
using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void ConfigureConventions(
        ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.UseBrazilianPrimitiveTypesSqlServer();
    }
}
```

Referencing the package alone does not modify the EF model. The integration is explicitly opt-in.

## Explicit property mappings

You can also configure individual properties:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Customer>(entity =>
    {
        entity.Property(customer => customer.Cpf)
            .HasBrazilianCpfSqlServer();

        entity.Property(customer => customer.Email)
            .HasBrazilianEmailSqlServer();

        entity.Property(customer => customer.Cep)
            .HasBrazilianCepSqlServer();
    });
}
```

## What is stored

The database receives each primitive's canonical `Value`, not the display mask or original input.

Typical mappings use provider-aware `varchar(n)` columns, for example:

```sql
Cpf   VARCHAR(11)  NOT NULL,
Email VARCHAR(254) NULL,
Cep   VARCHAR(8)   NOT NULL
```

CLR nullability is preserved: nullable primitives map naturally to SQL `NULL`.

## RG and State Registration

`Rg` and `InscricaoEstadual` support two explicit persistence strategies:

- **Context-free:** stores only the canonical value.
- **State-aware:** stores both the value and the Brazilian state context.

No state is inferred automatically. This keeps persistence behavior explicit and prevents accidental loss of domain context.

## Migrations and schema

This package provides value conversions and SQL Server mapping metadata. Decisions such as indexes, unique constraints, alternate keys, and aggregate-specific rules remain the responsibility of the application.

## Documentation

- [Complete EF Core + SQL Server guide](https://github.com/rodri-oliveira-dev/brazilian-primitives/blob/main/docs/en/entity-framework-core-sql-server.md)
- [Guia completo em Português do Brasil](https://github.com/rodri-oliveira-dev/brazilian-primitives/blob/main/docs/pt-BR/entity-framework-core-sql-server.md)
- [Repository](https://github.com/rodri-oliveira-dev/brazilian-primitives)

## Related packages

| Package | Purpose |
| --- | --- |
| `Brazilian.PrimitivesTypes` | Core Brazilian value objects |
| `Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer` | EF Core + SQL Server integration |
| `Brazilian.PrimitivesTypes.Dapper.SqlServer` | Dapper + SQL Server integration |
