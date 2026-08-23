# CNPJ

`Cnpj` is an immutable value object for Brazilian Cadastro Nacional da Pessoa Jurídica numbers.

The type accepts both the traditional numeric CNPJ and the alphanumeric format introduced by Receita Federal in 2026. Both formats use the same domain type and the same 14-position canonical representation.

Validation is local and deterministic. It confirms structure and verification digits, but does **not** confirm that the CNPJ exists at Receita Federal, belongs to a legal entity, is active, or has a particular cadastral status.

## Create a numeric CNPJ

```csharp
using Brazilian.Primitives;

Cnpj cnpj = Cnpj.Parse("11.222.333/0001-81");

Console.WriteLine(cnpj.Value);     // 11222333000181
Console.WriteLine(cnpj.Formatted); // 11.222.333/0001-81
```

The canonical unmasked representation is also accepted:

```csharp
Cnpj cnpj = Cnpj.Parse("11222333000181");
```

Leading zeros are preserved:

```csharp
Cnpj cnpj = Cnpj.Parse("04.252.011/0001-10");

Console.WriteLine(cnpj.Value); // 04252011000110
```

## Create an alphanumeric CNPJ

The first alphanumeric CNPJ officially issued by Receita Federal on July 31, 2026 was `00.000.000/E08G-12`:

```csharp
Cnpj cnpj = Cnpj.Parse("00.000.000/E08G-12");

Console.WriteLine(cnpj.Value);     // 00000000E08G12
Console.WriteLine(cnpj.Formatted); // 00.000.000/E08G-12
```

The unmasked representation is accepted as well:

```csharp
Cnpj cnpj = Cnpj.Parse("00000000E08G12");
```

ASCII lowercase letters are accepted in the first 12 positions and normalized deterministically to uppercase:

```csharp
Cnpj cnpj = Cnpj.Parse("00.000.000/e08g-12");

Console.WriteLine(cnpj.Value); // 00000000E08G12
```

## Validate without exceptions

```csharp
if (Cnpj.TryParse(input, out Cnpj cnpj))
{
    Console.WriteLine(cnpj.Value);
}
```

For boolean-only validation:

```csharp
bool numeric = Cnpj.IsValid("11.222.333/0001-81");
bool alphanumeric = Cnpj.IsValid("00.000.000/E08G-12");
```

## Supported structure

The canonical CNPJ always contains 14 positions:

```text
AAAAAAAAAAAADD
```

- positions 1 through 12 accept ASCII `0-9` and `A-Z`;
- lowercase ASCII `a-z` is normalized to uppercase on input;
- positions 13 and 14 are numeric verification digits only.

The canonical mask is:

```text
AA.AAA.AAA/AAAA-DD
```

Examples:

```text
11222333000181
11.222.333/0001-81
00000000E08G12
00.000.000/E08G-12
```

Parsing is intentionally strict. Arbitrary characters are not removed before validation. Accented letters, Unicode lookalike digits, internal spaces, unsupported symbols, and non-canonical punctuation are rejected.

## Verification digits

For the first 12 positions, Receita Federal defines the numeric value used by the modulo-11 calculation as:

```text
value = uppercase ASCII code - 48
```

Therefore:

- `0-9` map to `0-9`;
- `A-Z` map to `17-42`.

The first verification digit uses weights `5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2`.
The second uses `6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2`.

For each digit, if the weighted sum modulo 11 is `0` or `1`, the verification digit is `0`; otherwise it is `11 - remainder`.

## Formatting

```csharp
Cnpj cnpj = Cnpj.Parse("00000000E08G12");

cnpj.ToString();           // 00000000E08G12
cnpj.ToString("G", null); // 00000000E08G12
cnpj.ToString("F", null); // 00.000.000/E08G-12
```

## Official references

- Receita Federal — CNPJ Alfanumérico: https://www.gov.br/receitafederal/pt-br/acesso-a-informacao/acoes-e-programas/programas-e-atividades/cnpj-alfanumerico
- Receita Federal — documentação técnica do cálculo do DV: https://www.gov.br/receitafederal/pt-br/centrais-de-conteudo/publicacoes/documentos-tecnicos/cnpj
- Receita Federal — primeiro CNPJ alfanumérico emitido: https://www.gov.br/receitafederal/pt-br/assuntos/noticias/2026/julho/receita-federal-gera-o-primeiro-cnpj-em-formato-alfanumerico
