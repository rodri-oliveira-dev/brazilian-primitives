# Design Principles

Brazilian.Primitives favors predictable domain primitives over permissive string cleanup.

## Value Object Contract

- Construction validates input.
- `Value` is the canonical representation used for equality.
- Leading zeros are preserved by storing identifiers as `string`.
- `Parse` throws `FormatException` for invalid input.
- `TryParse` returns `false` and `default` for invalid input.
- `IsValid` delegates to parsing and does not perform external lookup.
- Default struct instances throw when reading `Value`.

## Strict Input

The library accepts documented shapes only. It rejects unrelated text, loose punctuation, leading or trailing spaces,
Unicode lookalike digits, and masks not explicitly supported by the primitive.

This is intentional: silent sanitization can hide bad input, make logs harder to trust, and create identifiers that a
user did not actually provide.

## Validation Modes

Some primitives are structural only, such as `Cep`, `Ispb`, `CodigoCompe`, `Nit`, and the current
`InscricaoEstadual` matrix. Others include local check-digit algorithms, such as `Cpf`, `Cnpj`, `Cnh`, `Renavam`,
`PisPasep`, `TituloEleitoral`, and `Cns`.

For `Rg`, validation is state-aware: Sao Paulo includes a check digit; other states in the current implementation are
format-only.

## Context Is Part Of Identity

When a Brazilian identifier has no single national rule, the context is explicit:

- `Rg` includes the issuing `BrazilianState`;
- `InscricaoEstadual` includes the state tax context;
- `CpfCnpj`, `ChavePix`, `TelefoneBrasileiro`, and vehicle plates expose discriminators instead of asking callers to
  infer the chosen domain from a string.
