# TelefoneBrasileiro

`TelefoneBrasileiro` is a wrapper for fields that accept either a Brazilian landline or mobile number.

## Contract

| Aspect | Behavior |
| --- | --- |
| Canonical value | delegated national value |
| Accepted input | any value accepted by exactly one of `LandlinePhone` or `MobilePhone` |
| Normalization | delegated |
| Formatting | `G`, `F`, and `E` delegated |
| Type discriminator | `TipoTelefoneBrasileiro.Fixo` or `TipoTelefoneBrasileiro.Celular` |

```csharp
TelefoneBrasileiro telefone = TelefoneBrasileiro.Parse("+55 11 98765-4321");

Console.WriteLine(telefone.Tipo); // Celular
Console.WriteLine(telefone.E164); // +5511987654321
```

Use `LandlinePhone` or `MobilePhone` directly when the domain accepts only one kind.
