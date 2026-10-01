# Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql

Entity Framework Core + PostgreSQL provider boundary for `Brazilian.PrimitivesTypes`.

This package keeps PostgreSQL/Npgsql persistence concerns outside the core domain package and independent from the existing SQL Server integration.

## Package boundary

The dependency direction is intentionally one-way:

```text
Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql
  ├─> Brazilian.PrimitivesTypes
  ├─> Microsoft.EntityFrameworkCore.Relational
  └─> Npgsql.EntityFrameworkCore.PostgreSQL
```

The provider targets .NET 10 and uses the repository's Entity Framework Core 10 baseline.

## Current scope

The package currently establishes:

- the dedicated PostgreSQL project/package/namespace;
- Npgsql and EF Core relational dependencies;
- Central Package Management and locked restore;
- NuGet metadata, README, icon, XML documentation, symbols, Source Link, and package validation;
- scalar value converters for canonical single-column persistence;
- context-free and state-aware PostgreSQL persistence for RG and Inscricao Estadual, preserving optional UF context;
- intrinsic PostgreSQL `character varying(n)` metadata for supported scalar primitives;
- real PostgreSQL Testcontainers infrastructure and relational round-trip coverage.

Model-wide conventions, fluent mapping extensions, and end-to-end consumer documentation remain follow-up work in roadmap #52.

## Namespace

Provider-specific APIs belong to:

```csharp
Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql
```

PostgreSQL behavior must not be added to `Brazilian.PrimitivesTypes` or to `Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer`.

## Repository

https://github.com/rodri-oliveira-dev/brazilian-primitives
