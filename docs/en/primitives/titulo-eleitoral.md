# TituloEleitoral

`TituloEleitoral` represents the Brazilian voter registration number in the canonical 12-digit form.

## Contract

| Aspect | Behavior |
| --- | --- |
| Canonical value | 12 ASCII digits |
| Accepted input | unmasked only |
| Structure | 8 sequential digits, 2 origin digits, 2 check digits |
| Origin | `01` through `27` map to states; `28` is Exterior |
| Check digits | two modulo-11 calculations |

```csharp
TituloEleitoral titulo = TituloEleitoral.Parse("000123450159");

Console.WriteLine(titulo.NumeroSequencial); // 00012345
Console.WriteLine(titulo.CodigoOrigem);     // 01
```

Use `TryGetState(out BrazilianState state)` for national origin codes. `IsExterior` is `true` for origin `28`.

Validation does not query TSE and does not prove electoral regularity, discharge, domicile, polling section, biometric
status, or ownership.
