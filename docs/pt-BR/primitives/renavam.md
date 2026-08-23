# RENAVAM

`Renavam` representa o Registro Nacional de Veículos Automotores como 10 dígitos-base mais um DV.

## Contrato

| Aspecto | Comportamento |
| --- | --- |
| Valor canônico | 11 dígitos ASCII |
| Entrada aceita | somente sem máscara |
| Normalização | preserva os dígitos |
| Dígito verificador | módulo 11 com pesos `3, 2, 9, 8, 7, 6, 5, 4, 3, 2` |
| Formatação | `ToString()` retorna o valor canônico |

```csharp
Renavam renavam = Renavam.Parse("00123456789");

Console.WriteLine(renavam.Value); // 00123456789
```

Valores históricos mais curtos não são completados pelo parser; forneça a forma atual com 11 dígitos.

A validação não consulta SENATRAN/DETRAN e não comprova existência do veículo, licenciamento, placa, chassi, proprietário,
débitos, multas ou restrições.
