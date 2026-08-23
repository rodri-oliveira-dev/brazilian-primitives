# CEP

CEP is Brazil's postal code. `Cep` represents only the supported eight-digit postal-code structure.

## Contract

| Aspect | Behavior |
| --- | --- |
| Canonical value | 8 ASCII digits |
| Accepted input | `01311000` or `01311-000` |
| Normalization | removes the canonical hyphen |
| Check digits | none |
| Formatting | `Value`/`G`: `01311000`; `Formatted`/`F`: `01311-000` |

```csharp
Cep cep = Cep.Parse("01311-000");

Console.WriteLine(cep.Value);     // 01311000
Console.WriteLine(cep.Formatted); // 01311-000
```

`Cep.IsValid("00000000")` returns `true` because validation is structural. The library does not query Correios, DNE, or
any address database and does not prove the code is assigned to a real address.
