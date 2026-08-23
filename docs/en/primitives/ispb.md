# ISPB

`Ispb` represents an identifier for Brazilian Payments System/STR participants.

## Contract

| Aspect | Behavior |
| --- | --- |
| Canonical value | 8 ASCII digits |
| Accepted input | unmasked only |
| Normalization | none beyond storing the digits |
| Check digits | none |
| Validation mode | structural only |

```csharp
Ispb ispb = Ispb.Parse("12345678");

Console.WriteLine(ispb.Value); // 12345678
```

The parser does not derive ISPB from CNPJ and does not convert between ISPB and COMPE.

Validation does not prove Banco Central authorization, participant existence, STR/Pix/COMPE participation, settlement
account status, or any CNPJ/status relationship.
