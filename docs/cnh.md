# CNH — Número do Registro Nacional

`Cnh` representa exclusivamente o **Número do Registro Nacional da Carteira Nacional de Habilitação**, composto por 9 dígitos-base e 2 dígitos verificadores de segurança.

```csharp
using Brazilian.Primitives;

Cnh cnh = Cnh.Parse("62472927637");

Console.WriteLine(cnh.Value);    // 62472927637
Console.WriteLine(cnh.ToString()); // 62472927637
```

## O que este tipo representa

A Resolução CONTRAN nº 886/2021 diferencia três identificadores existentes no contexto da CNH:

- **Número do Registro Nacional**: 9 posições + 2 DVs, único para o condutor;
- **Número do Espelho da CNH**: 9 posições + 1 DV e identifica cada espelho expedido;
- **Número do Formulário RENACH**: identificador estadual do formulário de coleta de dados.

`Cnh` modela somente o primeiro item. Ele também não representa o CPF do condutor, QR Code ou código numérico de segurança/validação do documento.

Referência oficial:

- CONTRAN — Resolução nº 886/2021: https://www.gov.br/transportes/pt-br/assuntos/transito/conteudo-contran/resolucoes/Resolucao88620212.pdf

## Formato aceito

A representação canônica possui exatamente 11 dígitos ASCII contíguos:

```text
62472927637
```

A biblioteca não inventa máscara e não sanitiza entrada arbitrária. Portanto:

```csharp
Cnh.IsValid("62472927637");  // true
Cnh.IsValid("624.729.276-37"); // false
Cnh.IsValid("624 729 276 37"); // false
Cnh.IsValid(" 62472927637"); // false
```

Zeros à esquerda são preservados porque o número é armazenado como `string`:

```csharp
Cnh cnh = Cnh.Parse("02650306461");
Console.WriteLine(cnh.Value); // 02650306461
```

## Algoritmo dos dígitos verificadores

A norma define a estrutura do Registro Nacional e seus dois dígitos verificadores. A rotina matemática adotada é o algoritmo público de módulo 11 historicamente utilizado para esse número e foi cross-checkada contra implementações e vetores independentes.

Para os nove dígitos-base `d1..d9`:

1. **DV1**
   - multiplicar a base pelos pesos decrescentes `9, 8, 7, 6, 5, 4, 3, 2, 1`;
   - calcular `soma % 11`;
   - resto `0..9` é o próprio DV1;
   - resto `10` gera DV1 `0` e ativa um **desconto de 2** para o cálculo do segundo DV.

2. **DV2**
   - multiplicar os mesmos nove dígitos-base pelos pesos crescentes `1, 2, 3, 4, 5, 6, 7, 8, 9`;
   - calcular `soma % 11`;
   - quando o desconto foi ativado pelo DV1, subtrair `2`; se o resultado ficaria negativo, fazer o ajuste modular equivalente;
   - resultado `10` é convertido para `0`.

O desconto é uma regra **interdependente entre os dois verificadores**. Ele não deve ser removido nem substituído por um módulo 11 genérico.

### Cross-checks

Vetores públicos usados para verificar a implementação:

```text
62472927637
69044271146
02650306461
04397322870
04375701302
02996843266
04375700501
```

Casos específicos que exercitam o desconto entre DVs:

```text
00000001801
00000009309
00000018200
```

Os últimos vetores são matemáticos para cobertura das ramificações da regra e não afirmam corresponder a registros emitidos.

Referências adicionais de cross-check:

- `br-validators`: https://github.com/open-data-brazil/br-validators/blob/main/packages/br-validators/src/core/cnh/check-digits.ts
- Respect Validation: https://github.com/Respect/Validation

## Validação não comprova existência

`Cnh.IsValid` realiza somente validação estrutural e matemática local. Um resultado `true` não comprova que:

- o número foi efetivamente emitido pela SENATRAN/DETRAN;
- o registro pertence a determinada pessoa;
- a habilitação está vigente;
- existe determinada categoria;
- o condutor não está suspenso ou cassado.

Essas verificações dependem de sistemas governamentais e estão fora do escopo do Core.

## Parsing

```csharp
if (Cnh.TryParse("62472927637", out Cnh cnh))
{
    Console.WriteLine(cnh.Value);
}
```

`Parse` lança `FormatException` para entrada inválida. `TryParse` retorna `false` sem lançar. O tipo implementa `IParsable<Cnh>` e `ISpanParsable<Cnh>` para integração com APIs genéricas de parsing do .NET.

Sequências com o mesmo dígito repetido em todas as 11 posições também são rejeitadas, mesmo quando uma combinação pudesse coincidir matematicamente com os verificadores.
