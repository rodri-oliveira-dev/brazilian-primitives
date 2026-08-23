# Telefone celular brasileiro

`MobilePhone` representa um número brasileiro do Serviço Móvel Pessoal (SMP) conforme as regras estáveis do plano de numeração da Anatel e a implantação nacional do nono dígito.

A validação é local e estrutural. Um valor aceito pela biblioteca **não comprova** que a linha exista, esteja ativa, pertença a uma pessoa, possa receber chamadas ou mensagens, ou esteja vinculada a uma operadora específica.

## Exemplo

```csharp
using Brazilian.Primitives;

MobilePhone phone = MobilePhone.Parse("(11) 98765-4321");

Console.WriteLine(phone.Value);            // 11987654321
Console.WriteLine(phone.AreaCode);         // 11
Console.WriteLine(phone.SubscriberNumber); // 987654321
Console.WriteLine(phone.Formatted);        // (11) 98765-4321
Console.WriteLine(phone.E164);              // +5511987654321
```

## Estrutura

A representação canônica nacional contém exatamente 11 dígitos:

```text
DD + 9XXXXXXXX
```

onde:

- `DD` é um Código Nacional/DDD de dois dígitos atribuído pela Anatel;
- o número do assinante móvel possui nove dígitos;
- o primeiro dígito do assinante deve ser obrigatoriamente `9`.

A Anatel informa que a migração nacional do nono dígito foi concluída em 14 de fevereiro de 2017. Desde então, os números de telefonia celular seguem o formato `9XXXX-XXXX`.

Referências oficiais:

- Anatel — Nono Dígito: https://www.gov.br/anatel/pt-br/regulado/numeracao/codigos-nacionais/nono-digito
- Anatel — Plano de Numeração Brasileiro: https://www.gov.br/anatel/pt-br/regulado/numeracao/plano-de-numeracao-brasileiro
- Anatel — Tabela Serviço Móvel Pessoal (SMP): https://www.gov.br/anatel/pt-br/regulado/numeracao/tabela-servico-movel-celular

## DDDs aceitos

`MobilePhone` reutiliza o componente interno `BrazilianAreaCode`, introduzido por `LandlinePhone`, em vez de manter outra lista de DDDs.

Os 67 Códigos Nacionais atualmente considerados são:

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

A relação é cross-checkada contra as publicações da Anatel:

- Códigos Nacionais: https://www.gov.br/anatel/pt-br/regulado/numeracao/codigos-nacionais
- Tabela SMP: https://www.gov.br/anatel/pt-br/regulado/numeracao/tabela-servico-movel-celular

## Formatos aceitos

A biblioteca reconhece apenas formatos explícitos e determinísticos.

### Nacional canônico

```text
11987654321
```

### Nacional formatado

```text
(11) 98765-4321
```

### Internacional legível

```text
+55 11 98765-4321
```

### E.164

```text
+5511987654321
```

Todas as quatro representações acima são normalizadas para:

```text
11987654321
```

Não há remoção genérica de caracteres. Por exemplo:

```csharp
MobilePhone.IsValid("abc11xyz98765-4321"); // false
MobilePhone.IsValid("(11)98765-4321");      // false
MobilePhone.IsValid("+55 (11) 98765-4321"); // false
```

## Formatação

```csharp
MobilePhone phone = MobilePhone.Parse("11987654321");

phone.ToString();           // 11987654321
phone.ToString("G", null); // 11987654321
phone.ToString("F", null); // (11) 98765-4321
phone.ToString("E", null); // +5511987654321
```

`G` representa o formato nacional canônico, `F` o formato nacional para exibição e `E` o formato internacional E.164.

## Formato móvel antigo

Números móveis de oito dígitos não são aceitos, mesmo que possam aparecer em bases históricas ou bibliotecas legadas.

```csharp
MobilePhone.IsValid("(11) 8765-4321"); // false
MobilePhone.IsValid("1187654321");     // false
```

Da mesma forma, um assinante atual com nove posições começando por `6`, `7` ou `8` não é tratado como SMP por este Value Object. A regra estável usada é o formato atual `9XXXX-XXXX` documentado pela Anatel.

## Telefones fixos

Faixas iniciadas por `2`, `3`, `4` ou `5` pertencem ao domínio de `LandlinePhone`, não de `MobilePhone`.

```csharp
MobilePhone.IsValid("11323456789"); // false
```

## Operadora e portabilidade

`MobilePhone` não tenta descobrir ou validar operadora a partir do prefixo. Com portabilidade numérica e gestão dinâmica das faixas, prefixos de prestadora não fazem parte do requisito de validade do Value Object.

A validação usa apenas características estáveis do plano:

1. formato explicitamente suportado;
2. DDD reconhecido pela regra compartilhada;
3. nove dígitos de assinante;
4. primeiro dígito `9`.

## O que é rejeitado

`MobilePhone` rejeita deliberadamente:

- DDDs não atribuídos;
- ausência de DDD;
- assinantes móveis históricos com oito dígitos;
- assinantes atuais iniciados por `2` a `8`;
- telefones fixos;
- códigos não geográficos como `0300`, `0500`, `0800` e `0900`;
- códigos de utilidade pública e emergência;
- código de país diferente de `+55`;
- código de seleção de prestadora;
- máscaras ou pontuação fora dos formatos documentados;
- letras, espaços extras e dígitos Unicode semelhantes aos ASCII.

Os códigos não geográficos são recursos separados do plano de numeração:

- https://www.gov.br/anatel/pt-br/regulado/numeracao/codigos-nacionais/codigos-nao-geograficos

## Limites da validação

`MobilePhone.IsValid(...)` não consulta Anatel, ABR Telecom, portabilidade ou operadoras em runtime e não tenta inferir existência, ativação, titularidade, alcance, capacidade de receber SMS/WhatsApp ou prestadora atual/original.
