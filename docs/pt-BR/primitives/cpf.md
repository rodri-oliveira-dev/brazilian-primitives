# CPF

`Cpf` representa um Cadastro de Pessoas Físicas com estrutura e dígitos verificadores válidos.

## Contrato

| Aspecto | Comportamento |
| --- | --- |
| Valor canônico | 11 dígitos ASCII, por exemplo `52998224725` |
| Entrada aceita | `52998224725` ou `529.982.247-25` |
| Normalização | remove apenas a máscara canônica |
| Dígitos verificadores | valida os dois DVs módulo 11 |
| Formatação | `Value`/`ToString()`/`G`: `52998224725`; `Formatted`/`F`: `529.982.247-25` |

```csharp
Cpf cpf = Cpf.Parse("529.982.247-25");

Console.WriteLine(cpf.Value);     // 52998224725
Console.WriteLine(cpf.Formatted); // 529.982.247-25
```

`Parse` lança `FormatException`; `TryParse` retorna `false`; `IsValid` faz validação local.

CPF válido na biblioteca não comprova existência na Receita Federal, titularidade ou situação cadastral.
