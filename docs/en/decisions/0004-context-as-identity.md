# ADR 0004: Make required domain context part of identity

- **Status:** Accepted
- **Date:** 2026-09-02

## Context

Not every Brazilian identifier can be understood from the identifier text alone.

`Rg` and `InscricaoEstadual` are the clearest cases in this repository: validation rules can depend on the issuing state. For RG, São Paulo currently has check-digit validation while other supported states may be format-only. Guessing the state from the value would turn an implementation heuristic into domain data.

The persistence integrations exposed the same issue from another angle. The EF Core adapter can preserve state-aware mappings, while the Dapper integration currently documents `Rg` and `InscricaoEstadual` as value-only scenarios.

## Decision

When context is required to preserve the meaning or validation contract of a primitive, that context is explicit.

For state-aware identifiers, the state is not inferred from the string. Composite primitives such as `CpfCnpj`, `ChavePix`, and `TelefoneBrasileiro` likewise expose a discriminator instead of asking consumers to rediscover the selected subtype from the canonical value.

## Alternatives considered

Keeping only the string and requiring callers to track context separately would make the type easier to construct, but easier to misuse as well. Inferring context from value shape was rejected because there is no reliable general rule for doing so.

## Consequences

- Two identical-looking values can remain distinct when their required domain context differs.
- Validation code does not need hidden state or heuristics.
- Persistence adapters must be explicit about whether they preserve context.
- Value-only persistence remains possible where documented, but it is not presented as equivalent to state-aware persistence.
