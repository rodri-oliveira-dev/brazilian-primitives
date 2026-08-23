# CodigoCompe

`CodigoCompe` represents the current numeric COMPE institution-code contract.

## Contract

| Aspect | Behavior |
| --- | --- |
| Canonical value | 3 ASCII digits |
| Accepted input | unmasked only |
| Normalization | none beyond storing the digits |
| Rejected sentinel | `999` |
| Validation mode | structural only |

```csharp
CodigoCompe codigo = CodigoCompe.Parse("001");

Console.WriteLine(codigo.Value); // 001
```

Leading zeros are significant. Absence should be modeled as `null`, an optional value, or a missing field, not as
`999`.

Validation does not prove assignment, institution existence, current COMPE participation, account validity, association
with an ISPB, or operational status.
