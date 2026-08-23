# ChavePix

`ChavePix` representa localmente o formato de uma chave Pix. Ela não representa um registro consultado no DICT.

## Contrato

| Tipo de chave | Valor canônico |
| --- | --- |
| CPF | dígitos delegados de `Cpf` |
| CNPJ | valor delegado de `Cnpj`, incluindo CNPJ alfanumérico |
| Celular | telefone celular em E.164 |
| Email | email delegado em lowercase, até 77 caracteres |
| Aleatória | UUID textual canônico, hex em lowercase |

```csharp
ChavePix celular = ChavePix.Parse("(11) 98765-4321");
ChavePix email = ChavePix.Parse("User@Example.COM");

Console.WriteLine(celular.Tipo);  // Celular
Console.WriteLine(celular.Value); // +5511987654321
Console.WriteLine(email.Value);   // user@example.com
```

`Email` preserva case do local part; `ChavePix` aplica lowercase porque esse é o contrato da chave Pix de email.

A validação não consulta DICT, bancos, Receita Federal, DNS, portabilidade telefônica ou contas bancárias.
