# Entity Framework Core + PostgreSQL

The package `Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql` keeps PostgreSQL/Npgsql persistence outside the core domain package.

## RG and Inscricao Estadual

Both primitives have two persistence modes because the state context is optional in the domain model.

### Context-free mode

Use the single-column converters when the identifier does not carry a UF:

```csharp
entity.Property(x => x.Rg)
    .HasConversion(new RgValueConverter())
    .HasMaxLength(10)
    .HasColumnType("character varying(10)");

entity.Property(x => x.InscricaoEstadual)
    .HasConversion(new InscricaoEstadualValueConverter())
    .HasMaxLength(14)
    .HasColumnType("character varying(14)");
```

This mode persists only the canonical `Value`. Materialization calls the context-free parser, so no state is inferred. Passing a state-aware value to either converter throws instead of silently dropping its UF.

### State-aware mode

When the identifier carries an explicit state, persist both `Value` and `State`:

```csharp
entity.ComplexProperty(
    x => x.Rg,
    complex => RgStateAwarePostgreSqlMapping.Configure(
        complex,
        "rg_value",
        "rg_state"));

entity.ComplexProperty(
    x => x.InscricaoEstadual,
    complex => InscricaoEstadualStateAwarePostgreSqlMapping.Configure(
        complex,
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
