# LandlinePhone

`LandlinePhone` represents a Brazilian geographic fixed-line phone number under Anatel's numbering-plan shape.

## Contract

| Aspect | Behavior |
| --- | --- |
| Canonical value | 2-digit DDD + 8-digit subscriber |
| Accepted input | `1132345678`, `(11) 3234-5678`, `+55 11 3234-5678`, `+551132345678` |
| Normalization | canonical national digits |
| Subscriber rule | eight digits starting with `2`, `3`, `4`, or `5` |
| Formatting | `F`: `(11) 3234-5678`; `E`: `+551132345678` |

```csharp
LandlinePhone phone = LandlinePhone.Parse("(11) 3234-5678");

Console.WriteLine(phone.Value);            // 1132345678
Console.WriteLine(phone.SubscriberNumber); // 32345678
```

The `57` rural fixed-line prefix remains accepted because the implementation checks the first subscriber digit, not a
narrower second-digit rule. Mobile numbers, non-geographic codes, service numbers, country codes other than `+55`, and
loose punctuation are rejected.

Validation does not prove line existence, activation, ownership, carrier, portability, or current location.
