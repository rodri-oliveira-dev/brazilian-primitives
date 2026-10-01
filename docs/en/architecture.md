# Architecture

[English](architecture.md) | [Português (Brasil)](../pt-BR/architecture.md)

Brazilian Primitives separates domain modeling from persistence infrastructure.

## Package boundaries

```text
                              Application
                                  |
                                  v
                       Brazilian.PrimitivesTypes
                           Domain primitives
                    ^             ^              ^
                    |             |              |
          +---------+      +------+-------+      +----------+
          |                |              |                 |
EF Core + SQL Server   EF Core + PostgreSQL      Dapper + SQL Server
      adapter                 adapter                    adapter
```

The dependency direction is always toward `Brazilian.PrimitivesTypes`.

- **Brazilian.PrimitivesTypes** owns immutable value objects, canonical representations, parsing, formatting, local validation, equality, and domain context.
- **Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer** contains EF Core/SQL Server persistence behavior.
- **Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql** contains EF Core/PostgreSQL/Npgsql persistence behavior.
- **Brazilian.PrimitivesTypes.Dapper.SqlServer** contains Dapper/SQL Server persistence behavior.

Adapters may depend on Core and their own persistence framework/provider. They must not depend on each other. In particular, the PostgreSQL EF Core adapter does not reference the SQL Server EF Core adapter, and neither provider-specific package leaks into Core.

## Architectural rules

1. Core remains persistence-framework agnostic.
2. EF Core, Dapper, SQL clients, Npgsql, migrations, schemas, and provider-specific behavior stay outside Core.
3. Persistence adapters must not redefine domain semantics.
4. Adapters persist the canonical value exposed by the primitive rather than creating a second domain representation.
5. Persistence adapters remain independently installable and do not reference each other.
6. Provider-specific relational metadata stays in the corresponding adapter instead of being copied across providers.
7. Primitive validation remains local and deterministic; package architecture does not introduce network or registry dependencies.

## Architecture fitness functions

The repository treats these boundaries as executable constraints. Tests verify that Core does not reference Dapper, Entity Framework Core, SQL client, or Npgsql assemblies. Provider-specific tests also verify that the PostgreSQL adapter does not reference SQL Server or Dapper integrations.

Package verification complements these tests by validating the expected NuGet dependency graph in CI, including the PostgreSQL package's Core, EF Core Relational, and Npgsql provider dependencies.

## Decisions

Durable decisions are recorded as [Architecture Decision Records](decisions/README.md):

- [ADR 0001 — Keep Core independent from persistence frameworks](decisions/0001-core-persistence-independence.md)
- [ADR 0002 — Use canonical string representations for identifiers](decisions/0002-canonical-string-representation.md)
- [ADR 0003 — Prefer strict parsing over silent sanitization](decisions/0003-strict-parsing.md)
- [ADR 0004 — Make required domain context part of identity](decisions/0004-context-as-identity.md)

See also [Design principles](design-principles.md).
