# PisPasep

`PisPasep` representa um registro PIS/PASEP com 10 dígitos-base e um dígito verificador.

## Contrato

| Aspecto | Comportamento |
| --- | --- |
| Valor canônico | 11 dígitos ASCII |
| Entrada aceita | somente sem máscara |
| Normalização | preserva os dígitos |
| Dígito verificador | módulo 11 com pesos `3, 2, 9, 8, 7, 6, 5, 4, 3, 2` |
| Formatação | `ToString()` retorna o valor canônico |

```csharp
PisPasep pis = PisPasep.Parse("12044529868");

Console.WriteLine(pis.Value); // 12044529868
```

Sequências repetidas e DVs inválidos são rejeitados.

A validação não consulta Caixa, Banco do Brasil, CNIS ou sistemas de benefícios e não comprova existência, titularidade,
vínculo trabalhista, elegibilidade ou situação cadastral.
