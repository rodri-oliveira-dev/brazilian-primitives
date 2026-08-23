# InscricaoEstadual

`InscricaoEstadual` represents a Brazilian state ICMS taxpayer registration with explicit `BrazilianState` context.

## Contract

| Aspect | Behavior |
| --- | --- |
| Canonical value | ASCII digits with state-specific length |
| Required context | `BrazilianState` |
| Accepted input | unmasked only |
| Check digits | not implemented in the current matrix |
| Validation mode | format-only by documented length |

```csharp
InscricaoEstadual ie = InscricaoEstadual.Parse("110042490114", BrazilianState.SaoPaulo);

Console.WriteLine(ie.Value); // 110042490114
Console.WriteLine(ie.State); // SaoPaulo
```

Bahia, Pernambuco, and Rio Grande do Norte accept two documented lengths; other states accept one documented length.
`ISENTO` is rejected because it is a fiscal condition, not an identifier.

Validation does not query SINTEGRA or SEFAZ and does not prove taxpayer existence, active status, NF-e authorization,
tax regularity, or relationship to a CPF/CNPJ.
