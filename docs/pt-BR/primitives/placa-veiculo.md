# PlacaVeiculo

`PlacaVeiculo` modela a sequência textual de sete caracteres da placa brasileira.

## Contrato

| Aspecto | Comportamento |
| --- | --- |
| Valor canônico | 7 letras/dígitos ASCII em maiúsculas |
| Padrão nacional anterior | `ABC1234` ou `ABC-1234` |
| Padrão Mercosul/PIV | `ABC1D23` sem hífen |
| Normalização | letras ASCII minúsculas viram maiúsculas |
| Formatação | padrão anterior usa `ABC-1234`; Mercosul fica sem máscara |

```csharp
PlacaVeiculo placa = PlacaVeiculo.Parse("abc-1234");

Console.WriteLine(placa.Value);     // ABC1234
Console.WriteLine(placa.Formatted); // ABC-1234
Console.WriteLine(placa.Padrao);    // NacionalAnterior
```

`ConverterParaPadraoMercosul()` aplica a tabela `0..9 -> A..J` apenas para placas do padrão anterior.

O tipo não infere categoria, cor, dimensão, quantidade de placas, atribuição, existência, regularidade, QR Code ou
relação com RENAVAM/chassi.
