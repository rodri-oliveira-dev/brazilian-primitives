# RG

RG is a legacy state-issued Brazilian identity document. `Rg` supports both context-free and state-aware usage because
many legacy databases store the identifier without a reliable issuing UF.

## Contract

| Aspect | Behavior |
| --- | --- |
| Canonical value | unmasked legacy text |
| Context-free mode | 6 to 10 characters, ASCII digits, with optional final `X` only for a 9-character value |
| Missing state | `State == BrazilianState.Unknown` and `HasState == false` |
| State-aware mode | explicit `BrazilianState`; state-specific structure and known checksum rules apply |
| Sao Paulo | 8 digits plus numeric or `X` check digit; mask `12.030.001-1` |
| Rio de Janeiro | 9 digits; optional mask `12.345.678-9`; format-only |
| Minas Gerais | 8 digits; accepts `MG-` and historical `M-` prefixes; format-only |
| Santa Catarina | 9 digits; optional mask `123.456.789`; format-only |
| Other states | documented digit length only |

## Context-free usage

```csharp
Rg rg = Rg.Parse("123456789");

Console.WriteLine(rg.Value);    // 123456789
Console.WriteLine(rg.State);    // Unknown
Console.WriteLine(rg.HasState); // False
```

Context-free validation is deliberately structural and format-only. It does not infer a UF, a state-specific display
mask, or a state-specific checksum. For example, text that is structurally acceptable without a state can still be
rejected when parsed explicitly as a Sao Paulo RG because the Sao Paulo check digit is then enforced.

## State-aware usage

```csharp
Rg rg = Rg.Parse("12.030.001-1", BrazilianState.SaoPaulo);

Console.WriteLine(rg.Value);     // 120300011
Console.WriteLine(rg.Formatted); // 12.030.001-1
Console.WriteLine(rg.State);     // SaoPaulo
Console.WriteLine(rg.HasState);  // True
```

Equality preserves context. A context-free RG is not equal to the same canonical text with a known state, and the same
text issued in different states is not the same `Rg`.

## Entity Framework Core SQL Server

For context-free values, `RgValueConverter` persists only the canonical value in one `varchar(10)` column. The converter
rejects state-aware instances instead of silently discarding their UF.

For state-aware values, use `RgStateAwareSqlServerMapping` with an EF Core complex property. It persists the canonical RG
value and the UF in separate columns; the UF uses a stable two-letter code such as `SP` or `RJ`.

Property nullability is independent from state context: `Rg? == null` means the identifier is absent, while a non-null
`Rg` with `State == BrazilianState.Unknown` means the identifier is present but the issuing UF was not supplied.

This type does not represent CIN, whose national number is CPF. Validation does not prove existence, authenticity,
ownership, or document status.
