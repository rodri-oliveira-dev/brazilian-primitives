# ISPB

`Ispb` represents the Identificador do Sistema de Pagamentos Brasileiro as exactly 8 ASCII digits.

```csharp
Ispb ispb = Ispb.Parse("12345678");

Console.WriteLine(ispb.Value); // 12345678
```

The value is stored as `string` to preserve leading zeros. The parser does not derive ISPB from `Cnpj`, does not accept
alphanumeric CNPJ bases, and does not create any conversion to or from `CodigoCompe`.

`Ispb.IsValid` is structural only. It does not prove that the participant exists, is authorized by Banco Central,
currently participates in STR/Pix/COMPE, has an active settlement account, or has any specific cadastral status.

References consulted on 2026-08-23:

- Banco Central, Tutorial de Acesso ao STR, ISPB with eight digits:
  `https://www.bcb.gov.br/content/estabilidadefinanceira/estabilidade_docs/Tutorial_Internet_STR.pdf`
- Banco Central, STR documentation: `https://www.bcb.gov.br/estabilidadefinanceira/str`
