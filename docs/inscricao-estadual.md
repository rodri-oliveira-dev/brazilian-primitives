# InscricaoEstadual

`InscricaoEstadual` represents a state ICMS taxpayer registration with explicit `BrazilianState` context. There is no
single national format or check-digit algorithm, and the same text in different UFs is not the same identity.

This first implementation is intentionally `format-only` for every UF in the matrix below. It validates strict ASCII
canonical length only and does not invent state check-digit algorithms without embedding an authoritative UF-specific
formula in the library. `ISENTO` is rejected because it is a fiscal/cadastral condition, not an Inscricao Estadual
identifier.

```csharp
InscricaoEstadual ie = InscricaoEstadual.Parse("110042490114", BrazilianState.SaoPaulo);

Console.WriteLine(ie.Value); // 110042490114
Console.WriteLine(ie.State); // SaoPaulo
```

| UF | Formats | DV | Validation | Source |
|---|---|---|---|---|
| AC | 13 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| AL | 9 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| AP | 9 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| AM | 9 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| BA | 8 or 9 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| CE | 9 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| DF | 13 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| ES | 9 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| GO | 9 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| MA | 9 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| MT | 11 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| MS | 9 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| MG | 13 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| PA | 9 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| PB | 9 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| PR | 10 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| PE | 9 or 14 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| PI | 9 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| RJ | 8 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| RN | 9 or 10 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| RS | 10 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| RO | 14 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| RR | 9 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| SC | 9 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| SP | 12 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| SE | 9 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |
| TO | 11 digits | State-specific | format-only | SINTEGRA/SEFAZ matrix consulted 2026-08-23 |

`InscricaoEstadual.IsValid` does not query SINTEGRA or SEFAZ and does not prove taxpayer existence, active status,
NF-e authorization, tax regularity, or relationship to a CPF/CNPJ.

References consulted on 2026-08-23:

- SINTEGRA, national Inscricao Estadual conference page and UF formula routing:
  `https://www.sintegra.gov.br/insc_est.html`
- SINTEGRA portal: `https://www.sintegra.gov.br/`
