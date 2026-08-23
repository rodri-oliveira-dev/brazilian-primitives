# RG legado por UF

`Rg` representa o Registro Geral estadual no modelo legado. Ele não representa a Carteira de Identidade Nacional (CIN).

O RG legado não possui um formato nem um algoritmo de dígito verificador nacional. Por isso, a UF emissora é obrigatória e faz parte da identidade do Value Object.

```csharp
using Brazilian.Primitives;

Rg rg = Rg.Parse("12.030.001-1", BrazilianState.SaoPaulo);

Console.WriteLine(rg.Value);     // 120300011
Console.WriteLine(rg.Formatted); // 12.030.001-1
Console.WriteLine(rg.State);     // SaoPaulo
```

## O que `IsValid` significa

`Rg.IsValid` verifica apenas a coerência com a regra local conhecida para a UF informada:

- em São Paulo, a validação inclui o dígito verificador legado do SSP/IIRGD;
- nas demais UFs desta versão, a validação é **format-only**: tamanho, caracteres e máscaras explicitamente suportadas;
- nenhum resultado confirma existência, autenticidade, titularidade ou situação do documento em bases governamentais.

Quando uma UF não possui algoritmo de DV publicado com confiança suficiente para esta biblioteca, nenhum checksum é inventado.

## CIN não é RG legado

A Carteira de Identidade Nacional utiliza o CPF como número único de identificação nacional. O tipo `Rg` não tenta interpretar CPF/CIN como RG estadual e não executa validação de CPF.

O modelo antigo de carteira de identidade permanece válido durante a transição prevista pelo Decreto nº 10.977/2022, até 28 de fevereiro de 2032.

Referências federais:

- https://www.gov.br/gestao/pt-br/assuntos/identidade/identidade-nacional
- https://www.gov.br/governodigital/pt-br/identidade/cin/perguntas-frequentes-sobre-a-cin

## Parsing estrito

A biblioteca não remove arbitrariamente caracteres para produzir um RG aparentemente válido.

Exemplos:

```csharp
Rg.IsValid("12.030.001-1", BrazilianState.SaoPaulo); // true
Rg.IsValid("12-030-001.1", BrazilianState.SaoPaulo); // false
Rg.IsValid(" 12.030.001-1", BrazilianState.SaoPaulo); // false
```

Somente máscaras conhecidas pela estratégia da UF são aceitas. UFs sem máscara suportada devem receber a representação canônica sem pontuação.

## Cobertura por UF

A tabela abaixo documenta todas as 27 UFs. `checksum` significa que existe validação matemática implementada; `format-only` significa que a biblioteca deliberadamente não afirma validar DV.

