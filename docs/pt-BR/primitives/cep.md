# CEP

`Cep` representa somente a estrutura de um Código de Endereçamento Postal brasileiro.

## Contrato

| Aspecto | Comportamento |
| --- | --- |
| Valor canônico | 8 dígitos ASCII |
| Entrada aceita | `01311000` ou `01311-000` |
| Normalização | remove o hífen canônico |
| Dígitos verificadores | nenhum |
| Formatação | `Value`/`G`: `01311000`; `Formatted`/`F`: `01311-000` |

```csharp
Cep cep = Cep.Parse("01311-000");

Console.WriteLine(cep.Value);     // 01311000
Console.WriteLine(cep.Formatted); // 01311-000
```

`Cep.IsValid("00000000")` retorna `true` porque a validação é estrutural. A biblioteca não consulta Correios, DNE ou
base de endereços e não comprova que o CEP esteja atribuído.
