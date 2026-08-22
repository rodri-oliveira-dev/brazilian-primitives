# CPF

`Cpf` is an immutable value object for Brazilian Cadastro de Pessoas Físicas numbers.

It validates the CPF structure and both modulo-11 verification digits locally. Validation does **not** confirm that the CPF exists at Receita Federal, belongs to a person, or has a particular cadastral status.

## Create a CPF

```csharp
using Brazilian.Primitives;

Cpf cpf = Cpf.Parse("529.982.247-25");

Console.WriteLine(cpf.Value);     // 52998224725
Console.WriteLine(cpf.Formatted); // 529.982.247-25
```

The canonical unmasked representation is also accepted:

```csharp
Cpf cpf = Cpf.Parse("52998224725");
```

## Validate without exceptions

```csharp
if (Cpf.TryParse(input, out Cpf cpf))
{
    Console.WriteLine(cpf.Value);
}
```

For a boolean-only validation:

```csharp
bool valid = Cpf.IsValid("529.982.247-25");
```

## Supported formats

Only the following input representations are accepted:

```text
52998224725
529.982.247-25
```

Parsing is intentionally strict. Arbitrary characters are not removed before validation, so values such as `529abc98224725` are rejected rather than silently normalized.

## Formatting

```csharp
Cpf cpf = Cpf.Parse("52998224725");

cpf.ToString();          // 52998224725
cpf.ToString("G", null); // 52998224725
cpf.ToString("F", null); // 529.982.247-25
```
