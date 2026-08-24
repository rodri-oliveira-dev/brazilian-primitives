# Brazilian.PrimitivesTypes

[![Build & Tests](https://github.com/rodri-oliveira-dev/brazilian-primitives/actions/workflows/ci.yml/badge.svg)](https://github.com/rodri-oliveira-dev/brazilian-primitives/actions/workflows/ci.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=rodri-oliveira-dev_brazilian-primitives&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=rodri-oliveira-dev_brazilian-primitives)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![Coverage](https://img.shields.io/badge/coverage-%E2%89%A570%25-brightgreen)](https://github.com/rodri-oliveira-dev/brazilian-primitives/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Brazilian.PrimitivesTypes is a small .NET library for modeling Brazilian identifiers as immutable value objects.

It validates and normalizes values such as CPF, CNPJ, CEP, Pix keys, Brazilian phone numbers, vehicle plates, RENAVAM,
CNH, CNS, voter registration numbers, state tax registrations, and banking identifiers without making external network
calls.

The primary documentation is Brazilian Portuguese: [README.md](README.md).

## Install

```bash
dotnet add package Brazilian.PrimitivesTypes
```

The current repository targets .NET 10.

## Quick Example

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

## Entity Framework Core + SQL Server

EF Core integration is optional and lives in a separate package:

```bash
dotnet add package Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer
```

It stores canonical `Value` representations in SQL Server columns, maps `T?` to SQL `NULL`, and keeps strongly typed primitives in LINQ queries. RG and State Registration support explicit context-free and state-aware persistence modes; no state is inferred automatically.

See the complete [Entity Framework Core with SQL Server](docs/en/entity-framework-core-sql-server.md) guide for the `Customer` example, model-wide conventions, Fluent API, expected schema, nullability, and `Rg`/`InscricaoEstadual` persistence with and without a state.

## What This Library Does

- Keeps Brazilian identifiers as strongly typed values instead of loose strings.
- Preserves meaningful leading zeros.
- Accepts only documented input formats.
- Normalizes canonical values deterministically.
- Implements local check-digit algorithms where the current type actually embeds one.
- Separates structural or mathematical validity from real-world existence.

For international developers, start with [Brazilian.PrimitivesTypes explained](docs/en/brazilian-primitives-types-explained.md).

## Validation Boundaries

`IsValid`, `TryParse`, and `Parse` are local operations. They do not query Receita Federal, Correios, Banco Central,
DICT, Anatel, SENATRAN, DETRAN, TSE, CADSUS, SINTEGRA, SEFAZ, Caixa, Banco do Brasil, CNIS, or any banking system.

A value accepted by the library can be structurally or mathematically valid and still not be assigned, active, owned by
a specific person or company, reachable, regular, authorized, or current in an official registry.

## Supported Primitives

| Domain | Type |
| --- | --- |
| Personal and company tax registrations | [`Cpf`](docs/en/primitives/cpf.md), [`Cnpj`](docs/en/primitives/cnpj.md), [`CpfCnpj`](docs/en/primitives/cpf-cnpj.md) |
| Addresses and contact data | [`Cep`](docs/en/primitives/cep.md), [`Email`](docs/en/primitives/email.md), [`LandlinePhone`](docs/en/primitives/landline-phone.md), [`MobilePhone`](docs/en/primitives/mobile-phone.md), [`TelefoneBrasileiro`](docs/en/primitives/telefone-brasileiro.md) |
| Pix and banking | [`ChavePix`](docs/en/primitives/chave-pix.md), [`Ispb`](docs/en/primitives/ispb.md), [`CodigoCompe`](docs/en/primitives/codigo-compe.md) |
| Civil, labor, health, and electoral identifiers | [`Rg`](docs/en/primitives/rg.md), [`Cnh`](docs/en/primitives/cnh.md), [`Cns`](docs/en/primitives/cns.md), [`Nit`](docs/en/primitives/nit.md), [`PisPasep`](docs/en/primitives/pis-pasep.md), [`TituloEleitoral`](docs/en/primitives/titulo-eleitoral.md) |
| Tax and vehicle registrations | [`InscricaoEstadual`](docs/en/primitives/inscricao-estadual.md), [`PlacaVeiculo`](docs/en/primitives/placa-veiculo.md), [`Renavam`](docs/en/primitives/renavam.md) |

See the full [primitive inventory](docs/en/primitives/index.md) for canonical formats, accepted inputs, normalization,
and validation mode.

## Design

The library follows a conservative value-object contract:

- invalid input fails at construction time;
- canonical values are stored as strings, not numbers;
- equality uses the normalized domain value and, when applicable, explicit context such as `BrazilianState`;
- default struct instances do not expose a valid value;
- no type silently strips arbitrary text to find a valid identifier inside it.

Read more in [Design principles](docs/en/design-principles.md).

## Development

```bash
dotnet tool restore
dotnet restore --locked-mode
dotnet format Brazilian.PrimitivesTypes.slnx --verify-no-changes --no-restore
dotnet build Brazilian.PrimitivesTypes.slnx --configuration Release --no-restore
dotnet test Brazilian.PrimitivesTypes.slnx --configuration Release --no-build
```

Package validation:

```bash
dotnet pack src/Brazilian.PrimitivesTypes/Brazilian.PrimitivesTypes.csproj --configuration Release --no-build --output artifacts/packages
dotnet pack src/Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer/Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.csproj --configuration Release --no-build --output artifacts/packages
dotnet run --file scripts/verify-package.cs -- artifacts/packages --package-id Brazilian.PrimitivesTypes
dotnet run --file scripts/verify-package.cs -- artifacts/packages --package-id Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer --expected-dependency Brazilian.PrimitivesTypes
```

## Contributing

See [CONTRIBUTING.md](CONTRIBUTING.md), [CHANGELOG.md](CHANGELOG.md), and [SECURITY.md](SECURITY.md).
