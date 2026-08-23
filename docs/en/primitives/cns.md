# CNS

`Cns` represents a Brazilian National Health Card number as a 15-digit value with supported family algorithms.

## Contract

| Aspect | Behavior |
| --- | --- |
| Canonical value | 15 ASCII digits |
| Accepted input | unmasked only |
| Supported prefixes | `1`, `2`, `7`, `8`, `9` |
| Algorithms | beneficiary families `1`/`2`; provisional families `7`/`8`/`9` |
| Formatting | `ToString()` returns the canonical value |

```csharp
Cns cns = Cns.Parse("123456789010000");

Console.WriteLine(cns.Value); // 123456789010000
```

All-zero values and unsupported prefixes are rejected.

Validation does not query CADSUS or Meu SUS Digital and does not prove ownership, main CNS status, duplicate linkage,
CPF linkage, cadastral quality, or entitlement to care.
