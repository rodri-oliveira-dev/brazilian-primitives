# Brazilian.PrimitivesTypes

[![Build & Tests](https://github.com/rodri-oliveira-dev/brazilian-primitives/actions/workflows/ci.yml/badge.svg)](https://github.com/rodri-oliveira-dev/brazilian-primitives/actions/workflows/ci.yml)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=rodri-oliveira-dev_brazilian-primitives&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=rodri-oliveira-dev_brazilian-primitives)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)](https://dotnet.microsoft.com/)
[![Coverage](https://img.shields.io/badge/coverage-%E2%89%A570%25-brightgreen)](https://github.com/rodri-oliveira-dev/brazilian-primitives/actions/workflows/ci.yml)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Idioma: Português do Brasil | [English](https://github.com/rodri-oliveira-dev/brazilian-primitives/blob/main/README.en.md)

Brazilian.PrimitivesTypes é uma biblioteca .NET para modelar identificadores brasileiros como value objects imutáveis.

Ela valida e normaliza valores como CPF, CNPJ, CEP, chaves Pix, telefones brasileiros, placas, RENAVAM, CNH, CNS, título
eleitoral, inscrição estadual e identificadores bancários sem fazer chamadas externas.

## Instalação

```bash
dotnet add package Brazilian.PrimitivesTypes
```

O repositório atual usa .NET 10.

## Exemplo Rápido

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

## Dapper + SQL Server

A integração com Dapper também é opcional e vive em um pacote separado:

```bash
dotnet add package Brazilian.PrimitivesTypes.Dapper.SqlServer
```

Registre os handlers uma vez no bootstrap da aplicação:

```csharp
using Brazilian.PrimitivesTypes.Dapper.SqlServer;

BrazilianPrimitivesDapperSqlServer.Register();
```

Os handlers permitem usar os primitives diretamente em parâmetros escalares de `INSERT`, `UPDATE` e `WHERE`, além de materializá-los em `SELECT`. Eles enviam o `Value` canônico como `AnsiString` com o tamanho recomendado para `varchar(n)`.

Dapper não cria schema nem migrations: a aplicação continua responsável pelas colunas SQL Server. `Rg` e `InscricaoEstadual` são **Value-only** nesta integração; a UF não é persistida nem recuperada. List expansion (`IN @Values`) de coleções de primitives não usa os handlers por item no Dapper 2.1.x e, por isso, não é declarada como cenário suportado.

Consulte o guia completo de [Dapper com SQL Server](docs/pt-BR/dapper-sql-server.md) para instalação, `SqlConnection`, registro, `INSERT`, `SELECT`, `UPDATE`, filtros parametrizados, nullable, `DynamicParameters`, tabela completa de `varchar(n)` e diferenças em relação ao EF Core.

## Entity Framework Core + SQL Server

A integração com EF Core é opcional e vive em um pacote separado:

```bash
dotnet add package Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer
```

Ela persiste os `Value` canônicos em colunas SQL Server, respeita `T?` como SQL `NULL` e permite consultas LINQ usando os tipos fortes do domínio. RG e Inscrição Estadual suportam modos explícitos context-free e state-aware; nenhuma UF é inferida automaticamente.

Consulte o guia completo de [Entity Framework Core com SQL Server](docs/pt-BR/entity-framework-core-sql-server.md) para o exemplo `Customer`, convenções globais, Fluent API, schema esperado, nullabilidade e persistência de `Rg`/`InscricaoEstadual` com e sem UF.

## O Que a Biblioteca Faz

- Representa identificadores brasileiros com tipos explícitos.
- Preserva zeros à esquerda.
- Aceita somente formatos documentados.
- Normaliza valores canônicos de forma determinística.
- Implementa dígitos verificadores locais quando o tipo realmente contém esse algoritmo.
- Separa validade estrutural ou matemática de existência real.

## Limites de Validação

`IsValid`, `TryParse` e `Parse` são operações locais. Elas não consultam Receita Federal, Correios, Banco Central, DICT,
Anatel, SENATRAN, DETRAN, TSE, CADSUS, SINTEGRA, SEFAZ, Caixa, Banco do Brasil, CNIS ou sistemas bancários.

Um valor aceito pela biblioteca pode ter estrutura ou dígitos verificadores válidos e ainda assim não existir, não estar
ativo, não pertencer a determinada pessoa ou empresa, não estar regular, não ser alcançável ou não estar vigente em uma
base oficial.

## Primitivos Suportados

| Domínio | Tipo |
| --- | --- |
| Documentos fiscais de pessoa física e jurídica | [`Cpf`](docs/pt-BR/primitives/cpf.md), [`Cnpj`](docs/pt-BR/primitives/cnpj.md), [`CpfCnpj`](docs/pt-BR/primitives/cpf-cnpj.md) |
| Endereço e contato | [`Cep`](docs/pt-BR/primitives/cep.md), [`Email`](docs/pt-BR/primitives/email.md), [`LandlinePhone`](docs/pt-BR/primitives/landline-phone.md), [`MobilePhone`](docs/pt-BR/primitives/mobile-phone.md), [`TelefoneBrasileiro`](docs/pt-BR/primitives/telefone-brasileiro.md) |
| Pix e bancos | [`ChavePix`](docs/pt-BR/primitives/chave-pix.md), [`Ispb`](docs/pt-BR/primitives/ispb.md), [`CodigoCompe`](docs/pt-BR/primitives/codigo-compe.md) |
| Documentos civis, trabalhistas, saúde e eleitorais | [`Rg`](docs/pt-BR/primitives/rg.md), [`Cnh`](docs/pt-BR/primitives/cnh.md), [`Cns`](docs/pt-BR/primitives/cns.md), [`Nit`](docs/pt-BR/primitives/nit.md), [`PisPasep`](docs/pt-BR/primitives/pis-pasep.md), [`TituloEleitoral`](docs/pt-BR/primitives/titulo-eleitoral.md) |
| Fiscal estadual e veículos | [`InscricaoEstadual`](docs/pt-BR/primitives/inscricao-estadual.md), [`PlacaVeiculo`](docs/pt-BR/primitives/placa-veiculo.md), [`Renavam`](docs/pt-BR/primitives/renavam.md) |

Consulte o [inventário de primitivos](docs/pt-BR/primitives/index.md) para formatos canônicos, formatos aceitos,
normalizações e modo de validação.

## Design

A biblioteca segue um contrato conservador de value objects:

- entrada inválida falha na criação;
- valores canônicos são armazenados como `string`, não como número;
- igualdade usa o valor normalizado e, quando necessário, contexto explícito como `BrazilianState`;
- instâncias `default` de structs não expõem valor válido;
- nenhum tipo remove texto arbitrário para tentar encontrar um identificador dentro dele.

Leia mais em [Princípios de design](docs/pt-BR/design-principles.md).

## Desenvolvimento

```bash
dotnet tool restore
dotnet restore --locked-mode
dotnet format Brazilian.PrimitivesTypes.slnx --verify-no-changes --no-restore
dotnet build Brazilian.PrimitivesTypes.slnx --configuration Release --no-restore
dotnet test Brazilian.PrimitivesTypes.slnx --configuration Release --no-build
```

Validação dos pacotes:

```bash
dotnet pack src/Brazilian.PrimitivesTypes/Brazilian.PrimitivesTypes.csproj --configuration Release --no-build --output artifacts/packages
dotnet pack src/Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer/Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.csproj --configuration Release --no-build --output artifacts/packages
dotnet pack src/Brazilian.PrimitivesTypes.Dapper.SqlServer/Brazilian.PrimitivesTypes.Dapper.SqlServer.csproj --configuration Release --no-build --output artifacts/packages
dotnet run --file scripts/verify-package.cs -- artifacts/packages --package-id Brazilian.PrimitivesTypes
dotnet run --file scripts/verify-package.cs -- artifacts/packages --package-id Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer --expected-dependency Brazilian.PrimitivesTypes
dotnet run --file scripts/verify-package.cs -- artifacts/packages --package-id Brazilian.PrimitivesTypes.Dapper.SqlServer --expected-dependency Brazilian.PrimitivesTypes
dotnet run --file scripts/verify-package.cs -- artifacts/packages --package-id Brazilian.PrimitivesTypes.Dapper.SqlServer --expected-dependency Dapper
```

## Contribuição

Veja [CONTRIBUTING.md](CONTRIBUTING.md), [CHANGELOG.md](CHANGELOG.md) e [SECURITY.md](SECURITY.md).
