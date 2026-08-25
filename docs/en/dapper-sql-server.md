# Dapper with SQL Server

`Brazilian.PrimitivesTypes.Dapper.SqlServer` is the optional integration package for applications that use Dapper with SQL Server. It registers `TypeHandler<T>` implementations for the Brazilian Primitive Types so value objects can be sent to and materialized from Dapper commands directly.

The domain package `Brazilian.PrimitivesTypes` remains independent of Dapper, SQL Server, and any ADO.NET provider.

## Installation

Install the integration package:

```bash
dotnet add package Brazilian.PrimitivesTypes.Dapper.SqlServer
```

The package brings `Dapper` as a dependency. To open SQL Server connections in the examples below, also install an ADO.NET provider, normally `Microsoft.Data.SqlClient`:

```bash
dotnet add package Microsoft.Data.SqlClient
```

Package requirements:

- .NET 10;
- Dapper 2.1.x;
- SQL Server;
- an ADO.NET SQL Server provider in the consumer project when one is not already present.

## Register the handlers

Registration is explicit. Perform it during application bootstrap before the first Dapper operation:

```csharp
using Brazilian.PrimitivesTypes.Dapper.SqlServer;

BrazilianPrimitivesDapperSqlServer.Register();
```

Registration uses Dapper's global type-handler registry. Repeated calls to `Register()` are safe, but the recommended pattern is to register once during application startup.

## Schema remains the application's responsibility

Dapper has no model builder or migration system. This package configures provider parameters and converts values returned by the database, but it **does not create tables, columns, indexes, or migrations**.

Your schema should use `varchar(n)` columns that match each primitive contract. Example:

```sql
CREATE TABLE dbo.Customers
(
    Id BIGINT NOT NULL,
    Cpf VARCHAR(11) NOT NULL,
    Email VARCHAR(254) NULL,
    Cep VARCHAR(8) NOT NULL,
    CONSTRAINT PK_Customers PRIMARY KEY (Id)
);
```

Handlers send values as `DbType.AnsiString`, set `Size` to the recommended maximum length, and use the primitive's canonical `Value`.

## Complete example

### Read model

```csharp
using Brazilian.PrimitivesTypes;

public sealed class CustomerRow
{
    public long Id { get; set; }

    public Cpf Cpf { get; set; }

    public Email? Email { get; set; }

    public Cep Cep { get; set; }
}
```

### Open the connection

```csharp
using Microsoft.Data.SqlClient;

await using SqlConnection connection = new(connectionString);
await connection.OpenAsync();
```

### INSERT with primitives

After `Register()`, value objects can be passed directly as parameters:

```csharp
using Brazilian.PrimitivesTypes;
using Dapper;

Cpf cpf = Cpf.Parse("529.982.247-25");
Email email = Email.Parse("USER@Example.COM");
Cep cep = Cep.Parse("01311-000");

await connection.ExecuteAsync(
    """
    INSERT INTO dbo.Customers (Id, Cpf, Email, Cep)
    VALUES (@Id, @Cpf, @Email, @Cep);
    """,
    new
    {
        Id = 1L,
        Cpf = cpf,
        Email = email,
        Cep = cep,
    });
```

SQL Server receives canonical values. For example, the CPF above is sent as `52998224725` and the CEP as `01311000`.

### SELECT directly into strongly typed primitives

```csharp
CustomerRow customer = await connection.QuerySingleAsync<CustomerRow>(
    """
    SELECT Id, Cpf, Email, Cep
    FROM dbo.Customers
    WHERE Id = @Id;
    """,
    new { Id = 1L });

Cpf cpf = customer.Cpf;
Email? email = customer.Email;
Cep cep = customer.Cep;
```

Dapper gives the database value to the matching type handler, which recreates the value object through the domain parser. Invalid non-null text in the database fails during materialization rather than becoming an invalid value object silently.

### Parameterized WHERE

Use the primitive itself as a parameter:

```csharp
Cpf cpf = Cpf.Parse("529.982.247-25");

CustomerRow? customer = await connection.QuerySingleOrDefaultAsync<CustomerRow>(
    """
    SELECT Id, Cpf, Email, Cep
    FROM dbo.Customers
    WHERE Cpf = @Cpf;
    """,
    new { Cpf = cpf });
```

There is no need to convert `cpf` manually to `string` for scalar parameters.

### UPDATE

```csharp
Email updatedEmail = Email.Parse("new.user@example.com");
Cep updatedCep = Cep.Parse("04567-890");

await connection.ExecuteAsync(
    """
    UPDATE dbo.Customers
    SET Email = @Email,
        Cep = @Cep
    WHERE Cpf = @Cpf;
    """,
    new
    {
        Email = updatedEmail,
        Cep = updatedCep,
        Cpf = cpf,
    });
```

### Nullable values and SQL NULL

Absence should be represented by `T?`/`null`, not by an empty string or `default(T)`:

```csharp
Email? email = null;

await connection.ExecuteAsync(
    """
    UPDATE dbo.Customers
    SET Email = @Email
    WHERE Cpf = @Cpf;
    """,
    new
    {
        Email = email,
        Cpf = cpf,
    });
```

