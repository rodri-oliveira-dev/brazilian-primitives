# InscricaoEstadual

`InscricaoEstadual` representa uma inscrição estadual de ICMS e suporta uso sem contexto de UF e uso state-aware para
schemas legados que nem sempre possuem uma UF confiável.

## Contrato

| Aspecto | Comportamento |
| --- | --- |
| Valor canônico | dígitos ASCII sem máscara |
| Modo sem UF | 8 a 14 dígitos ASCII; validação estrutural/format-only |
| UF ausente | `State == BrazilianState.Unknown` e `HasState == false` |
| Modo com UF | `BrazilianState` explícito; aplicam-se os tamanhos documentados para a UF |
| Entrada aceita | somente sem máscara |
| Dígitos verificadores | não implementados na matriz estadual atual |

## Uso sem UF

```csharp
InscricaoEstadual ie = InscricaoEstadual.Parse("0012345678");

Console.WriteLine(ie.Value);    // 0012345678
Console.WriteLine(ie.State);    // Unknown
Console.WriteLine(ie.HasState); // False
```

A validação sem UF aceita apenas 8 a 14 dígitos ASCII. Ela não infere UF nem aplica tamanho ou regra de checksum
específicos de um estado. Zeros à esquerda são preservados. `ISENTO` continua inválido porque representa uma condição
fiscal, não um identificador.

## Uso com UF

```csharp
InscricaoEstadual ie = InscricaoEstadual.Parse("110042490114", BrazilianState.SaoPaulo);

Console.WriteLine(ie.Value);    // 110042490114
Console.WriteLine(ie.State);    // SaoPaulo
Console.WriteLine(ie.HasState); // True
```

Bahia, Pernambuco e Rio Grande do Norte aceitam dois tamanhos documentados; as demais UFs aceitam um tamanho. A
igualdade preserva o contexto: uma inscrição sem UF não é igual aos mesmos dígitos canônicos com UF conhecida, e os
mesmos dígitos associados a UFs diferentes representam valores diferentes.

## Entity Framework Core SQL Server

Para valores sem UF, `InscricaoEstadualValueConverter` persiste o valor canônico em uma coluna `varchar(14)`. Ele
rejeita uma instância com UF conhecida em vez de descartar silenciosamente esse contexto.

Para valores state-aware, use `InscricaoEstadualStateAwareSqlServerMapping` em uma complex property do EF Core. O
mapeamento persiste a inscrição canônica e a UF em colunas separadas; a UF usa um código estável de duas letras, como
`SP` ou `RO`.

A nulabilidade da propriedade é independente do contexto estadual: `InscricaoEstadual? == null` significa que o
identificador está ausente; um valor não nulo com `State == BrazilianState.Unknown` significa que a inscrição existe,
mas sua UF não foi informada.

A validação não consulta SINTEGRA ou SEFAZ e não comprova existência do contribuinte, status ativo, autorização de
NF-e, regularidade fiscal ou relação com CPF/CNPJ.
