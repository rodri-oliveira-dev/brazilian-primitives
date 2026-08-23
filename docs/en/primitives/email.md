# Email

`Email` represents a strict interoperable email syntax subset. It is intentionally narrower than the full historical
RFC grammar.

## Contract

| Aspect | Behavior |
| --- | --- |
| Canonical value | local part as supplied, domain lowercase/Punycode |
| Accepted input | one `local@domain` address |
| Local part | ASCII dot-atom, max 64 characters |
| Domain | DNS/IDN domain labels, max 63 characters each |
| Address length | max 254 characters after normalization |

```csharp
Email email = Email.Parse("User@Domínio.COM");

Console.WriteLine(email.Value);     // User@xn--domnio-5va.com
Console.WriteLine(email.LocalPart); // User
Console.WriteLine(email.Domain);    // xn--domnio-5va.com
```

Quoted local parts, display names, comments, address lists, domain literals, `mailto:` values, and SMTPUTF8 local parts
are rejected.

Validation does not query DNS, MX, mailbox existence, deliverability, ownership, or provider-specific aliasing rules.
