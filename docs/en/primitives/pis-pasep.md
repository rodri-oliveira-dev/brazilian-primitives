# PisPasep

`PisPasep` represents a PIS/PASEP registration number as 10 base digits plus one check digit.

## Contract

| Aspect | Behavior |
| --- | --- |
| Canonical value | 11 ASCII digits |
| Accepted input | unmasked only |
| Normalization | none beyond storing the digits |
| Check digit | modulo-11 weights `3, 2, 9, 8, 7, 6, 5, 4, 3, 2` |
| Formatting | `ToString()` returns the canonical value |

```csharp
PisPasep pis = PisPasep.Parse("12044529868");

Console.WriteLine(pis.Value); // 12044529868
```

Repeated-digit sentinels and invalid check digits are rejected.

Validation does not query Caixa, Banco do Brasil, CNIS, or benefit systems and does not prove registration existence,
ownership, employment relationship, benefit eligibility, or cadastral status.
