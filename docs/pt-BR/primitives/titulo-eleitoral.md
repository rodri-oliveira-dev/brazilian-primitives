# TituloEleitoral

`TituloEleitoral` representa o título de eleitor na forma canônica de 12 dígitos.

## Contrato

| Aspecto | Comportamento |
| --- | --- |
| Valor canônico | 12 dígitos ASCII |
| Entrada aceita | somente sem máscara |
| Estrutura | 8 dígitos sequenciais, 2 de origem, 2 DVs |
| Origem | `01` a `27` mapeiam UFs; `28` representa Exterior |
| Dígitos verificadores | dois cálculos módulo 11, incluindo exceção do primeiro DV para origens `01` e `02` quando o resto é 10 |

```csharp
TituloEleitoral titulo = TituloEleitoral.Parse("000123450159");

Console.WriteLine(titulo.NumeroSequencial); // 00012345
Console.WriteLine(titulo.CodigoOrigem);     // 01
```

Use `TryGetState(out BrazilianState state)` para origens nacionais. `IsExterior` é `true` para origem `28`.

A validação não consulta TSE e não comprova regularidade eleitoral, quitação, domicílio, zona/seção, biometria ou
titularidade.
