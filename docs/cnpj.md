# CNPJ

`Cnpj` is an immutable value object for Brazilian Cadastro Nacional da Pessoa Jurídica numbers.

This version validates the traditional 14-character numeric CNPJ structure and both modulo-11 verification digits locally. Validation does **not** confirm that the CNPJ exists at Receita Federal, belongs to a legal entity, is active, or has a particular cadastral status.

The canonical value is stored as text. This is intentional so the same public `Cnpj` type can evolve to support the alphanumeric CNPJ format without changing the domain type.

## Create a CNPJ

```csharp
using Brazilian.Primitives;

Cnpj cnpj = Cnpj.Parse("11.222.333/0001-81");

Console.WriteLine(cnpj.Value);     // 11222333000181
Console.WriteLine(cnpj.Formatted); // 11.222.333/0001-81
```

The canonical unmasked representation is also accepted:

```csharp
Cnpj cnpj = Cnpj.Parse("11222333000181");
```

Leading zeros are preserved:

```csharp
Cnpj cnpj = Cnpj.Parse("04.252.011/0001-10");

Console.WriteLine(cnpj.Value); // 04252011000110
```

## Validate without exceptions

```csharp
if (Cnpj.TryParse(input, out Cnpj cnpj))
{
    Console.WriteLine(cnpj.Value);
}
```

For a boolean-only validation:

```csharp
bool valid = Cnpj.IsValid("11.222.333/0001-81");
```

## Supported formats

Only the following numeric input representations are accepted in this version:

```text
11222333000181
11.222.333/0001-81
```

Parsing is intentionally strict. Arbitrary characters are not removed before validation, so values such as `11abc222333000181` are rejected rather than silently normalized.

Alphanumeric CNPJ support is intentionally outside this implementation and will be added by evolving the same `Cnpj` type.

## Formatting

```csharp
Cnpj cnpj = Cnpj.Parse("11222333000181");

cnpj.ToString();           // 11222333000181
cnpj.ToString("G", null); // 11222333000181
cnpj.ToString("F", null); // 11.222.333/0001-81
```
