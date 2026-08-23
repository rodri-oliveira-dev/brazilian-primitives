# RG

`Rg` representa o Registro Geral estadual legado. A UF emissora é obrigatória porque não existe formato nacional único.

## Contrato

| Aspecto | Comportamento |
| --- | --- |
| Valor canônico | texto específico da UF |
| Contexto obrigatório | `BrazilianState` emissor |
| São Paulo | 8 dígitos + DV numérico ou `X`; máscara `12.030.001-1` |
| Rio de Janeiro | 9 dígitos; máscara opcional `12.345.678-9`; format-only |
| Minas Gerais | 8 dígitos; aceita prefixos `MG-` e `M-`; format-only |
| Santa Catarina | 9 dígitos; máscara opcional `123.456.789`; format-only |
| Demais UFs | tamanho documentado em dígitos |

```csharp
Rg rg = Rg.Parse("12.030.001-1", BrazilianState.SaoPaulo);

Console.WriteLine(rg.Value);     // 120300011
Console.WriteLine(rg.Formatted); // 12.030.001-1
Console.WriteLine(rg.State);     // SaoPaulo
```

A igualdade inclui a UF. O mesmo número textual em estados diferentes não é o mesmo `Rg`.

Este tipo não representa a CIN, cujo número nacional é o CPF. A validação não comprova existência, autenticidade,
titularidade ou situação do documento.
