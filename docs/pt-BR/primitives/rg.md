# RG

`Rg` representa o Registro Geral estadual legado. Ele suporta uso sem contexto de UF e uso state-aware porque muitos
bancos legados armazenam o identificador sem uma UF emissora confiável.

## Contrato

| Aspecto | Comportamento |
| --- | --- |
| Valor canônico | texto legado sem máscara |
| Modo sem UF | 6 a 10 caracteres, dígitos ASCII, com `X` final opcional somente em valor de 9 caracteres |
| UF ausente | `State == BrazilianState.Unknown` e `HasState == false` |
| Modo com UF | `BrazilianState` explícito; aplicam-se estrutura estadual e regras de DV conhecidas |
| São Paulo | 8 dígitos + DV numérico ou `X`; máscara `12.030.001-1` |
| Rio de Janeiro | 9 dígitos; máscara opcional `12.345.678-9`; format-only |
| Minas Gerais | 8 dígitos; aceita prefixos `MG-` e `M-`; format-only |
| Santa Catarina | 9 dígitos; máscara opcional `123.456.789`; format-only |
| Demais UFs | tamanho documentado em dígitos |

## Uso sem UF

```csharp
Rg rg = Rg.Parse("123456789");

Console.WriteLine(rg.Value);    // 123456789
Console.WriteLine(rg.State);    // Unknown
Console.WriteLine(rg.HasState); // False
```

A validação sem UF é deliberadamente estrutural e format-only. Ela não infere UF, máscara estadual ou dígito
verificador estadual. Por isso, um texto estruturalmente aceitável sem UF ainda pode ser rejeitado quando analisado
explicitamente como RG de São Paulo, caso o DV paulista esteja incorreto.

## Uso com UF

```csharp
Rg rg = Rg.Parse("12.030.001-1", BrazilianState.SaoPaulo);

Console.WriteLine(rg.Value);     // 120300011
Console.WriteLine(rg.Formatted); // 12.030.001-1
Console.WriteLine(rg.State);     // SaoPaulo
Console.WriteLine(rg.HasState);  // True
```

A igualdade preserva o contexto. Um RG sem UF não é igual ao mesmo valor canônico com UF conhecida, e o mesmo texto
associado a UFs diferentes representa valores diferentes.

## Entity Framework Core SQL Server

Para valores sem UF, `RgValueConverter` persiste somente o valor canônico em uma coluna `varchar(10)`. O converter
rejeita instâncias que tenham UF conhecida em vez de descartar silenciosamente esse contexto.

Para valores state-aware, use `RgStateAwareSqlServerMapping` em uma complex property do EF Core. O mapeamento persiste o
valor canônico do RG e a UF em colunas separadas; a UF usa um código estável de duas letras, como `SP` ou `RJ`.

A nulabilidade da propriedade é independente do contexto estadual: `Rg? == null` significa que o identificador está
ausente; um `Rg` não nulo com `State == BrazilianState.Unknown` significa que o identificador existe, mas a UF não foi
informada.

Este tipo não representa a CIN, cujo número nacional é o CPF. A validação não comprova existência, autenticidade,
titularidade ou situação do documento.
