# InscricaoEstadual

`InscricaoEstadual` represents a Brazilian state ICMS taxpayer registration and supports both context-free and
state-aware usage for legacy schemas that do not always contain a reliable UF.

## Contract

| Aspect | Behavior |
| --- | --- |
| Canonical value | unmasked ASCII digits |
| Context-free mode | 8 to 14 ASCII digits; structural/format-only |
| Missing state | `State == BrazilianState.Unknown` and `HasState == false` |
| State-aware mode | explicit `BrazilianState`; documented state-specific lengths apply |
| Accepted input | unmasked only |
| Check digits | not implemented in the current state matrix |

## Context-free usage

```csharp
InscricaoEstadual ie = InscricaoEstadual.Parse("0012345678");

Console.WriteLine(ie.Value);    // 0012345678
Console.WriteLine(ie.State);    // Unknown
Console.WriteLine(ie.HasState); // False
```

Context-free validation accepts only 8 to 14 ASCII digits. It does not infer a UF or apply a state-specific length or
checksum rule. Leading zeros are preserved. `ISENTO` remains invalid because it is a fiscal condition, not an identifier.

## State-aware usage

```csharp
InscricaoEstadual ie = InscricaoEstadual.Parse("110042490114", BrazilianState.SaoPaulo);

Console.WriteLine(ie.Value);    // 110042490114
Console.WriteLine(ie.State);    // SaoPaulo
Console.WriteLine(ie.HasState); // True
```

Bahia, Pernambuco, and Rio Grande do Norte accept two documented lengths; other states accept one documented length.
Equality preserves context: a context-free registration is not equal to the same canonical digits with a known state,
and the same digits associated with different states represent different values.

## Entity Framework Core SQL Server

For context-free values, `InscricaoEstadualValueConverter` persists the canonical value in one `varchar(14)` column.
It rejects a state-aware instance instead of silently dropping its UF.

For state-aware values, use `InscricaoEstadualStateAwareSqlServerMapping` with an EF Core complex property. It persists
the canonical registration and the UF in separate columns; the UF uses a stable two-letter code such as `SP` or `RO`.

Property nullability is separate from the state context: `InscricaoEstadual? == null` means the identifier is absent,
while a non-null value with `State == BrazilianState.Unknown` means the registration exists but its UF was not supplied.

Validation does not query SINTEGRA or SEFAZ and does not prove taxpayer existence, active status, NF-e authorization,
tax regularity, or relationship to a CPF/CNPJ.
