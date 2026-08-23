# Email

`Email` representa um subconjunto estrito e interoperável de endereço de email.

## Contrato

| Aspecto | Comportamento |
| --- | --- |
| Valor canônico | local part preservado, domínio em lowercase/Punycode |
| Entrada aceita | um endereço `local@dominio` |
| Local part | dot-atom ASCII, até 64 caracteres |
| Domínio | labels DNS/IDN, até 63 caracteres cada |
| Tamanho total | até 254 caracteres após normalização |

```csharp
Email email = Email.Parse("User@Domínio.COM");

Console.WriteLine(email.Value);     // User@xn--domnio-5va.com
Console.WriteLine(email.LocalPart); // User
Console.WriteLine(email.Domain);    // xn--domnio-5va.com
```

Quoted local parts, nomes de exibição, listas, literais de domínio, `mailto:` e local part SMTPUTF8 são rejeitados.

A validação não consulta DNS, MX, existência da caixa, entregabilidade, titularidade ou aliases do provedor.
