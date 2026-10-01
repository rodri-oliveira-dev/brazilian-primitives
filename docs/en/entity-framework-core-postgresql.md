# Entity Framework Core + PostgreSQL

`Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql` is the optional Entity Framework Core integration for PostgreSQL/Npgsql. The domain package remains persistence-agnostic; add this package only to the persistence/infrastructure project that owns the EF Core model.

## Installation

```bash
dotnet add package Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql
```

The integration package depends on `Brazilian.PrimitivesTypes`, EF Core relational APIs, and `Npgsql.EntityFrameworkCore.PostgreSQL`. Referencing it alone does **not** mutate the EF Core model: configuration is explicit and opt-in.

## Customer example

Domain entities keep the strongly typed primitives:

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

Configure Npgsql normally:

```csharp
services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
```

### Model-wide conventions

For applications that want the standard PostgreSQL mapping for all scalar primitives, register the integration from `ConfigureConventions`:

```csharp
using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;
using Microsoft.EntityFrameworkCore;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void ConfigureConventions(
        ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.UseBrazilianPrimitiveTypesPostgreSql();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Cpf).HasColumnName("cpf");
            entity.Property(x => x.Email).HasColumnName("email");
            entity.Property(x => x.Cep).HasColumnName("cep");
        });
    }
}
```

The conventions configure canonical conversion, intrinsic maximum length, and provider-appropriate `character varying(n)` store types. CLR nullability is preserved: `Cpf` and `Cep` are required, while `Email?` is nullable.

### Explicit property mapping

When mapping should be visible property by property:

```csharp
entity.Property(x => x.Cpf)
    .HasBrazilianCpfPostgreSql();

entity.Property(x => x.Email)
    .HasBrazilianEmailPostgreSql();

entity.Property(x => x.Cep)
    .HasBrazilianCepPostgreSql();
```

Normal EF Core/Npgsql configuration can be chained after the library call:

```csharp
entity.Property(x => x.Email)
    .HasBrazilianEmailPostgreSql()
    .HasColumnName("contact_email")
    .HasColumnType("text");
```

The package does not automatically create indexes, unique constraints, primary keys, foreign keys, or aggregate-specific rules.

## Canonical PostgreSQL representation

Persistence uses each primitive's canonical `Value`, never `Formatted` or the original input.

For example:

| Domain value | PostgreSQL value |
| --- | --- |
| `Cpf.Parse("529.982.247-25")` | `52998224725` |
| `Cep.Parse("01311-000")` | `01311000` |
| `Email.Parse("User@Domínio.com")` | `User@xn--domnio-5va.com` |

Leading zeros and canonical alphanumeric casing are preserved.

`Formatted` is a presentation concern and should not be the default persisted representation.

## Insert, read, query, and update

```csharp
Cpf cpf = Cpf.Parse("529.982.247-25");

Customer customer = new()
{
    Id = 1,
    Cpf = cpf,
    Email = Email.Parse("User@Domínio.com"),
    Cep = Cep.Parse("01311-000"),
};

db.Customers.Add(customer);
await db.SaveChangesAsync();

Customer loaded = await db.Customers
    .SingleAsync(x => x.Cpf == cpf);

loaded.Email = Email.Parse("updated@domínio.com");
loaded.Cep = Cep.Parse("01001-000");

await db.SaveChangesAsync();
```

LINQ equality compares the strongly typed property while EF Core sends the canonical provider value to PostgreSQL.

## Nullable primitives and invalid data

`Email? == null` represents absence and maps to SQL `NULL`. Materialization returns `null`; it does not attempt to parse an absent value and it does not create a default value object.

That is different from an invalid **non-null** database value. If a column contains invalid text such as `not-an-email`, materialization fails through the primitive's public invariant-preserving parser. The provider does not manufacture an invalid/default object.

## RG and Inscricao Estadual

Both types have optional `BrazilianState` context. Property nullability and state context are independent concepts.

### Context-free: one column, no UF

When the identifier exists but its UF is not known:

```csharp
entity.Property(x => x.Rg)
    .HasBrazilianRgContextFreePostgreSql();

entity.Property(x => x.InscricaoEstadual)
    .HasBrazilianInscricaoEstadualContextFreePostgreSql();
```

This persists only canonical `Value`:

```text
RG                 -> character varying(10)
InscricaoEstadual  -> character varying(14)
```

Materialization calls `Rg.Parse(value)` / `InscricaoEstadual.Parse(value)`. No UF is inferred from the identifier.

A state-aware instance sent through a context-free converter is rejected instead of silently losing its UF.

If every RG/IE in a model is intentionally context-free, the choice can be made model-wide:

```csharp
protected override void ConfigureConventions(
    ModelConfigurationBuilder configurationBuilder)
{
    configurationBuilder
        .UseBrazilianPrimitiveTypesPostgreSql()
        .UseBrazilianContextFreeStateRegistrationsPostgreSql();
}
```

### State-aware: Value + UF

When the UF is explicitly known, map the primitive as an EF Core complex property:

```csharp
entity.ComplexProperty(
    x => x.Rg,
    complex => complex.HasBrazilianRgStateAwarePostgreSql(
        "rg_value",
        "rg_state"));

entity.ComplexProperty(
    x => x.InscricaoEstadual,
    complex => complex.HasBrazilianInscricaoEstadualStateAwarePostgreSql(
        "inscricao_value",
        "inscricao_state"));
```

PostgreSQL stores both parts:

```text
rg_value        character varying(10)
rg_state        character varying(2)
inscricao_value character varying(14)
inscricao_state character varying(2)
```

The UF uses stable two-letter codes such as `SP`, `MG`, and `RO`. Materialization retains the state and therefore the original domain equality semantics.

Where the core primitive implements a state-specific validation rule, supplying the state enables that stronger local validation. The persistence provider itself never performs external DETRAN, SEFAZ, SINTEGRA, or other cadastral lookups.

### Three distinct states

These cases are intentionally different:

1. `Rg? == null`: the entire identifier is absent and maps to SQL `NULL`.
2. Non-null `Rg` with `HasState == false` / `State == BrazilianState.Unknown`: identifier exists without UF.
3. Non-null `Rg` with `HasState == true`: identifier and explicit UF are both known.

The same distinction applies to `InscricaoEstadual`.

## Expected PostgreSQL schema

With the `Customer` example and standard mappings, migrations should produce semantics equivalent to:

```sql
CREATE TABLE customers (
    id bigint NOT NULL,
    cpf character varying(11) NOT NULL,
    email character varying(254) NULL,
    cep character varying(8) NOT NULL,
    CONSTRAINT pk_customers PRIMARY KEY (id)
);
```

For context-free RG/IE there is only the value column. State-aware mappings add a separate `character varying(2)` UF column. The integration does not add indexes or uniqueness constraints for CPF, CNPJ, e-mail, RG, or other primitives.

## PostgreSQL versus SQL Server

The PostgreSQL integration does not copy SQL Server's ANSI/Unicode facet model. PostgreSQL textual types use the database encoding; the provider uses `character varying(n)` for intrinsic length constraints instead of SQL Server `varchar(n)` plus `IsUnicode(false)`.

Both provider packages preserve the same domain invariants, but their relational metadata is provider-specific. SQL Server and PostgreSQL integrations are separate packages and do not reference each other.

## Validation boundary

All parsing and validation performed by these value objects is local and deterministic. Persistence does not prove that an identifier exists, is active, belongs to a person/company, or is registered in an official database.
