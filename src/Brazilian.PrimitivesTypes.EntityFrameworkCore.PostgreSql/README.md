# Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql

Entity Framework Core + PostgreSQL/Npgsql integration for `Brazilian.PrimitivesTypes`.

This package keeps PostgreSQL persistence concerns outside the domain package and independent from the SQL Server integration.

## Install

```bash
dotnet add package Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql
```

## Model-wide mapping

```csharp
using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;

protected override void ConfigureConventions(
    ModelConfigurationBuilder configurationBuilder)
{
    configurationBuilder.UseBrazilianPrimitiveTypesPostgreSql();
}
```

Or map individual properties explicitly:

```csharp
entity.Property(x => x.Cpf).HasBrazilianCpfPostgreSql();
entity.Property(x => x.Email).HasBrazilianEmailPostgreSql();
entity.Property(x => x.Cep).HasBrazilianCepPostgreSql();
```

The integration persists canonical `Value` strings, applies provider-specific `character varying(n)` metadata, preserves nullable `T?` as SQL `NULL`, and supports strongly typed LINQ equality queries.

`Rg` and `InscricaoEstadual` have explicit context-free and state-aware mappings. A known UF is never silently discarded or inferred.

The library does not create indexes, uniqueness constraints, keys, or perform external cadastral validation.

## Documentation

- [English: Entity Framework Core + PostgreSQL](https://github.com/rodri-oliveira-dev/brazilian-primitives/blob/main/docs/en/entity-framework-core-postgresql.md)
- [Português: Entity Framework Core + PostgreSQL](https://github.com/rodri-oliveira-dev/brazilian-primitives/blob/main/docs/pt-BR/entity-framework-core-postgresql.md)

## Package boundary

```text
Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql
  ├─> Brazilian.PrimitivesTypes
  ├─> Microsoft.EntityFrameworkCore.Relational
  └─> Npgsql.EntityFrameworkCore.PostgreSQL
```

The PostgreSQL package does not reference the SQL Server or Dapper integrations.
