# ADR 0003: Prefer strict parsing over silent sanitization

- **Status:** Accepted
- **Date:** 2026-09-02

## Context

Permissive sanitization can turn malformed or unrelated text into an apparently valid identifier. That hides input-quality problems and makes logs and caller intent harder to trust.

## Decision

Parsing accepts only documented shapes and normalization rules. Punctuation is removed only when that exact formatted shape is part of the public contract. The library does not search arbitrary text for digits or silently repair unsupported input.

`Parse`, `TryParse`, and `IsValid` share the same validation semantics.

## Alternatives considered

- Strip all non-alphanumeric characters before validation.
- Attempt best-effort correction of malformed values.

Both were rejected because convenience would reduce predictability.

## Consequences

- Invalid input fails early and predictably.
- Accepted values better reflect what the caller actually supplied.
- New accepted formats require an explicit compatibility decision.
- Parsing remains deterministic and testable.
