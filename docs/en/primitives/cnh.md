# CNH

`Cnh` models only the 11-digit National Registration Number from the Brazilian driver's license context.

## Contract

| Aspect | Behavior |
| --- | --- |
| Canonical value | 11 ASCII digits |
| Accepted input | unmasked only |
| Normalization | none beyond storing the digits |
| Check digits | two modulo-11 digits with the inter-check-digit discount rule |
| Formatting | `ToString()` returns the canonical value |

```csharp
Cnh cnh = Cnh.Parse("62472927637");

Console.WriteLine(cnh.Value); // 62472927637
```

`Cnh` does not model the CNH mirror number, RENACH form number, QR Code, security code, or the driver's CPF.

Validation does not prove that the registration was issued, belongs to a driver, is active, has a category, or is free
from administrative restrictions.
