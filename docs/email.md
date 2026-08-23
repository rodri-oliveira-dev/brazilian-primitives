# Email

`Email` represents an email address using a strict, modern syntax subset. It accepts an ASCII `dot-atom`
local part and a DNS/IDN domain, normalizing the domain to lowercase ASCII/Punycode through
`System.Globalization.IdnMapping`.

The local part is preserved exactly as supplied, including case. The domain is case-insensitive and is the only
part normalized:

```csharp
Email email = Email.Parse("User@Domínio.COM");

Console.WriteLine(email.Value);     // User@xn--domnio-2wa.com
Console.WriteLine(email.LocalPart); // User
Console.WriteLine(email.Domain);    // xn--domnio-2wa.com
```

This type deliberately does not implement the whole historical RFC 5322 grammar. Quoted local parts, comments,
display names, address lists, domain literals, and SMTPUTF8 local parts are outside this first contract.

`Email.IsValid` is syntactic validation only. It does not query DNS, does not check MX records, does not prove that
the mailbox exists or receives messages, and does not prove ownership.

References consulted on 2026-08-23:

- RFC 5321, SMTP mailbox limits.
- RFC 5322, `addr-spec` and `dot-atom`.
- RFC 6531, SMTPUTF8 scope intentionally not supported for the local part.
- .NET `System.Globalization.IdnMapping` documentation for IDN normalization.
