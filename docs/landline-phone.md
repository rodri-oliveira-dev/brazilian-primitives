# Telefone fixo geográfico

`LandlinePhone` representa um número brasileiro de telefonia fixa geográfica (STFC) conforme as regras estáveis do plano de numeração da Anatel.

A validação é local e estrutural. Um valor aceito pela biblioteca **não comprova** que a linha exista, esteja ativa, pertença a uma pessoa ou esteja vinculada a uma operadora específica.

## Exemplo

```csharp
using Brazilian.Primitives;

LandlinePhone phone = LandlinePhone.Parse("(11) 3234-5678");

Console.WriteLine(phone.Value);            // 1132345678
Console.WriteLine(phone.AreaCode);         // 11
Console.WriteLine(phone.SubscriberNumber); // 32345678
Console.WriteLine(phone.Formatted);        // (11) 3234-5678
Console.WriteLine(phone.E164);              // +551132345678
```

## Estrutura

A representação canônica nacional contém exatamente 10 dígitos:

```text
DD + XXXXXXXX
```

onde:

- `DD` é um Código Nacional/DDD de dois dígitos atribuído pela Anatel;
- o número do assinante possui oito dígitos;
- o primeiro dígito do assinante deve ser `2`, `3`, `4` ou `5` para telefonia fixa.

A faixa iniciada por `57` continua pertencendo ao STFC em numeração destinada à telefonia rural. A implementação, portanto, não impõe uma restrição adicional ao segundo dígito quando o assinante começa por `5`.

Referências oficiais:

- Anatel — Plano de Numeração Brasileiro: https://www.gov.br/anatel/pt-br/regulado/numeracao/plano-de-numeracao-brasileiro
- Anatel — Perguntas Frequentes de Numeração: https://www.gov.br/anatel/pt-br/regulado/numeracao/perguntas-frequentes
- Anatel — Ato nº 12712/2024, com a reserva da faixa `57` para STFC fora da Área de Tarifa Básica: https://informacoes.anatel.gov.br/legislacao/pesquisar?searchword=12712

## DDDs aceitos

A relação de Códigos Nacionais fica centralizada no componente interno `BrazilianAreaCode`, que será reutilizável por outros Value Objects telefônicos, como `MobilePhone`.

Os 67 DDDs atualmente considerados são:

```text
11 12 13 14 15 16 17 18 19
21 22 24 27 28
31 32 33 34 35 37 38
41 42 43 44 45 46 47 48 49
51 53 54 55
61 62 63 64 65 66 67 68 69
71 73 74 75 77 79
81 82 83 84 85 86 87 88 89
91 92 93 94 95 96 97 98 99
```

A lista é cross-checkada contra a publicação atual da Anatel sobre as Áreas de Numeração do STFC, que consolida os Códigos Nacionais usados em todo o país:

- https://www.gov.br/anatel/pt-br/regulado/competicao/tarifas-e-precos/areas-locais-da-telefonia-fixa
- https://www.gov.br/anatel/pt-br/regulado/numeracao/codigos-nacionais

## Formatos aceitos

A biblioteca reconhece apenas formatos explícitos e determinísticos.

### Nacional canônico

```text
1132345678
```

### Nacional formatado

```text
(11) 3234-5678
```

### Internacional legível

```text
+55 11 3234-5678
```

### E.164

```text
+551132345678
```

Todas as quatro representações acima são normalizadas para:

```text
1132345678
```

Não há remoção genérica de caracteres. Por exemplo:

```csharp
LandlinePhone.IsValid("abc11xyz3234-5678"); // false
LandlinePhone.IsValid("(11)3234-5678");      // false
LandlinePhone.IsValid("+55 (11) 3234-5678"); // false
```

## Formatação

```csharp
LandlinePhone phone = LandlinePhone.Parse("1132345678");

phone.ToString();          // 1132345678
phone.ToString("G", null); // 1132345678
phone.ToString("F", null); // (11) 3234-5678
phone.ToString("E", null); // +551132345678
```

`G` representa o formato nacional canônico, `F` o formato nacional para exibição e `E` o formato internacional E.164.

## O que é rejeitado

`LandlinePhone` rejeita deliberadamente:

- DDDs não atribuídos;
- ausência de DDD;
- assinantes iniciados por `6`, `7`, `8` ou `9`;
- números móveis;
- códigos não geográficos como `0300`, `0500`, `0800` e `0900`;
- códigos de utilidade pública e emergência;
- código de país diferente de `+55`;
- código de seleção de prestadora;
- ramais internos de PABX;
- máscaras ou pontuação fora dos formatos documentados;
- letras, espaços extras e dígitos Unicode semelhantes aos ASCII.

Os códigos não geográficos possuem plano próprio e não são DDD + assinante geográfico. A Anatel documenta esses recursos separadamente em:

- https://www.gov.br/anatel/pt-br/regulado/numeracao/codigos-nacionais/codigos-nao-geograficos

## Limites da validação

`LandlinePhone.IsValid(...)` significa somente que o texto:

1. usa um formato explicitamente suportado;
2. possui um DDD atualmente reconhecido pela regra centralizada;
3. possui oito dígitos de assinante;
4. pertence estruturalmente à faixa de telefonia fixa pelo primeiro dígito.

A biblioteca não consulta Anatel, ABR Telecom, portabilidade ou operadoras em runtime e não tenta inferir existência, ativação, titularidade ou prestadora atual.
