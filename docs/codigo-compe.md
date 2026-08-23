# CodigoCompe

`CodigoCompe` represents the current COMPE institution code contract as exactly 3 ASCII digits.

```csharp
CodigoCompe codigo = CodigoCompe.Parse("001");

Console.WriteLine(codigo.Value); // 001
```

The value is stored as `string` to preserve leading zeros and avoid numeric identity mistakes. The parser rejects
`999`, which appears in Banco Central layouts as a sentinel for institutions without a COMPE code. Applications should
represent absence with `null`, an optional value, or a missing field instead of constructing a fake code.

`CodigoCompe` is separate from `Ispb` and from the STR "numero-codigo". The BCB Comunicado 45.753 of 2026-08-14
announces an alphanumeric future for the STR regulatory number-code in 2027, while BCB layouts consulted for
`COD_COMP_INSTITUICAO` still describe the COMPE code field as numeric with length 3 and sentinel `999`. This primitive
therefore keeps the current COMPE contract numeric and isolated. If BCB later confirms that COMPE itself changes to
an alphanumeric domain, the textual storage allows evolution without changing the identity representation.

`CodigoCompe.IsValid` is structural only. It does not prove assignment, institution existence, current COMPE
participation, account validity, association with an ISPB, or operational status. The Core package does not embed a
bank table and does not query Banco Central.

References consulted on 2026-08-23:

- Banco Central, Instrucao Normativa BCB 636, layouts with `COD_COMP_INSTITUICAO` numeric length 3 and sentinel `999`:
  `https://www.bcb.gov.br/estabilidadefinanceira/exibenormativo?numero=636&tipo=Instru%C3%A7%C3%A3o+Normativa+BCB`
- Banco Central, Comunicado 45.753 of 2026-08-14, future alphanumeric STR number-code:
  `https://www.bcb.gov.br/estabilidadefinanceira/exibenormativo?numero=45753&tipo=Comunicado`
- Banco Central, STR documentation: `https://www.bcb.gov.br/estabilidadefinanceira/str`
