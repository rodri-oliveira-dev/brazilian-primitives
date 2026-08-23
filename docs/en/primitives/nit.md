# NIT

`Nit` represents a Brazilian worker identification number in the current structural contract.

## Contract

| Aspect | Behavior |
| --- | --- |
| Canonical value | 11 ASCII digits |
| Accepted input | unmasked only |
| Normalization | none beyond storing the digits |
| Check digits | not implemented for NIT |
| Validation mode | structural only |

```csharp
Nit nit = Nit.Parse("12345678901");

Console.WriteLine(nit.Value); // 12345678901
```

The type is deliberately separate from `PisPasep` and does not treat NIT, PIS, PASEP, and NIS as interchangeable.

Validation does not prove CNIS existence, social-security affiliation, ownership, contribution history, benefit rights,
or active cadastral status.
