# CNPJ

CNPJ is Brazil's company taxpayer registration. `Cnpj` supports the numeric form and the current alphanumeric contract
where the first 12 positions may contain ASCII digits or letters and the last two positions are numeric check digits.

## Contract

| Aspect | Behavior |
| --- | --- |
| Canonical value | 14 characters, uppercase, no mask |
| Accepted input | unmasked or `AA.AAA.AAA/AAAA-DD` |
| Normalization | ASCII lowercase letters become uppercase |
| Check digits | Receita Federal modulo-11 using uppercase ASCII code minus 48 |
| Formatting | `Value`/`G`: `00000000E08G12`; `Formatted`/`F`: `00.000.000/E08G-12` |

```csharp
Cnpj cnpj = Cnpj.Parse("00.000.000/e08g-12");

Console.WriteLine(cnpj.Value);     // 00000000E08G12
Console.WriteLine(cnpj.Formatted); // 00.000.000/E08G-12
```

The parser rejects arbitrary punctuation, accents, Unicode lookalikes, repeated-character sentinels, and invalid check
digits.

It does not prove that the company exists, is active, or has any cadastral status at Receita Federal.
