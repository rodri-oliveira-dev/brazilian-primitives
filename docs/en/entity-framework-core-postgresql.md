# Entity Framework Core + PostgreSQL

The package `Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql` keeps PostgreSQL/Npgsql persistence outside the core domain package. Configuration is explicit: referencing the package alone does not modify the EF Core model.

## Model-wide opt-in

Register scalar Brazilian primitives from `ConfigureConventions`:

```csharp
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    configurationBuilder.UseBrazilianPrimitiveTypesPostgreSql();
}
```

This applies the provider-specific converters, maximum lengths and PostgreSQL `character varying(n)` store types for the supported scalar primitives. Required `T` and nullable `T?` CLR properties keep their normal EF Core nullability semantics.

`Rg` and `InscricaoEstadual` are deliberately excluded because their state context cannot be inferred safely.

If every RG/IE property in a model is intentionally context-free, opt in separately:

```csharp
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    configurationBuilder
        .UseBrazilianPrimitiveTypesPostgreSql()
        .UseBrazilianContextFreeStateRegistrationsPostgreSql();
}
```

This second registration still uses one column per identifier and never creates or requires a UF column.

## Explicit property mappings

For property-level control, use the PostgreSQL fluent extensions:

```csharp
entity.Property(x => x.Cpf)
    .HasBrazilianCpfPostgreSql();

entity.Property(x => x.Email)
    .HasBrazilianEmailPostgreSql()
    .HasColumnName("contact_email");
```

Normal EF Core/Npgsql configuration can be chained afterwards. Consumers may override column names, nullability, column types and other normal facets. The library does not create indexes, unique constraints, keys or aggregate-specific rules.

## RG and Inscricao Estadual

Both primitives have two persistence modes because the state context is optional in the domain model.

### Context-free mode

Use the explicit single-column mappings when the identifier does not carry a UF:

```csharp
entity.Property(x => x.Rg)
    .HasBrazilianRgContextFreePostgreSql();

entity.Property(x => x.InscricaoEstadual)
    .HasBrazilianInscricaoEstadualContextFreePostgreSql();
```

This mode persists only the canonical `Value`. Materialization calls the context-free parser, so no state is inferred. Passing a state-aware value to either mapping fails instead of silently dropping its UF.

### State-aware mode

When the identifier carries an explicit state, map it as a complex property so both `Value` and `State` are persisted:

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

The state is stored as a stable two-letter UF code such as `SP`, `MG`, or `RO`. `BrazilianState.Unknown` is not valid in a state-aware mapping.

### Nullability

Property nullability is independent from state context:

- `Rg? == null` or `InscricaoEstadual? == null`: the identifier is absent and PostgreSQL stores SQL `NULL`;
- non-null context-free value: the identifier exists but has no UF;
- non-null state-aware value: both identifier and UF are known.

These three states remain distinct. No UF is inferred from identifier text.

## Validation behavior

Persisted non-null values are rehydrated through invariant-preserving domain parsing. Invalid database text or invalid UF codes therefore fail during materialization rather than producing a default or invalid value object.
