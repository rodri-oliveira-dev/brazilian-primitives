# InscricaoEstadual

`InscricaoEstadual` representa uma inscrição estadual de ICMS com contexto explícito de `BrazilianState`.

## Contrato

| Aspecto | Comportamento |
| --- | --- |
| Valor canônico | dígitos ASCII com tamanho específico por UF |
| Contexto obrigatório | `BrazilianState` |
| Entrada aceita | somente sem máscara |
| Dígitos verificadores | não implementados na matriz atual |
| Modo de validação | format-only por tamanho estadual |

```csharp
InscricaoEstadual ie = InscricaoEstadual.Parse("110042490114", BrazilianState.SaoPaulo);

Console.WriteLine(ie.Value); // 110042490114
Console.WriteLine(ie.State); // SaoPaulo
```

Bahia, Pernambuco e Rio Grande do Norte aceitam dois tamanhos documentados; as demais UFs aceitam um tamanho.
`ISENTO` é rejeitado porque é condição fiscal, não identificador.

A validação não consulta SINTEGRA ou SEFAZ e não comprova existência do contribuinte, status ativo, autorização de
NF-e, regularidade fiscal ou relação com CPF/CNPJ.
