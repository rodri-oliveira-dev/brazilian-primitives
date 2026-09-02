# ADR 0002: Use canonical string representations for identifiers

- **Status:** Accepted
- **Date:** 2026-09-02

## Context

Brazilian identifiers often contain meaningful leading zeros and are identifiers rather than quantities. Numeric storage can erase those zeros and suggests arithmetic semantics that the domain does not have.

## Decision

Domain primitives use a canonical `string` representation as their value contract. Parsing may accept explicitly documented formatted forms, but successful construction produces one deterministic canonical value. Equality uses that normalized value plus explicit context when required.

## Alternatives considered

- Store identifiers as numeric types.
- Preserve whichever textual representation the caller supplied.

Numeric storage was rejected because it can lose significant zeros. Preserving arbitrary input text was rejected because it weakens deterministic equality and persistence.

## Consequences

- Leading zeros are preserved.
- Equality and persistence use a stable representation.
- Formatting remains separate from identity.
- Persistence adapters consume the canonical domain value rather than inventing provider-specific domain representations.
