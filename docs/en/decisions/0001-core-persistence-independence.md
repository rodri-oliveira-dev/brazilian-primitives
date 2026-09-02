# ADR 0001: Keep Core independent from persistence frameworks

- **Status:** Accepted
- **Date:** 2026-09-02

## Context

Brazilian identifiers are domain concepts. Consumers may use EF Core, Dapper, another persistence technology, or no persistence framework at all. Putting ORM or SQL-provider dependencies in Core would impose infrastructure choices on every consumer.

## Decision

`Brazilian.PrimitivesTypes` remains persistence-framework agnostic. Persistence integrations are separate adapter packages. An adapter may depend on Core and its own persistence framework, but Core must not reference adapters, EF Core, Dapper, or SQL client libraries. Adapters must not reference each other.

## Alternatives considered

- Put EF Core and Dapper support directly in Core.
- Ship Core and all persistence integrations in one package.

Both were rejected because they increase transitive dependencies and weaken the domain/infrastructure boundary.

## Consequences

- Core can be consumed without persistence dependencies.
- Persistence integrations can evolve independently.
- New adapters can be added without reversing the Core dependency direction.
- CI must enforce the boundary to prevent accidental coupling.
