# LandlinePhone

`LandlinePhone` representa um telefone fixo geográfico brasileiro.

## Contrato

| Aspecto | Comportamento |
| --- | --- |
| Valor canônico | DDD de 2 dígitos + assinante de 8 dígitos |
| Entrada aceita | `1132345678`, `(11) 3234-5678`, `+55 11 3234-5678`, `+551132345678` |
| Normalização | dígitos nacionais canônicos |
| Regra do assinante | 8 dígitos começando por `2`, `3`, `4` ou `5` |
| Formatação | `F`: `(11) 3234-5678`; `E`: `+551132345678` |

```csharp
LandlinePhone telefone = LandlinePhone.Parse("(11) 3234-5678");

Console.WriteLine(telefone.Value);            // 1132345678
Console.WriteLine(telefone.SubscriberNumber); // 32345678
```

A faixa rural iniciada por `57` permanece aceita porque a implementação valida o primeiro dígito do assinante. Celular,
número não geográfico, código de serviço, país diferente de `+55` e pontuação solta são rejeitados.

A validação não comprova existência, ativação, titularidade, operadora, portabilidade ou localização.
