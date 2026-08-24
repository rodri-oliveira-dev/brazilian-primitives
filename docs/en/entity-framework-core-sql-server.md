# Entity Framework Core with SQL Server

`Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer` is the optional integration package for applications that use Entity Framework Core with SQL Server. The domain package `Brazilian.PrimitivesTypes` remains independent from EF Core and database providers.

## Installation

In the project that contains the domain model:

```bash
dotnet add package Brazilian.PrimitivesTypes
```

In the persistence project that uses EF Core SQL Server:

```bash
dotnet add package Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer
```

The SQL Server package references `Brazilian.PrimitivesTypes`, while keeping the packages separate preserves the dependency boundary between domain and persistence concerns.

## Complete `Customer` example

The domain model stays strongly typed:

```csharp
using Brazilian.PrimitivesTypes;

public sealed class Customer
{
    public long Id { get; set; }

    public Cpf Cpf { get; set; }

    public Email? Email { get; set; }

    public Cep Cep { get; set; }
}
```

Configure the SQL Server provider normally:

```csharp
services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
```

### Model-wide conventions

When the model contains several scalar primitives, register the opt-in pre-conventions explicitly:

```csharp
using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void ConfigureConventions(
        ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.UseBrazilianPrimitiveTypesSqlServer();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(customer => customer.Id);
        });
    }
}
```

Referencing the package by itself does not modify the EF model. `UseBrazilianPrimitiveTypesSqlServer()` is explicitly opt-in.

### Explicit property mappings

If local configuration is preferred, use the Fluent API extensions:

```csharp
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Customer>(entity =>
    {
        entity.HasKey(customer => customer.Id);

        entity.Property(customer => customer.Cpf)
            .HasBrazilianCpfSqlServer();

        entity.Property(customer => customer.Email)
            .HasBrazilianEmailSqlServer();

        entity.Property(customer => customer.Cep)
            .HasBrazilianCepSqlServer();
    });
}
```

These are regular EF Core builders. Facets configured after the extensions, such as column names, SQL types, or required/optional metadata, can override the defaults when compatible with the CLR type.

## What is stored

The database receives the canonical `Value`, never the display mask or original user input.

A conceptual `Customer` schema is:

```sql
CREATE TABLE Customers
(
    Id BIGINT NOT NULL,
    Cpf VARCHAR(11) NOT NULL,
    Email VARCHAR(254) NULL,
    Cep VARCHAR(8) NOT NULL,
    CONSTRAINT PK_Customers PRIMARY KEY (Id)
);
```

For these values:

```csharp
Cpf cpf = Cpf.Parse("529.982.247-25");
Email email = Email.Parse("USER@Example.COM");
Cep cep = Cep.Parse("01311-000");
```

SQL Server receives the corresponding canonical representations, for example `52998224725` for CPF and `01311000` for CEP. Primitive-specific canonical normalization is preserved as well.

`Formatted` is presentation-only. It should not be persisted by default.

## Nullability

Value-object validity and value absence are separate concepts:

- `Email` means a valid e-mail value exists according to the local rules of the type;
- `Email?` allows absence;
- `Email? == null` is stored as SQL `NULL`;
- SQL `NULL` materializes back to `null` without calling `Email.Parse`;
- invalid non-null text in the database does not become `null` or an invalid value object: materialization fails predictably.

Do not use an empty string, `default(Email)`, or sentinel values to represent absence.

## Strongly typed LINQ queries

The converter participates in query translation, so application code keeps using the domain type:

```csharp
Cpf cpf = Cpf.Parse("529.982.247-25");

Customer? customer = await dbContext.Customers
    .SingleOrDefaultAsync(x => x.Cpf == cpf);
```

There is no need to convert `cpf` to `string` manually in the LINQ expression.

## RG and State Registration without a state

`Rg` and `InscricaoEstadual` have two explicit persistence modes. Without a state, validation is conservative and structural, and only `Value` is stored.

Property-level mapping:

```csharp
entity.Property(x => x.Rg)
    .HasBrazilianRgContextFreeSqlServer();

entity.Property(x => x.InscricaoEstadual)
    .HasBrazilianInscricaoEstadualContextFreeSqlServer();
```

If every RG/IE property in a model is intentionally context-free, pre-conventions can also be registered explicitly:

```csharp
protected override void ConfigureConventions(
    ModelConfigurationBuilder configurationBuilder)
{
    configurationBuilder
        .UseBrazilianPrimitiveTypesSqlServer()
        .UseBrazilianContextFreeStateRegistrationsSqlServer();
}
```

In this mode:

```csharp
Rg rg = Rg.Parse("00000005X");

Console.WriteLine(rg.HasState); // false
Console.WriteLine(rg.State);    // BrazilianState.Unknown
```

No state is inferred from the identifier text. The schema needs only the value column, such as `Rg VARCHAR(10)` or `InscricaoEstadual VARCHAR(14)`.

A nullable `Rg?` set to `null` means the whole identifier is absent. This differs from a non-null context-free `Rg`, which has a `Value`, `HasState == false`, and `State == BrazilianState.Unknown`. The same distinction applies to `InscricaoEstadual?`.

## RG and State Registration with a state

When the state is known, use state-aware persistence so that context is not lost. Construction still applies the strongest state-specific validation available:

```csharp
Rg rg = Rg.Parse("120300011", BrazilianState.SaoPaulo);
InscricaoEstadual ie = InscricaoEstadual.Parse(
    "110042490114",
    BrazilianState.SaoPaulo);
```

Map each value object as a complex property:

```csharp
entity.ComplexProperty(x => x.Rg)
    .HasBrazilianRgStateAwareSqlServer("RgValue", "RgState");

entity.ComplexProperty(x => x.InscricaoEstadual)
    .HasBrazilianInscricaoEstadualStateAwareSqlServer(
        "InscricaoValue",
        "InscricaoState");
```

The schema now preserves both pieces of information:

```sql
RgValue VARCHAR(10) NOT NULL,
RgState VARCHAR(2) NOT NULL,
InscricaoValue VARCHAR(14) NOT NULL,
InscricaoState VARCHAR(2) NOT NULL
```

States are stored using stable two-letter codes such as `SP`, `RJ`, and `MG`. A state-aware value cannot be written through a context-free converter: the operation fails instead of silently discarding the known state.

Nullable state-aware properties such as `Rg?` can represent absence of the entire identifier. When non-null, `Value + State` are preserved together.

## Migrations and constraints

The mappings provide conversion, length, and non-Unicode SQL Server metadata. Nullability follows the CLR type and can be refined by the EF model where allowed.

The integration does **not** automatically create:

- indexes;
- unique constraints;
- alternate keys;
- primary keys;
- aggregate-specific rules.

Those remain application-model decisions:

```csharp
entity.HasIndex(x => x.Cpf).IsUnique();
```

Only add them when they match the application's business rules.

## Validation boundaries

Persisting or materializing a primitive runs only the local rules implemented by that value object. The EF Core integration does not query Receita Federal, Correios, DICT, SEFAZ, DETRAN, or any other external registry.

A structurally or mathematically valid identifier can still be unassigned, inactive, or absent from an official registry. Persistence does not change that contract.
