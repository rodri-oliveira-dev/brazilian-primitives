# ChavePix

Pix is Brazil's instant payment system. `ChavePix` represents a local Pix key shape, not a DICT registry entry.

## Contract

| Pix key kind | Canonical value |
| --- | --- |
| CPF | delegated CPF digits |
| CNPJ | delegated CNPJ value, including alphanumeric CNPJ |
| Mobile phone | delegated E.164 mobile phone |
| Email | delegated email value lowercased, max 77 characters |
| Random key | canonical UUID text, lowercase hex |

```csharp
ChavePix celular = ChavePix.Parse("(11) 98765-4321");
ChavePix email = ChavePix.Parse("User@Example.COM");

Console.WriteLine(celular.Tipo); // Celular
Console.WriteLine(celular.Value); // +5511987654321
Console.WriteLine(email.Value);   // user@example.com
```

The generic `Email` primitive preserves local-part case; Pix email keys are lowercased by `ChavePix`.

Validation does not query DICT, banks, Receita Federal, DNS, phone portability, or account systems.
