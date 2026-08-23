# CNS

`Cns` representa o Cartão Nacional de Saúde como valor de 15 dígitos com algoritmos por família.

## Contrato

| Aspecto | Comportamento |
| --- | --- |
| Valor canônico | 15 dígitos ASCII |
| Entrada aceita | somente sem máscara |
| Prefixos suportados | `1`, `2`, `7`, `8`, `9` |
| Algoritmos | famílias beneficiárias `1`/`2`; famílias provisórias `7`/`8`/`9` |
| Formatação | `ToString()` retorna o valor canônico |

```csharp
Cns cns = Cns.Parse("123456789010000");

Console.WriteLine(cns.Value); // 123456789010000
```

Valores com todos os dígitos zero e prefixos não suportados são rejeitados.

A validação não consulta CADSUS ou Meu SUS Digital e não comprova titularidade, CNS principal, vínculo com CPF,
qualidade cadastral ou direito a atendimento.
