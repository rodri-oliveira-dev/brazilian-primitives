# ADR 0001: Keep Core independent from persistence frameworks

- **Status:** Accepted
- **Date:** 2026-09-02

## Context

The repository started with the primitives themselves and later gained separate integrations for Entity Framework Core and Dapper.

At that point there was a choice to make: persistence support could either become part of `Brazilian.PrimitivesTypes`, or the core package could stay focused on domain behavior while integrations lived around it.

The second option matches how the packages are already consumed. A project that only needs `Cpf`, `Cnpj`, `Cep`, or `ChavePix` should not take an ORM or SQL client dependency just because another consumer uses one.

## Decision

`Brazilian.PrimitivesTypes` remains the persistence-agnostic core.

EF Core and Dapper support stay in their own adapter packages:

- `Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer`
- `Brazilian.PrimitivesTypes.Dapper.SqlServer`

Adapters may reference Core and the framework they integrate with. Core must not reference them, and the adapters must not reference each other.

## Alternatives considered

We considered putting persistence support directly in Core, and also shipping all three concerns in one package. Both make installation simpler on paper, but they also make infrastructure dependencies unavoidable for consumers that do not need them.

## Consequences

- Core stays small and usable on its own.
- Persistence integrations can evolve without changing the Core dependency direction.
- Adding a future adapter does not require turning Core into a persistence abstraction layer.
- The dependency boundary is checked in tests so it does not rely only on convention.
