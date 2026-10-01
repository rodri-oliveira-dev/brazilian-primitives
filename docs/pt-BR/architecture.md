# Arquitetura

[English](../en/architecture.md) | [Português (Brasil)](architecture.md)

Brazilian Primitives separa modelagem de domínio da infraestrutura de persistência.

## Limites dos pacotes

```text
                              Aplicação
                                  |
                                  v
                       Brazilian.PrimitivesTypes
                         Primitivos de domínio
                    ^             ^              ^
                    |             |              |
          +---------+      +------+-------+      +----------+
          |                |              |                 |
EF Core + SQL Server   EF Core + PostgreSQL      Dapper + SQL Server
      adapter                 adapter                    adapter
```

A direção das dependências é sempre para `Brazilian.PrimitivesTypes`.

- **Brazilian.PrimitivesTypes** contém value objects imutáveis, representação canônica, parsing, formatação, validação local, igualdade e contexto de domínio.
- **Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer** contém comportamento de persistência para EF Core/SQL Server.
- **Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql** contém comportamento de persistência para EF Core/PostgreSQL/Npgsql.
- **Brazilian.PrimitivesTypes.Dapper.SqlServer** contém comportamento de persistência para Dapper/SQL Server.

Os adapters podem depender do Core e do framework/provider correspondente. Eles não devem depender um do outro. Em especial, o adapter EF Core PostgreSQL não referencia o adapter EF Core SQL Server, e nenhum provider específico vaza para o Core.

## Regras arquiteturais

1. O Core permanece independente de frameworks de persistência.
2. EF Core, Dapper, SQL clients, Npgsql, migrations, schemas e comportamento específico de provider ficam fora do Core.
3. Adapters de persistência não redefinem a semântica do domínio.
4. Os adapters persistem o valor canônico exposto pelo primitive em vez de criar uma segunda representação de domínio.
5. Os adapters permanecem instaláveis de forma independente e não se referenciam.
6. Metadata relacional específica de provider permanece no adapter correspondente em vez de ser copiada entre bancos.
7. A validação dos primitives permanece local e determinística; a arquitetura não introduz rede ou consultas a registros externos.

## Architecture fitness functions

O repositório trata esses limites como restrições executáveis. Testes verificam que o Core não referencia Dapper, Entity Framework Core, SQL clients ou Npgsql. Testes específicos de provider também verificam que o adapter PostgreSQL não referencia integrações SQL Server ou Dapper.

A validação dos pacotes complementa esses testes ao verificar o grafo esperado de dependências NuGet no CI, incluindo as dependências do pacote PostgreSQL para Core, EF Core Relational e Npgsql.

## Decisões

Decisões duráveis são registradas como [Architecture Decision Records](decisions/README.md):

- [ADR 0001 — Manter o Core independente de frameworks de persistência](decisions/0001-core-persistence-independence.md)
- [ADR 0002 — Usar representação canônica em string para identificadores](decisions/0002-canonical-string-representation.md)
- [ADR 0003 — Preferir parsing estrito a sanitização silenciosa](decisions/0003-strict-parsing.md)
- [ADR 0004 — Tornar contexto obrigatório parte da identidade](decisions/0004-context-as-identity.md)

Veja também [Princípios de design](design-principles.md).
