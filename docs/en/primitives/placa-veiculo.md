# PlacaVeiculo

`PlacaVeiculo` models the seven-character Brazilian vehicle plate sequence.

## Contract

| Aspect | Behavior |
| --- | --- |
| Canonical value | 7 uppercase ASCII letters/digits |
| Previous national pattern | `ABC1234` or `ABC-1234` |
| Mercosur/PIV pattern | `ABC1D23` without a hyphen |
| Normalization | ASCII lowercase letters become uppercase |
| Formatting | previous pattern formats as `ABC-1234`; Mercosur remains unmasked |

```csharp
PlacaVeiculo placa = PlacaVeiculo.Parse("abc-1234");

Console.WriteLine(placa.Value);     // ABC1234
Console.WriteLine(placa.Formatted); // ABC-1234
Console.WriteLine(placa.Padrao);    // NacionalAnterior
```

`ConverterParaPadraoMercosul()` applies the official sequence table `0..9 -> A..J` for previous national plates only.

The type does not infer vehicle category, visual color, dimensions, plate quantity, assignment, existence, regularity,
QR Code status, or relation to RENAVAM/chassis.
