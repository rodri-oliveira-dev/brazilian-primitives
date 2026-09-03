# ADR 0002: Use canonical string representations for identifiers

- **Status:** Accepted
- **Date:** 2026-09-02

## Context

Several values handled by this library look numeric but are not numbers.

CPF, CNPJ, CEP, COMPE codes, and other identifiers can contain meaningful leading zeros. Treating them as numeric values would make those zeros easy to lose and would imply arithmetic behavior that is not part of the domain.

The library also accepts some formatted input, so keeping the exact text supplied by the caller would make equality and persistence depend on presentation.

## Decision

A valid primitive exposes one canonical `string` value.

Parsing may accept the documented formatted forms, but once construction succeeds the value used for equality and persistence is deterministic. Formatting is presentation; the canonical value is identity.

When a primitive also requires explicit context, that context participates in identity separately from the canonical string.

## Alternative

The main alternative was to store the original input and normalize only when formatting or persisting. That would preserve what the caller typed, but it would also allow equivalent identifiers to carry different internal representations.

Numeric storage was ruled out because losing a leading zero is a correctness bug, not a formatting difference.

## Consequences

- Leading zeros are preserved without special cases.
- `529.982.247-25` and its accepted unformatted equivalent resolve to the same CPF value.
- Persistence adapters have one stable value to store.
- Formatting rules can change without changing the identity contract.
