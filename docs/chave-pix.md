# Chave Pix

`ChavePix` represents the five Pix key kinds supported by the local contract:

- CPF, delegated to `Cpf`;
- CNPJ, delegated to `Cnpj`, including alphanumeric CNPJ values accepted by the current library contract;
- Brazilian mobile phone, delegated to `MobilePhone` and represented as E.164;
- email, delegated first to `Email` and then canonicalized to the Pix lowercase representation with a 77-character
  limit;
- random EVP key, accepted only as canonical UUID text (`xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`) and normalized to
  lowercase hexadecimal.

```csharp
ChavePix celular = ChavePix.Parse("(11) 98765-4321");
ChavePix email = ChavePix.Parse("User@Example.COM");

Console.WriteLine(celular.Value); // +5511987654321
Console.WriteLine(email.Value);   // user@example.com
```

The generic `Email` primitive preserves local-part case, while Pix email keys are lowercase by DICT contract. This
difference is specific to `ChavePix`; parsing Pix keys does not change the behavior of `Email`, `Cpf`, `Cnpj`, or
`MobilePhone`.

Validation is local only. It does not query DICT, DNS, Receita Federal, phone portability, or any banking system, and
does not prove that a key exists, is active, or belongs to a person/account.

References consulted on 2026-08-23:

- Banco Central, Pix FAQ: `https://www.bcb.gov.br/meubc/faqs/p/O-que-e-chave-pix`
- Banco Central, API DICT: `https://www.bcb.gov.br/content/estabilidadefinanceira/pix/API-DICT.html`
- Banco Central, Regulamento Pix / Resolucao BCB 1: `https://www.bcb.gov.br/estabilidadefinanceira/exibenormativo?numero=1&tipo=Resolu%C3%A7%C3%A3o+BCB`
