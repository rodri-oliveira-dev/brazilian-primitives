# CNS

`Cns` represents the Cartao Nacional de Saude as exactly 15 ASCII digits.

Supported families:

- prefixes `1` and `2`: the first 11 digits are used with weights `15..5`; the final four digits are composed as
  `000D` or, when the first result is `10`, as `001D` after adding 2 to the weighted sum;
- prefixes `7`, `8`, and `9`: all 15 digits are multiplied by weights `15..1`, and the weighted sum must be divisible
  by 11.

```csharp
Cns cns = Cns.Parse("123456789010000");

Console.WriteLine(cns.Value); // 123456789010000
```

The parser does not accept masks, spaces, Unicode digits, or embedded text. It also does not treat prefix `7` as the
only valid family: historical/provisional families `1`, `2`, `8`, and `9` remain supported when their mathematical
rules pass.

Validation is mathematical only. It does not query CADSUS or Meu SUS Digital and does not prove existence, ownership,
whether the number is the main CNS, duplicate linkage, CPF linkage, cadastral quality, or entitlement to care.

References consulted on 2026-08-23:

- DATASUS, Cartao Nacional de Saude: `https://datasus.saude.gov.br/cartao-nacional-de-saude/`
- DATASUS, CNS stored as `VARCHAR2(15)`: `https://datasus.saude.gov.br/faq/quais-sao-as-classes-de-dados/`
- ANS, Algoritmos do Aplicativo de Carga, CNS validation:
  `https://www.gov.br/ans/pt-br/centrais-de-conteudo/manuais-do-portal-operadoras/sib-manual-de-instalacao-historico-de-versao-e-outros-arquivos/manual/algoritmos-do-aplicativo-de-carga`
- Ministerio da Saude, multiple CNS and main CNS explanation:
  `https://www.gov.br/saude/pt-br/composicao/seidigi/meususdigital/perguntas-e-respostas/cidadao/11-por-que-tenho-dois`
