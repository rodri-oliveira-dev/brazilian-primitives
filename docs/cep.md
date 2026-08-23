# CEP

`Cep` representa a estrutura de um **Código de Endereçamento Postal brasileiro** no Core da biblioteca.

A validação é deliberadamente estrutural e local. Um valor aceito por `Cep.IsValid(...)` possui a forma de um CEP brasileiro, mas isso **não comprova** que o código esteja atualmente atribuído pelos Correios, exista no Diretório Nacional de Endereços (DNE) ou corresponda a um endereço específico.

## Estrutura

Os Correios definem o CEP como um conjunto numérico de **oito dígitos**.

Representação canônica:

```text
01311000
```

Representação formatada:

```text
01311-000
```

O zero à esquerda é significativo. Por isso, `Cep` armazena o valor como `string` e nunca converte o código para `int`, `long` ou outro tipo numérico.

A documentação da API Busca CEP dos Correios também utiliza oito dígitos sem hífen para consultas e destaca exemplos com zero à esquerda, como `01001001`.

Referências oficiais:

- Correios — Guia de Endereçamento: https://www.correios.com.br/enviar/precisa-de-ajuda/guia-de-enderecamento/guia-de-enderecamento
- Correios — Manual da API Busca CEP: https://www.correios.com.br/atendimento/developers/manuais/manual-api-busca-cep
- Correios — Manual de Integração Correios API: https://www.correios.com.br/atendimento/developers/arquivos/manual-para-integracao-correios-api

## Exemplo

```csharp
using Brazilian.Primitives;

Cep cep = Cep.Parse("01311-000");

Console.WriteLine(cep.Value);     // 01311000
Console.WriteLine(cep.Formatted); // 01311-000
Console.WriteLine(cep.ToString()); // 01311000
```

O exemplo `01311-000` é usado pelo próprio Guia de Endereçamento dos Correios.

## Formatos aceitos

A biblioteca reconhece somente duas representações explícitas:

### Sem máscara

```text
01311000
```

### Máscara canônica

```text
01311-000
```

Ambas normalizam para:

```text
01311000
```

Não há sanitização genérica. Por exemplo:

```csharp
Cep.IsValid("abc01311xyz000"); // false
Cep.IsValid("01311 000");      // false
Cep.IsValid("01311.000");      // false
```

Espaços extras, letras, símbolos fora da posição canônica e dígitos Unicode semelhantes aos ASCII também são rejeitados.

## Validação estrutural não é existência postal

`Cep.IsValid(...)` responde apenas se o texto possui uma das representações suportadas e contém exatamente oito dígitos ASCII.

Por exemplo:

```csharp
Cep.IsValid("00000000"); // true: estrutura válida; nenhuma afirmação sobre existência no DNE
```

A biblioteca não consulta os Correios durante `Parse`, `TryParse` ou `IsValid` e não mantém uma cópia local de faixas postais como fonte definitiva de existência.

Isso é importante porque a API oficial dos Correios possui um endpoint específico para consultar um CEP e recuperar seus dados. Esse lookup é uma responsabilidade diferente da validação estrutural do Value Object.

## Formatação

```csharp
Cep cep = Cep.Parse("01311000");

cep.ToString();          // 01311000
cep.ToString("G", null); // 01311000
cep.ToString("F", null); // 01311-000
```

`G` representa o valor canônico sem máscara e `F` a representação formatada.

## O que é rejeitado

`Cep` rejeita deliberadamente:

- menos ou mais de oito dígitos na forma não mascarada;
- hífen fora da sexta posição da representação formatada;
- pontos, espaços e outras máscaras não documentadas;
- letras e caracteres arbitrários misturados ao número;
- dígitos Unicode semelhantes aos ASCII;
- `null`, vazio e whitespace;
- qualquer tentativa de sanitização permissiva.

## O que não é validado

O Core não tenta determinar se o CEP:

- existe atualmente no DNE;
- está atribuído a uma localidade ou logradouro;
- pertence a determinada cidade ou UF;
- corresponde a uma unidade dos Correios, grande usuário, caixa postal ou CEP promocional;
- continua vigente em uma base postal atual.

Uma futura integração com serviços dos Correios pode oferecer consulta de existência/endereço em outro pacote sem alterar a semântica estrutural de `Cep`.
