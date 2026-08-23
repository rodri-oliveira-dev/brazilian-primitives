# TelefoneBrasileiro

`TelefoneBrasileiro` is a type-safe wrapper for fields that accept either `LandlinePhone` or `MobilePhone`.

It does not duplicate DDD, subscriber, E.164, or formatting rules. Parsing delegates to the two specific primitives and
accepts a value only when exactly one of them accepts it.

```csharp
TelefoneBrasileiro telefone = TelefoneBrasileiro.Parse("+55 11 98765-4321");

Console.WriteLine(telefone.Tipo);             // Celular
Console.WriteLine(telefone.Value);            // 11987654321
Console.WriteLine(telefone.Formatted);        // (11) 98765-4321
Console.WriteLine(telefone.ToString("E", null)); // +5511987654321
```

Use `LandlinePhone` or `MobilePhone` directly when the domain accepts only one kind. Use `TelefoneBrasileiro` when the
domain contract explicitly accepts either fixed-line or mobile Brazilian numbers.

Validation follows the numbering plan only. It does not prove line existence, ownership, carrier, portability,
reachability, SMS/WhatsApp support, or location.
