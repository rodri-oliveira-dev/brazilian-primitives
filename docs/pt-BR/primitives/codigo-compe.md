# CodigoCompe

`CodigoCompe` representa o contrato numérico atual do código COMPE de instituição.

## Contrato

| Aspecto | Comportamento |
| --- | --- |
| Valor canônico | 3 dígitos ASCII |
| Entrada aceita | somente sem máscara |
| Normalização | preserva os dígitos |
| Sentinela rejeitada | `999` |
| Modo de validação | somente estrutural |

```csharp
CodigoCompe codigo = CodigoCompe.Parse("001");

Console.WriteLine(codigo.Value); // 001
```

Zeros à esquerda são significativos. Ausência deve ser modelada como `null`, opcional ou campo ausente, não como `999`.

A validação não comprova atribuição, existência da instituição, participação atual no COMPE, validade de conta,
associação com ISPB ou status operacional.
