# CNPJ

`Cnpj` representa o Cadastro Nacional da Pessoa Jurídica, incluindo o formato numérico tradicional e o contrato
alfanumérico atual.

## Contrato

| Aspecto | Comportamento |
| --- | --- |
| Valor canônico | 14 posições, sem máscara, letras em maiúsculas |
| Entrada aceita | sem máscara ou `AA.AAA.AAA/AAAA-DD` |
| Normalização | letras ASCII minúsculas viram maiúsculas |
| Dígitos verificadores | módulo 11 da Receita Federal usando ASCII maiúsculo menos 48 |
| Formatação | `Value`/`G`: `00000000E08G12`; `Formatted`/`F`: `00.000.000/E08G-12` |

```csharp
Cnpj cnpj = Cnpj.Parse("00.000.000/e08g-12");

Console.WriteLine(cnpj.Value);     // 00000000E08G12
Console.WriteLine(cnpj.Formatted); // 00.000.000/E08G-12
```

A entrada é estrita: pontuação solta, acentos, dígitos Unicode parecidos e DVs inválidos são rejeitados.

CNPJ válido aqui não comprova existência, atividade ou situação cadastral na Receita Federal.
