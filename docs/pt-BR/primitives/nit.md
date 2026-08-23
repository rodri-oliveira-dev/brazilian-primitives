# NIT

`Nit` representa o Número de Identificação do Trabalhador no contrato estrutural atual.

## Contrato

| Aspecto | Comportamento |
| --- | --- |
| Valor canônico | 11 dígitos ASCII |
| Entrada aceita | somente sem máscara |
| Normalização | preserva os dígitos |
| Dígitos verificadores | não implementados para NIT |
| Modo de validação | somente estrutural |

```csharp
Nit nit = Nit.Parse("12345678901");

Console.WriteLine(nit.Value); // 12345678901
```

O tipo é separado de `PisPasep` e não trata NIT, PIS, PASEP e NIS como intercambiáveis.

A validação não comprova existência no CNIS, inscrição previdenciária, titularidade, contribuições, direito a benefícios
ou situação cadastral.
