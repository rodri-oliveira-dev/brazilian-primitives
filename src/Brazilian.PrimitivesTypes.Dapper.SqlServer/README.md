# Brazilian.PrimitivesTypes.Dapper.SqlServer

Dapper + SQL Server integration for `Brazilian.PrimitivesTypes`.

Use this package when you want Brazilian primitive value objects to participate directly in Dapper parameters and query materialization while keeping the core domain package independent from Dapper.

## Install

```bash
dotnet add package Brazilian.PrimitivesTypes.Dapper.SqlServer
```

The package references `Brazilian.PrimitivesTypes` and `Dapper`.

To open SQL Server connections, your application also needs an ADO.NET provider such as `Microsoft.Data.SqlClient`.

## Register the handlers

Register the type handlers once during application bootstrap, before the first Dapper operation:

```csharp
using Brazilian.PrimitivesTypes.Dapper.SqlServer;

BrazilianPrimitivesDapperSqlServer.Register();
```

Registration is safe to call repeatedly, but registering once during startup is the recommended pattern.

## Use primitives directly

After registration, supported primitives can be sent directly as scalar parameters:

```csharp
using Brazilian.PrimitivesTypes;
using Dapper;

Cpf cpf = Cpf.Parse("529.982.247-25");

CustomerRow? customer = await connection.QuerySingleOrDefaultAsync<CustomerRow>(
    """
    SELECT Id, Cpf, Email, Cep
    FROM dbo.Customers
    WHERE Cpf = @Cpf;
    """,
    new { Cpf = cpf });
```

The handlers persist canonical values using ANSI string metadata and the recommended SQL Server `varchar(n)` size.

They also materialize supported primitives directly from query results.

## Schema remains your responsibility

Dapper does not provide a model builder or migrations. This package does **not** create tables, columns, indexes, or constraints.

For example:

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

## Nullable values

Use nullable primitives (`T?`) and `null` to represent SQL `NULL`. A default struct value is not a valid absence sentinel.

## RG and State Registration

For `Rg` and `InscricaoEstadual`, this integration is **value-only**: the handler persists the canonical value but does not persist Brazilian state context.

If state context must be preserved, store it explicitly in your schema or use the EF Core state-aware integration.

## List expansion

Dapper 2.1.x list expansion (`IN @Values`) does not invoke the registered handler for each element. Collections of Brazilian primitives are therefore not declared as a supported handler scenario.

When list expansion is required, deliberately project the collection to canonical values before passing it to Dapper.

## Documentation

- [Complete Dapper + SQL Server guide](https://github.com/rodri-oliveira-dev/brazilian-primitives/blob/main/docs/en/dapper-sql-server.md)
- [Guia completo em Português do Brasil](https://github.com/rodri-oliveira-dev/brazilian-primitives/blob/main/docs/pt-BR/dapper-sql-server.md)
- [Repository](https://github.com/rodri-oliveira-dev/brazilian-primitives)

## Related packages

| Package | Purpose |
| --- | --- |
| `Brazilian.PrimitivesTypes` | Core Brazilian value objects |
| `Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer` | EF Core + SQL Server integration |
| `Brazilian.PrimitivesTypes.Dapper.SqlServer` | Dapper + SQL Server integration |