`null` is sent as SQL `NULL`. When a SQL `NULL` column is materialized into a nullable primitive property, the result is `null` again.

A `default` primitive instance does not represent valid absence and should not be used as a sentinel.

## DynamicParameters

`DynamicParameters` also uses the registered handlers:

```csharp
using Dapper;

DynamicParameters parameters = new();
parameters.Add("Cpf", cpf);
parameters.Add("Email", updatedEmail);

await connection.ExecuteAsync(
    """
    UPDATE dbo.Customers
    SET Email = @Email
    WHERE Cpf = @Cpf;
    """,
    parameters);
```

You do not need to specify `DbType.AnsiString` or the size manually when the value is a supported primitive: the matching handler provides that metadata.

## List expansion / IN

Dapper's list expansion does not pass each element through `ITypeHandler.SetValue` in Dapper 2.1.x. Therefore this package **does not claim type-handler support for `IN @Values` when `Values` is a collection of Brazilian Primitive Types**.

Do not rely on this pattern:

```csharp
// Not supported as a per-item type-handler scenario.
var parameters = new { Cpfs = new[] { cpf1, cpf2 } };
```

If an application needs Dapper list expansion, deliberately convert the collection to canonical provider values first and treat that as an explicit handler bypass owned by the consumer:

```csharp
var parameters = new
{
    Cpfs = cpfs.Select(item => item.Value).ToArray(),
};
```

Scalar parameters continue to use the handlers normally.

## Recommended SQL types

| Primitive | Recommended SQL Server type |
| --- | --- |
| `Cpf` | `varchar(11)` |
| `Cnpj` | `varchar(14)` |
| `CpfCnpj` | `varchar(14)` |
| `Cep` | `varchar(8)` |
| `Email` | `varchar(254)` |
| `MobilePhone` | `varchar(11)` |
| `LandlinePhone` | `varchar(10)` |
| `TelefoneBrasileiro` | `varchar(11)` |
| `ChavePix` | `varchar(77)` |
| `Cnh` | `varchar(11)` |
| `Cns` | `varchar(15)` |
| `TituloEleitoral` | `varchar(12)` |
| `Nit` | `varchar(11)` |
| `PisPasep` | `varchar(11)` |
| `PlacaVeiculo` | `varchar(7)` |
| `Renavam` | `varchar(11)` |
| `Ispb` | `varchar(8)` |
| `CodigoCompe` | `varchar(3)` |
| `Rg` | `varchar(10)` |
| `InscricaoEstadual` | `varchar(14)` |

These sizes are part of the handler contract. The package does not resize existing columns or create schema constraints automatically.

## RG and State Registration are Value-only

In the Dapper integration, `Rg` and `InscricaoEstadual` persist **only `Value`**. State context is not written by the handler and is not restored during materialization.

Even when the original object was created with a state:

```csharp
Rg rg = Rg.Parse("123456789", BrazilianState.Amazonas);
```

the Dapper parameter contains only `rg.Value`. After reading it back, the recreated `Rg` is context-free (`HasState == false`). The same rule applies to `InscricaoEstadual`.

If state is part of your persistence contract, maintain an additional state column explicitly or use the state-aware EF Core integration, which provides dedicated value + state mappings. The Dapper package never infers or persists state context automatically.

## Dapper vs Entity Framework Core

| Aspect | Dapper | EF Core |
| --- | --- | --- |
| Package | `Brazilian.PrimitivesTypes.Dapper.SqlServer` | `Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer` |
| Integration | Dapper global `TypeHandler<T>` | conventions, converters, and Fluent API |
| Schema/migrations | application responsibility | EF Core can model schema and generate migrations |
| Parameters | configures `AnsiString`, size, and canonical value | provider derives parameters from EF mapping |
| Materialization | handler recreates the primitive from the column | converter/mapping recreates the primitive |
| RG/State Registration with state | Value-only | supports context-free and state-aware modes |
| LINQ | not applicable; SQL is explicit | strongly typed LINQ queries |

Choose the integration that matches the application's persistence mechanism. Referencing both packages is not required.

## Packaging and dependencies

The distributed package contains only the dependencies required by the integration contract:

- `Brazilian.PrimitivesTypes`;
- `Dapper`.

It does not reference Entity Framework Core. Repository CI validates the package and checks a local consumer dependency graph so EF Core cannot be introduced transitively unnoticed.

## NuGet.org Trusted Publishing

For maintainers: the official release uses NuGet.org Trusted Publishing through OIDC in `.github/workflows/release.yml`.

Under the current NuGet.org model, a Trusted Publishing policy belongs to a user or organization and applies to packages owned by that owner; it does not need to be duplicated solely because a new package ID is added. Before the first publication of `Brazilian.PrimitivesTypes.Dapper.SqlServer`, verify operationally that:

1. the policy is active for the intended owner;
2. Repository Owner, Repository, and Workflow File match `rodri-oliveira-dev`, `brazilian-primitives`, and `release.yml`;
3. the GitHub `NUGET_USER` variable matches the user authorized by the policy;
4. the package ID is available or is already owned by the same owner.

This configuration lives in the NuGet.org portal and cannot be created or changed by repository code.
