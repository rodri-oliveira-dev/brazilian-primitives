# CPF

CPF is Brazil's individual taxpayer number. `Cpf` represents a structurally and mathematically valid CPF, not a person
record at Receita Federal.

## Contract

| Aspect | Behavior |
| --- | --- |
| Canonical value | 11 ASCII digits, for example `52998224725` |
| Accepted input | `52998224725` or `529.982.247-25` |
| Normalization | removes the canonical mask only |
| Check digits | validates both modulo-11 digits |
| Formatting | `Value`/`ToString()`/`G`: `52998224725`; `Formatted`/`F`: `529.982.247-25` |

```csharp
Cpf cpf = Cpf.Parse("529.982.247-25");

Console.WriteLine(cpf.Value);     // 52998224725
Console.WriteLine(cpf.Formatted); // 529.982.247-25
```

`Parse` throws `FormatException`; `TryParse` returns `false`; `IsValid` is local validation only.

It does not prove Receita Federal existence, ownership, active status, or suitability for KYC/compliance decisions.
