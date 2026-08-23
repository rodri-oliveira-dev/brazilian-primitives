# NIT

`Nit` represents the Numero de Identificacao do Trabalhador as an 11-digit ASCII string.

```csharp
Nit nit = Nit.Parse("12345678901");

Console.WriteLine(nit.Value); // 12345678901
```

The implementation is intentionally `format-only`: official sources consulted for this issue distinguish NIT from
PIS, PASEP, and NIS and describe NIT as 11 digits with a verifier, but the library does not embed a NIT check-digit
formula because the full weights, modulo, and remainder treatment were not confirmed in a sufficiently complete
official source for this primitive.

`Nit.IsValid` therefore means only that the text has the supported structure. It does not prove check-digit
correctness, CNIS existence, social-security affiliation, ownership, contributions, benefit rights, or active status.
The parser does not query Meu INSS or CNIS.

References consulted on 2026-08-23:

- INSS, Inscricao: `https://www.gov.br/inss/pt-br/direitos-e-deveres/inscricao-e-contribuicao/inscricao`
- Governo Federal/CMRI decision distinguishing NIT, PIS, PASEP, and NIS and describing 11 digits plus verifier:
  `https://www.gov.br/casacivil/pt-br/assuntos/colegiados/comissao-mista-de-reavaliacao-de-informacoes-cmri/decisoes-de-recurso-de-4a-instancia/2022/decisao-185-2022-nup-03005-187594-2022-16.pdf`
