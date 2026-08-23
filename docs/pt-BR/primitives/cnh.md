# CNH

`Cnh` modela somente o Número do Registro Nacional da Carteira Nacional de Habilitação.

## Contrato

| Aspecto | Comportamento |
| --- | --- |
| Valor canônico | 11 dígitos ASCII |
| Entrada aceita | somente sem máscara |
| Normalização | preserva os dígitos |
| Dígitos verificadores | dois DVs módulo 11 com regra de desconto entre DVs |
| Formatação | `ToString()` retorna o valor canônico |

```csharp
Cnh cnh = Cnh.Parse("62472927637");

Console.WriteLine(cnh.Value); // 62472927637
```

`Cnh` não modela número do espelho, formulário RENACH, QR Code, código de segurança ou CPF do condutor.

A validação não comprova emissão, titularidade, validade administrativa, categoria ou restrições do condutor.
