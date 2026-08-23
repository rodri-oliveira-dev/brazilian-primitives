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

`Parse`, `TryParse` e `IsValid` inferem o tipo da chave a partir do texto. Se uma entrada sem tipo explícito for válida
para mais de um tipo, por exemplo um valor que seja simultaneamente CPF válido e celular brasileiro válido, ela é
rejeitada como ambígua. Quando o tipo da chave já é conhecido, use as fábricas explícitas `From(Cpf)`,
`From(MobilePhone)`, `From(Cnpj)`, `From(Email)` ou `FromChaveAleatoria`.

A validação não consulta DICT, bancos, Receita Federal, DNS, portabilidade telefônica ou contas bancárias.
