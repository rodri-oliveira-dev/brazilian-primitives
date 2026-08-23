# RG

RG is a legacy state-issued Brazilian identity document. `Rg` requires the issuing `BrazilianState` because there is no
single national format or check-digit algorithm.

## Contract

| Aspect | Behavior |
| --- | --- |
| Canonical value | state-specific text |
| Required context | issuing `BrazilianState` |
| Sao Paulo | 8 digits plus numeric or `X` check digit; mask `12.030.001-1` |
| Rio de Janeiro | 9 digits; optional mask `12.345.678-9`; format-only |
| Minas Gerais | 8 digits; accepts `MG-` and historical `M-` prefixes; format-only |
| Santa Catarina | 9 digits; optional mask `123.456.789`; format-only |
| Other states | documented digit length only |

```csharp
Rg rg = Rg.Parse("12.030.001-1", BrazilianState.SaoPaulo);

Console.WriteLine(rg.Value);     // 120300011
Console.WriteLine(rg.Formatted); // 12.030.001-1
Console.WriteLine(rg.State);     // SaoPaulo
```

Equality includes the state. The same text issued in different states is not the same `Rg`.

This type does not represent CIN, whose national number is CPF. Validation does not prove existence, authenticity,
ownership, or document status.
