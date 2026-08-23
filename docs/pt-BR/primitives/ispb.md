# ISPB

`Ispb` representa o Identificador do Sistema de Pagamentos Brasileiro/STR.

## Contrato

| Aspecto | Comportamento |
| --- | --- |
| Valor canônico | 8 dígitos ASCII |
| Entrada aceita | somente sem máscara |
| Normalização | preserva os dígitos |
| Dígitos verificadores | nenhum |
| Modo de validação | somente estrutural |

```csharp
Ispb ispb = Ispb.Parse("12345678");

Console.WriteLine(ispb.Value); // 12345678
```

O parser não deriva ISPB de CNPJ e não converte entre ISPB e COMPE.

A validação não comprova autorização pelo Banco Central, existência do participante, participação atual em STR/Pix/COMPE,
conta de liquidação ou relação com CNPJ/status.
