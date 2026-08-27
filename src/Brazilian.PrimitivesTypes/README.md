# Brazilian.PrimitivesTypes

Strongly typed, immutable .NET value objects for Brazilian identifiers and domain data.

Use this package when you want Brazilian primitives such as CPF, CNPJ, CEP, Pix keys, phone numbers, vehicle identifiers, RG, state registrations, and banking identifiers **without coupling your domain model to a persistence framework**.

## Install

```bash
dotnet add package Brazilian.PrimitivesTypes
```

The current package targets .NET 10.

## Quick start

```csharp
using Brazilian.PrimitivesTypes;

Cpf cpf = Cpf.Parse("529.982.247-25");
Cnpj cnpj = Cnpj.Parse("00.000.000/e08g-12");
Cep cep = Cep.Parse("01311-000");
ChavePix pix = ChavePix.Parse("(11) 98765-4321");

Console.WriteLine(cpf.Value);       // 52998224725
Console.WriteLine(cnpj.Value);      // 00000000E08G12
Console.WriteLine(cep.Formatted);   // 01311-000
Console.WriteLine(pix.Value);       // +5511987654321
```

## What this package provides

- Strongly typed domain values instead of loose strings.
- Deterministic parsing and normalization.
- Preservation of meaningful leading zeros.
- Local check-digit validation where the identifier defines one.
- Explicit formatting APIs.
- Immutable value-object semantics.
- No dependency on EF Core, Dapper, SQL Server, or external services.

## Supported primitives

| Domain | Types |
| --- | --- |
| Personal and company tax registrations | `Cpf`, `Cnpj`, `CpfCnpj` |
| Address and contact data | `Cep`, `Email`, `LandlinePhone`, `MobilePhone`, `TelefoneBrasileiro` |
| Pix and banking | `ChavePix`, `Ispb`, `CodigoCompe` |
| Civil, labor, health, and electoral identifiers | `Rg`, `Cnh`, `Cns`, `Nit`, `PisPasep`, `TituloEleitoral` |
| State tax and vehicle registrations | `InscricaoEstadual`, `PlacaVeiculo`, `Renavam` |

See the complete [primitive inventory](https://github.com/rodri-oliveira-dev/brazilian-primitives/blob/main/docs/en/primitives/index.md) for accepted formats, canonical values, normalization, and validation behavior.

## Validation boundaries

`IsValid`, `TryParse`, and `Parse` are local operations. They do not query Receita Federal, Correios, Banco Central, DICT, Anatel, SENATRAN, DETRAN, TSE, CADSUS, SINTEGRA, SEFAZ, or other official registries.

A value accepted by the library may be structurally or mathematically valid while still not existing, not being active, or not belonging to a specific person or organization.

## Persistence integrations

Persistence support is intentionally distributed as separate packages:

| Package | Use when |
| --- | --- |
| `Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer` | Your persistence layer uses Entity Framework Core with SQL Server |
| `Brazilian.PrimitivesTypes.Dapper.SqlServer` | Your persistence layer uses Dapper with SQL Server |

Keeping these integrations separate allows the domain package to remain persistence-agnostic.

## Documentation

- [English documentation](https://github.com/rodri-oliveira-dev/brazilian-primitives/tree/main/docs/en)
- [Documentação em Português do Brasil](https://github.com/rodri-oliveira-dev/brazilian-primitives/tree/main/docs/pt-BR)
- [Design principles](https://github.com/rodri-oliveira-dev/brazilian-primitives/blob/main/docs/en/design-principles.md)
- [Repository](https://github.com/rodri-oliveira-dev/brazilian-primitives)

## Related packages

| Package | Purpose |
| --- | --- |
| `Brazilian.PrimitivesTypes` | Core Brazilian value objects |
| `Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer` | EF Core + SQL Server integration |
| `Brazilian.PrimitivesTypes.Dapper.SqlServer` | Dapper + SQL Server integration |
