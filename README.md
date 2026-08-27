# Brazilian Primitives for .NET

[![Build & Tests](https://github.com/rodri-oliveira-dev/brazilian-primitives/actions/workflows/ci.yml/badge.svg)](https://github.com/rodri-oliveira-dev/brazilian-primitives/actions/workflows/ci.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=rodri-oliveira-dev_brazilian-primitives&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=rodri-oliveira-dev_brazilian-primitives)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![Coverage](https://img.shields.io/badge/coverage-%E2%89%A570%25-brightgreen)](https://github.com/rodri-oliveira-dev/brazilian-primitives/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Idioma: Português do Brasil | [English](README.en.md)

Um ecossistema .NET para trabalhar com identificadores brasileiros como **value objects fortemente tipados**, com integrações opcionais para persistência.

O objetivo é manter o domínio independente de infraestrutura: use apenas os primitives quando precisar de validação e normalização, e adicione uma integração somente quando sua camada de persistência realmente precisar dela.

## Qual pacote devo instalar?

| Pacote | Use quando |
| --- | --- |
| [`Brazilian.PrimitivesTypes`](https://www.nuget.org/packages/Brazilian.PrimitivesTypes) | Você precisa de CPF, CNPJ, CEP, Pix, telefones, documentos, veículos e outros tipos brasileiros sem dependência de persistência |
| [`Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer`](https://www.nuget.org/packages/Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer) | Sua aplicação usa Entity Framework Core com SQL Server |
| [`Brazilian.PrimitivesTypes.Dapper.SqlServer`](https://www.nuget.org/packages/Brazilian.PrimitivesTypes.Dapper.SqlServer) | Sua aplicação usa Dapper com SQL Server |

### Core

```bash
dotnet add package Brazilian.PrimitivesTypes
```

### Entity Framework Core + SQL Server

```bash
dotnet add package Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer
```

### Dapper + SQL Server

```bash
dotnet add package Brazilian.PrimitivesTypes.Dapper.SqlServer
```

## Arquitetura dos pacotes

```text
Brazilian.PrimitivesTypes
├── Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer
└── Brazilian.PrimitivesTypes.Dapper.SqlServer
```

`Brazilian.PrimitivesTypes` é o pacote de domínio. Ele não depende de EF Core, Dapper ou SQL Server.

Os pacotes de integração dependem do Core e adicionam apenas o comportamento necessário para o mecanismo de persistência correspondente.

## Exemplo rápido

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

## Primitivos disponíveis

| Domínio | Tipos |
| --- | --- |
| Documentos fiscais | `Cpf`, `Cnpj`, `CpfCnpj` |
| Endereço e contato | `Cep`, `Email`, `LandlinePhone`, `MobilePhone`, `TelefoneBrasileiro` |
| Pix e bancos | `ChavePix`, `Ispb`, `CodigoCompe` |
| Documentos civis, trabalhistas, saúde e eleitorais | `Rg`, `Cnh`, `Cns`, `Nit`, `PisPasep`, `TituloEleitoral` |
| Fiscal estadual e veículos | `InscricaoEstadual`, `PlacaVeiculo`, `Renavam` |

Consulte o [inventário completo de primitivos](docs/pt-BR/primitives/index.md) para formatos aceitos, valores canônicos, normalização e regras de validação.

## Integrações

### Entity Framework Core

A integração para SQL Server oferece conventions, value converters e Fluent API para persistir os primitives sem remover os tipos fortes do modelo de domínio.

Veja o guia de [Entity Framework Core com SQL Server](docs/pt-BR/entity-framework-core-sql-server.md).

### Dapper

A integração Dapper registra `TypeHandler<T>` para enviar valores canônicos como parâmetros escalares e materializar os primitives diretamente das consultas.

Veja o guia de [Dapper com SQL Server](docs/pt-BR/dapper-sql-server.md).

## Limites de validação

As validações são locais. A biblioteca não consulta Receita Federal, Correios, Banco Central, DICT, Anatel, SENATRAN, DETRAN, TSE, CADSUS, SINTEGRA, SEFAZ ou outras bases oficiais.

Um valor aceito pode ser estrutural ou matematicamente válido e ainda assim não existir, não estar ativo ou não pertencer a uma pessoa ou empresa específica.

## Documentação

- [Documentação em Português do Brasil](docs/pt-BR)
- [English documentation](docs/en)
- [Princípios de design](docs/pt-BR/design-principles.md)
- [Guia de contribuição](CONTRIBUTING.md)
- [Changelog](CHANGELOG.md)
- [Política de segurança](SECURITY.md)

## Desenvolvimento

```bash
dotnet tool restore
dotnet restore --locked-mode
dotnet format Brazilian.PrimitivesTypes.slnx --verify-no-changes --no-restore
dotnet build Brazilian.PrimitivesTypes.slnx --configuration Release --no-restore
dotnet test Brazilian.PrimitivesTypes.slnx --configuration Release --no-build
```

Para validar os pacotes localmente:

```bash
dotnet pack src/Brazilian.PrimitivesTypes/Brazilian.PrimitivesTypes.csproj --configuration Release --output artifacts/packages
dotnet pack src/Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer/Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.csproj --configuration Release --output artifacts/packages
dotnet pack src/Brazilian.PrimitivesTypes.Dapper.SqlServer/Brazilian.PrimitivesTypes.Dapper.SqlServer.csproj --configuration Release --output artifacts/packages
```
