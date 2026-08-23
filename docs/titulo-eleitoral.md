# TituloEleitoral

`TituloEleitoral` represents the voter registration number in the supported canonical 12-digit form:

- 8 sequential digits;
- 2 origin digits;
- 2 modulo-11 check digits.

```csharp
TituloEleitoral titulo = TituloEleitoral.Parse("000123450159");

Console.WriteLine(titulo.NumeroSequencial); // 00012345
Console.WriteLine(titulo.CodigoOrigem);     // 01
```

Resolution TSE 23.659/2021 states that leading zeros in the sequential part may be omitted when issuing the visual
number. This implementation accepts only the unambiguous canonical 12-digit representation. Shorter issued forms are
outside the first contract until a deterministic official reconstruction rule is embedded.

Origin codes:

| Code | Origin |
|---|---|
| 01 | SP |
| 02 | MG |
| 03 | RJ |
| 04 | RS |
| 05 | BA |
| 06 | PR |
| 07 | CE |
| 08 | PE |
| 09 | SC |
| 10 | GO |
| 11 | MA |
| 12 | PB |
| 13 | PA |
| 14 | ES |
| 15 | PI |
| 16 | RN |
| 17 | AL |
| 18 | MT |
| 19 | MS |
| 20 | DF |
| 21 | SE |
| 22 | AM |
| 23 | RO |
| 24 | AC |
| 25 | AP |
| 26 | RR |
| 27 | TO |
| 28 | Exterior (ZZ) |

National origin codes are exposed through `TryGetState(out BrazilianState state)`. Exterior is represented explicitly
by `IsExterior` and is not forced into a fake `BrazilianState`.

The first check digit is calculated over the eight sequential digits with weights `2..9`. The second is calculated over
the two origin digits plus the first check digit with weights `7, 8, 9`. Each calculation uses modulo 11, mapping
result `10` to `0`.

Validation is local only. It does not query TSE and does not prove electoral regularity, discharge, domicile, polling
section, biometric status, or ownership.

Reference consulted on 2026-08-23:

- TSE Resolucao 23.659/2021, art. 36:
  `https://www.tse.jus.br/legislacao/compilada/res/2021/resolucao-no-23-659-de-26-de-outubro-de-2021`
