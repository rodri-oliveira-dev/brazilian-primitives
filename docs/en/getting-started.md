# Getting Started

Install the package:

```bash
dotnet add package Brazilian.PrimitivesTypes
```

Import the namespace and parse values at your application boundary:

```csharp
using Brazilian.PrimitivesTypes;

if (!Cpf.TryParse(input, out Cpf cpf))
{
    return Results.BadRequest("Invalid CPF.");
}

customer.Cpf = cpf;
```

Use `Parse` when invalid data is exceptional and `TryParse` when user input is expected:

```csharp
Cpf cpf = Cpf.Parse("529.982.247-25");
bool valid = Cpf.IsValid("52998224725");
```

Most primitives expose a canonical `Value`. Some also expose `Formatted`, `E164`, `Tipo`, `State`, or domain-specific
parts such as `AreaCode` and `SubscriberNumber`.

```csharp
MobilePhone phone = MobilePhone.Parse("+55 11 98765-4321");

Console.WriteLine(phone.Value);     // 11987654321
Console.WriteLine(phone.Formatted); // (11) 98765-4321
Console.WriteLine(phone.E164);      // +5511987654321
```

Do not treat local validation as registry lookup. The library does not prove that a CPF, CNPJ, CEP, Pix key, phone
number, vehicle, voter record, bank participant, or taxpayer registration exists in an official system.

For the complete behavior matrix, see [Primitives](primitives/index.md).
