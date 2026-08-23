# CPF/CNPJ

`CpfCnpj` é um wrapper discriminado para campos que aceitam CPF ou CNPJ. Ele delega validação, normalização e
formatação para `Cpf` e `Cnpj`.

## Contrato

| Aspecto | Comportamento |
| --- | --- |
| Valor canônico | delegado de `Cpf` ou `Cnpj` |
| Entrada aceita | qualquer representação aceita por exatamente um dos dois tipos |
| Normalização | delegada |
| Dígitos verificadores | delegados |
| Discriminador | `TipoCpfCnpj.Cpf` ou `TipoCpfCnpj.Cnpj` |

```csharp
CpfCnpj documento = CpfCnpj.Parse("00.000.000/e08g-12");

Console.WriteLine(documento.Tipo);      // Cnpj
Console.WriteLine(documento.Value);     // 00000000E08G12
Console.WriteLine(documento.Formatted); // 00.000.000/E08G-12
```

Prefira `Cpf` ou `Cnpj` diretamente quando o domínio aceitar apenas um tipo de documento.