| UF | `BrazilianState` | Canônico aceito | Máscara conhecida suportada | Modo | Referência do emissor / pesquisa |
| --- | --- | --- | --- | --- | --- |
| AC | `Acre` | 6 dígitos | — | format-only | https://www.policiacivil.ac.gov.br/ |
| AL | `Alagoas` | 7 dígitos | — | format-only | https://alagoasdigital.al.gov.br/servico/8 |
| AP | `Amapa` | 9 dígitos | — | format-only | https://apdigital.portal.ap.gov.br/carta-de-servico/solicitacao-de-agendamento-para-emissao-da-1o-via-da-carteira-de-identidade-nacional-cin1 |
| AM | `Amazonas` | 9 dígitos | — | format-only | https://www.ssp.am.gov.br/instituto-de-identificacao-tira-duvidas-sobre-emissao-de-documentos/ |
| BA | `Bahia` | 10 dígitos | — | format-only | https://www.ba.gov.br/policiatecnica/972/instituto-de-identificacao-pedro-mello-iipm |
| CE | `Ceara` | 9 dígitos | — | format-only | https://www.policiacivil.ce.gov.br/ |
| DF | `DistritoFederal` | 7 dígitos | — | format-only | https://www.nahora.df.gov.br/policia_civil/ |
| ES | `EspiritoSanto` | 9 dígitos | — | format-only | https://pci.es.gov.br/perguntas-frequentes |
| GO | `Goias` | 9 dígitos | — | format-only | https://identificacao.policiacivil.go.gov.br/1a-via-do-rg-goias/ |
| MA | `Maranhao` | 9 dígitos | — | format-only | https://www.ma.gov.br/servicos/obter-1-via-do-rg-agendamento-on-line |
| MT | `MatoGrosso` | 9 dígitos | — | format-only | https://www.politec.mt.gov.br/ |
| MS | `MatoGrossoDoSul` | 9 dígitos | — | format-only | https://servicos.sejusp.ms.gov.br/ |
| MG | `MinasGerais` | 8 dígitos; prefixo `M` opcional | `1.234.567-8`; `M1.234.567-8` | format-only | https://www.policiacivil.mg.gov.br/pagina/servicos-identificacao |
| PA | `Para` | 9 dígitos | — | format-only | https://www.policiacivil.pa.gov.br/ |
| PB | `Paraiba` | 9 dígitos | — | format-only | https://agendamentos.pb.gov.br/SAA/ipc/home |
| PR | `Parana` | 8 dígitos | — | format-only | https://www.iipar.pr.gov.br/ |
| PE | `Pernambuco` | 9 dígitos | — | format-only | https://www.policiacivil.pe.gov.br/ |
| PI | `Piaui` | 9 dígitos | — | format-only | https://www.policiacivil.pi.gov.br/ |
| RJ | `RioDeJaneiro` | 8 dígitos | `1.234.567-8` | format-only | https://www.detran.rj.gov.br/todos-os-servicos/servicos-dic/carteira-de-identidade-nacional-cin.html |
| RN | `RioGrandeDoNorte` | 9 dígitos | — | format-only | https://www.policiacivil.rn.gov.br/ |
| RS | `RioGrandeDoSul` | 10 dígitos | — | format-only | https://www.estado.rs.gov.br/ |
| RO | `Rondonia` | 9 dígitos | — | format-only | https://www.policiacivil.ro.gov.br/ |
| RR | `Roraima` | 9 dígitos | — | format-only | https://www.policiacivil.rr.gov.br/ |
| SC | `SantaCatarina` | 9 dígitos | `123.456.789` | format-only | https://www.policiacientifica.sc.gov.br/ |
| SP | `SaoPaulo` | 8 dígitos + DV (`0-9` ou `X`) | `12.030.001-1` | checksum | https://www3.ssp.sp.gov.br/aacweb/carrega-formulario |
| SE | `Sergipe` | 9 dígitos | — | format-only | https://www.policiacivil.se.gov.br/ |
| TO | `Tocantins` | 9 dígitos | — | format-only | https://www.policiacivil.to.gov.br/ |

### Nota sobre São Paulo

O portal da SSP-SP confirma que o RG legado do IIRGD possui número e dígito de controle. Para o cálculo local adotado, a biblioteca usa a referência técnica histórica de DV SSP-SP consolidada em `http://ghiorzi.org/DVnew.htm`, com pesos `9, 8, 7, 6, 5, 4, 3, 2`, resto módulo 11 e `X` quando o resto é 10.

Vetores usados para cross-check:

```text
12.030.001-1
00.000.005-X
```

A regra de SP não é reutilizada como fallback para nenhuma outra UF.

## Máscaras conhecidas

```csharp
Rg sp = Rg.Parse("120300011", BrazilianState.SaoPaulo);
sp.Formatted; // 12.030.001-1

Rg rj = Rg.Parse("12345678", BrazilianState.RioDeJaneiro);
rj.Formatted; // 1.234.567-8

Rg mg = Rg.Parse("M12345678", BrazilianState.MinasGerais);
mg.Formatted; // M1.234.567-8

Rg sc = Rg.Parse("123456789", BrazilianState.SantaCatarina);
sc.Formatted; // 123.456.789
```

Para as demais UFs, `Formatted` retorna o mesmo conteúdo de `Value` nesta versão.

## Igualdade inclui a UF

```csharp
Rg amazonas = Rg.Parse("123456789", BrazilianState.Amazonas);
Rg amapa = Rg.Parse("123456789", BrazilianState.Amapa);

Console.WriteLine(amazonas == amapa); // false
```

O mesmo texto emitido em estados diferentes não representa automaticamente o mesmo RG.
