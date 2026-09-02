# ADR 0004: Make required domain context part of identity

- **Status:** Accepted
- **Date:** 2026-09-02

## Context

Some Brazilian identifiers do not have one national validation rule. Their meaning or validation depends on explicit context such as the issuing state. Inferring that context from a string is ambiguous.

## Decision

When context changes the meaning or validation contract of a primitive, that context is represented explicitly and participates in identity where required.

Examples include state-aware `Rg` and `InscricaoEstadual`. Composite primitives such as `CpfCnpj`, `ChavePix`, and `TelefoneBrasileiro` expose discriminators rather than requiring callers to infer the selected domain from the canonical string.

## Alternatives considered

- Infer state or subtype from value shape.
- Store only the textual value and make callers track required context separately.

Both were rejected when context is necessary to preserve domain meaning.

## Consequences

- Domain meaning stays explicit.
- Equality can distinguish values whose required context differs.
- Persistence adapters must document whether they preserve context or operate in value-only mode.
- Unsupported inference is avoided.
