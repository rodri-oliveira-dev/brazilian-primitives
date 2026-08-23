# RENAVAM

`Renavam` represents the Brazilian national vehicle registry code as 10 base digits plus one check digit.

## Contract

| Aspect | Behavior |
| --- | --- |
| Canonical value | 11 ASCII digits |
| Accepted input | unmasked only |
| Normalization | none beyond storing the digits |
| Check digit | modulo-11 rule with weights `3, 2, 9, 8, 7, 6, 5, 4, 3, 2` |
| Formatting | `ToString()` returns the canonical value |

```csharp
Renavam renavam = Renavam.Parse("00123456789");

Console.WriteLine(renavam.Value); // 00123456789
```

Historical shorter RENAVAM values are not padded by the parser; callers must supply the current 11-digit form.

Validation does not query SENATRAN/DETRAN and does not prove vehicle existence, licensing status, plate, chassis,
owner, debts, fines, or restrictions.
