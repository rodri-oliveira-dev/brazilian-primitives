# TelefoneBrasileiro

`TelefoneBrasileiro` é um wrapper para campos que aceitam telefone fixo ou celular brasileiro.

## Contrato

| Aspecto | Comportamento |
| --- | --- |
| Valor canônico | valor nacional delegado |
| Entrada aceita | qualquer valor aceito por exatamente um entre `LandlinePhone` e `MobilePhone` |
| Normalização | delegada |
| Formatação | `G`, `F` e `E` delegados |
| Discriminador | `TipoTelefoneBrasileiro.Fixo` ou `TipoTelefoneBrasileiro.Celular` |

```csharp
TelefoneBrasileiro telefone = TelefoneBrasileiro.Parse("+55 11 98765-4321");

Console.WriteLine(telefone.Tipo); // Celular
Console.WriteLine(telefone.E164); // +5511987654321
```

Use `LandlinePhone` ou `MobilePhone` diretamente quando o domínio aceitar apenas um deles.
