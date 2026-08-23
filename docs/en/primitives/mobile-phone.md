# MobilePhone

`MobilePhone` represents a Brazilian mobile number under Anatel's current numbering-plan shape.

## Contract

| Aspect | Behavior |
| --- | --- |
| Canonical value | 2-digit DDD + 9-digit subscriber |
| Accepted input | `11987654321`, `(11) 98765-4321`, `+55 11 98765-4321`, `+5511987654321` |
| Normalization | canonical national digits |
| Subscriber rule | nine digits starting with `9` |
| Formatting | `F`: `(11) 98765-4321`; `E`: `+5511987654321` |

```csharp
MobilePhone phone = MobilePhone.Parse("+55 11 98765-4321");

Console.WriteLine(phone.AreaCode);         // 11
Console.WriteLine(phone.SubscriberNumber); // 987654321
Console.WriteLine(phone.E164);             // +5511987654321
```

The DDD must be in the embedded Brazilian geographic area-code list. Legacy eight-digit mobile numbers, landlines,
non-geographic numbers, carrier-selection codes, emergency/service codes, and loose masks are rejected.

Validation does not prove line existence, reachability, ownership, carrier, portability, SMS, WhatsApp, or location.
