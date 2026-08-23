# PisPasep

`PisPasep` represents a PIS/PASEP registration as an 11-digit ASCII string: 10 base digits plus one verifier.

The check digit follows the ANS-published modulo 11 routine for PIS/PASEP: apply weights `3, 2, 9, 8, 7, 6, 5, 4, 3, 2`
over the first 10 digits, compute the sum modulo 11, subtract the remainder from 11, and map results `10` and `11` to
`0`.

```csharp
PisPasep pis = PisPasep.Parse("12044529868");

Console.WriteLine(pis.Value); // 12044529868
```

`PisPasep` remains semantically separate from `Nit` and NIS. Validation is structural and mathematical only; it does
not query Caixa, Banco do Brasil, CNIS, or any benefits system and does not prove registration existence, ownership,
employment relationship, benefit eligibility, or cadastral status.

References consulted on 2026-08-23:

- ANS, Algoritmos do Aplicativo de Carga, PIS/PASEP validation:
  `https://www.gov.br/ans/pt-br/centrais-de-conteudo/manuais-do-portal-operadoras/sib-manual-de-instalacao-historico-de-versao-e-outros-arquivos/manual/algoritmos-do-aplicativo-de-carga`
- ANS, field-filling critiques for PIS/PASEP length and verifier:
  `https://www.gov.br/ans/pt-br/centrais-de-conteudo/manuais-do-portal-operadoras/sib-manual-de-instalacao-historico-de-versao-e-outros-arquivos/manual/criticas-de-preenchimento-dos-campos`
- INSS, inscription relationship between NIT/PIS/PASEP/NIS:
  `https://www.gov.br/inss/pt-br/direitos-e-deveres/inscricao-e-contribuicao/inscricao`
