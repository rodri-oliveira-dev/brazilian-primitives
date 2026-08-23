# CPF/CNPJ

`CpfCnpj` is a discriminated wrapper for fields that explicitly accept either a CPF or a CNPJ. It is not a third tax
registration algorithm.

## Contract

| Aspect | Behavior |
| --- | --- |
| Canonical value | delegated from `Cpf` or `Cnpj` |
| Accepted input | any representation accepted by exactly one of `Cpf` or `Cnpj` |
| Normalization | delegated |
| Check digits | delegated |
| Type discriminator | `TipoCpfCnpj.Cpf` or `TipoCpfCnpj.Cnpj` |

```csharp
CpfCnpj documento = CpfCnpj.Parse("00.000.000/e08g-12");

Console.WriteLine(documento.Tipo);      // Cnpj
Console.WriteLine(documento.Value);     // 00000000E08G12
Console.WriteLine(documento.Formatted); // 00.000.000/E08G-12
```

Use `Cpf` or `Cnpj` directly when a field accepts only one kind. Use `CpfCnpj` only when the business contract is the
union.
