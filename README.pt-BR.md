# Brazilian.PrimitivesTypes

Brazilian.PrimitivesTypes é uma biblioteca .NET para modelar identificadores brasileiros como value objects imutáveis.

Ela valida e normaliza valores como CPF, CNPJ, CEP, chaves Pix, telefones brasileiros, placas, RENAVAM, CNH, CNS, título
eleitoral, inscrição estadual e identificadores bancários sem fazer chamadas externas.

For English documentation, see [README.en.md](README.en.md).

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

A integração Dapper é opcional e distribuída separadamente:

```bash
dotnet add package Brazilian.PrimitivesTypes.Dapper.SqlServer
```

Registre os handlers no bootstrap:

```csharp
using Brazilian.PrimitivesTypes.Dapper.SqlServer;

BrazilianPrimitivesDapperSqlServer.Register();
```

O pacote permite usar os primitives diretamente em parâmetros escalares e materializá-los em consultas Dapper. Ele configura `AnsiString`, tamanho e `Value` canônico, mas não cria schema nem migrations. As colunas SQL Server continuam sendo responsabilidade da aplicação e devem seguir os `varchar(n)` documentados.

`Rg` e `InscricaoEstadual` são Value-only nessa integração: UF não é persistida. List expansion (`IN @Values`) de coleções de primitives não passa item a item pelos handlers no Dapper 2.1.x e não é declarada como suportada.

Veja o guia completo de [Dapper com SQL Server](docs/pt-BR/dapper-sql-server.md).

## Entity Framework Core + SQL Server

A integração EF Core também é opcional:

```bash
dotnet add package Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer
```

Ela usa conventions/converters do EF Core e, ao contrário da integração Dapper, pode preservar RG e Inscrição Estadual em modo state-aware. Veja [Entity Framework Core com SQL Server](docs/pt-BR/entity-framework-core-sql-server.md).

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

A direção das dependências entre pacotes e suas restrições arquiteturais executáveis estão documentadas em [Arquitetura](docs/pt-BR/architecture.md). Decisões duráveis são registradas como [Architecture Decision Records](docs/pt-BR/decisions/README.md).

## Desenvolvimento

```bash
dotnet tool restore
dotnet restore --locked-mode
dotnet format Brazilian.PrimitivesTypes.slnx --verify-no-changes --no-restore
dotnet build Brazilian.PrimitivesTypes.slnx --configuration Release --no-restore
dotnet test Brazilian.PrimitivesTypes.slnx --configuration Release --no-build
```

Validação de pacotes:

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
