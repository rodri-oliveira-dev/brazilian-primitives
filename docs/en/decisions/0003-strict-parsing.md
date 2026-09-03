# ADR 0003: Prefer strict parsing over silent sanitization

- **Status:** Accepted
- **Date:** 2026-09-02

## Context

It is tempting for an identifier library to remove every character it does not recognize and validate whatever remains.

That behavior is convenient until input such as `"CPF: 529.982.247-25"` is silently treated as if the caller had supplied a supported CPF format. At that point the library is no longer only parsing an identifier; it is guessing which part of arbitrary text the caller intended to use.

That also makes bad upstream data harder to spot.

## Decision

Parsing is intentionally strict.

Each primitive accepts only the shapes documented for that type. Punctuation is normalized when that formatted representation is part of the contract, but the library does not search free text for a value or repair unsupported input.

`Parse`, `TryParse`, and `IsValid` follow the same rules.

## What we are not doing

We are not adding a generic “strip everything except digits/letters” preprocessing step. Callers that need to clean user-entered text can do so before constructing a primitive, where that policy is visible to the application.

## Consequences

Invalid data tends to fail closer to where it entered the system, instead of being silently transformed into a different value.

The trade-off is deliberate: accepting a new input shape requires an explicit library change and compatibility decision rather than becoming valid accidentally through sanitization.
