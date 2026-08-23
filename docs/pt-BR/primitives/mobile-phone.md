# MobilePhone

`MobilePhone` representa um número celular brasileiro conforme o formato atual do plano de numeração.

## Contrato

| Aspecto | Comportamento |
| --- | --- |
| Valor canônico | DDD de 2 dígitos + assinante de 9 dígitos |
| Entrada aceita | `11987654321`, `(11) 98765-4321`, `+55 11 98765-4321`, `+5511987654321` |
| Normalização | dígitos nacionais canônicos |
| Regra do assinante | 9 dígitos começando por `9` |
| Formatação | `F`: `(11) 98765-4321`; `E`: `+5511987654321` |

```csharp
MobilePhone celular = MobilePhone.Parse("+55 11 98765-4321");

Console.WriteLine(celular.AreaCode);         // 11
Console.WriteLine(celular.SubscriberNumber); // 987654321
Console.WriteLine(celular.E164);             // +5511987654321
```

DDDs não atribuídos, celulares antigos de oito dígitos, fixos, não geográficos, códigos de serviço e máscaras soltas
são rejeitados.

A validação não comprova existência da linha, titularidade, operadora, portabilidade, alcance, SMS, WhatsApp ou
localização.
