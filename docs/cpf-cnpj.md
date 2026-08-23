# CPF/CNPJ

`CpfCnpj` represents fields that explicitly accept either `Cpf` or `Cnpj`. It is a discriminated wrapper over the
existing primitives, not a third implementation of Brazilian tax-registration algorithms.

```csharp
CpfCnpj documento = CpfCnpj.Parse("00.000.000/e08g-12");

Console.WriteLine(documento.Tipo);      // Cnpj
Console.WriteLine(documento.Value);     // 00000000E08G12
Console.WriteLine(documento.Formatted); // 00.000.000/E08G-12
```

Use `Cpf` or `Cnpj` directly when the domain requires exactly one kind of registration. Use `CpfCnpj` when the
domain contract really is the union of both. Validation remains local: it does not query Receita Federal, prove
existence, prove ownership, or indicate cadastral status.

The wrapper delegates parsing, check digits, canonical representation, and formatting to the underlying type,
including alphanumeric CNPJ support.
