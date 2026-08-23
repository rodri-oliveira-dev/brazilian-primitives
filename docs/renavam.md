# RENAVAM

`Renavam` represents the Registro Nacional de Veiculos Automotores code as an 11-digit ASCII string: 10 base digits
plus one check digit.

The check digit follows the modulo 11 rule described for the RENAVAM code in DENATRAN Portaria 27/2013: weights
`3, 2, 9, 8, 7, 6, 5, 4, 3, 2` are applied over the 10 base digits, the sum is multiplied by 10, the result is reduced
modulo 11, and result `10` maps to `0`.

```csharp
Renavam renavam = Renavam.Parse("00123456789");

Console.WriteLine(renavam.Value); // 00123456789
```

Only the current 11-digit representation is accepted. Historical 9-digit RENAVAM values must be supplied using the
official zero-padded 11-position representation; the parser does not pad arbitrary shorter input.

`Renavam.IsValid` checks structure and check digit only. It does not query SENATRAN/DETRAN, prove that a vehicle
exists, prove licensing status, or link the code to a plate, chassis, owner, debt, fine, or restriction.

References consulted on 2026-08-23:

- SENATRAN/DENATRAN Portaria 27/2013, RENAVAM as 10 digits plus one verifier by modulo 11, weight 9:
  `https://www.gov.br/transportes/pt-br/assuntos/transito/arquivos-senatran/portarias/2013/portaria0272013.pdf`
- Federal government notice about the 2013 expansion from 9 to 11 digits and zero-padding of older codes:
  `https://www.gov.br/mdr/pt-br/noticias/codigo-do-renavan-ganhara-dois-digitos-a-partir-de-1o-de-abril-de-2013`
