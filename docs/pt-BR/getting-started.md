# Primeiros Passos

Instale o pacote:

```bash
dotnet add package Brazilian.Primitives
```

Importe o namespace e faça o parsing nas bordas da aplicação:

```csharp
using Brazilian.Primitives;

if (!Cpf.TryParse(input, out Cpf cpf))
{
    return Results.BadRequest("CPF inválido.");
}

cliente.Cpf = cpf;
```

Use `Parse` quando entrada inválida for excepcional e `TryParse` quando a entrada vier de usuário:

```csharp
Cpf cpf = Cpf.Parse("529.982.247-25");
bool valido = Cpf.IsValid("52998224725");
```

A maioria dos primitivos expõe `Value`. Alguns também expõem `Formatted`, `E164`, `Tipo`, `State` ou partes do domínio,
como `AreaCode` e `SubscriberNumber`.

```csharp
MobilePhone celular = MobilePhone.Parse("+55 11 98765-4321");

Console.WriteLine(celular.Value);     // 11987654321
Console.WriteLine(celular.Formatted); // (11) 98765-4321
Console.WriteLine(celular.E164);      // +5511987654321
```

Não trate validação local como consulta cadastral. A biblioteca não comprova existência, titularidade, situação
cadastral, regularidade, alcance telefônico ou autorização operacional em bases oficiais.

Para a matriz completa de comportamento, veja [Primitivos](primitives/index.md).
