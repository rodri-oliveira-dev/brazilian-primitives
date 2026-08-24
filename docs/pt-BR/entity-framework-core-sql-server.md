# Entity Framework Core com SQL Server

`Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer` é o pacote opcional de integração para aplicações que usam Entity Framework Core com SQL Server. O pacote de domínio `Brazilian.PrimitivesTypes` continua independente de EF Core e de qualquer provider de banco de dados.

## Instalação

No projeto que contém o modelo de domínio:

```bash
dotnet add package Brazilian.PrimitivesTypes
```

No projeto de persistência que usa EF Core SQL Server:

```bash
dotnet add package Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer
```

O pacote SQL Server referencia `Brazilian.PrimitivesTypes`, mas mantê-los separados permite que o domínio continue sem dependência de persistência.

## Exemplo completo com `Customer`

O domínio permanece fortemente tipado:

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

Configure o provider SQL Server normalmente:

```csharp
services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
```

### Convenções para o modelo inteiro

A forma recomendada quando o modelo usa vários primitivos escalares é registrar as pre-conventions explicitamente:

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

Apenas referenciar o pacote não modifica o modelo. `UseBrazilianPrimitiveTypesSqlServer()` é opt-in.

### Mapeamento explícito por propriedade

Quando preferir configuração local, use as extensões Fluent API:

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

As configurações continuam sendo builders normais do EF Core. Facets aplicadas depois das extensões, como nome de coluna, tipo SQL ou required/optional, podem sobrescrever os defaults quando forem compatíveis com o tipo CLR.

## O que é armazenado

O banco recebe o `Value` canônico, nunca a máscara de apresentação nem o texto original digitado.

Exemplo conceitual para `Customer`:

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

Para estes valores:

```csharp
Cpf cpf = Cpf.Parse("529.982.247-25");
Email email = Email.Parse("USER@Example.COM");
Cep cep = Cep.Parse("01311-000");
```

o SQL Server recebe os valores canônicos correspondentes, por exemplo `52998224725` para CPF e `01311000` para CEP. A normalização própria de cada value object também é preservada na persistência.

`Formatted` é exclusivamente uma visão de apresentação. Não persista `Formatted` por padrão.

## Nullabilidade

A validade de um value object e a ausência de um valor são conceitos diferentes:

- `Email` significa que existe um e-mail válido segundo as regras locais do tipo;
- `Email?` permite ausência;
- `Email? == null` é persistido como SQL `NULL`;
- SQL `NULL` é materializado novamente como `null` sem chamar `Email.Parse`;
- texto não nulo e inválido no banco não vira `null` nem um value object inválido: a materialização falha de forma previsível.

Não use string vazia, `default(Email)` ou sentinelas para representar ausência.

## Consultas LINQ com tipos fortes

O converter participa da tradução da consulta, portanto o código continua usando o tipo de domínio:

```csharp
Cpf cpf = Cpf.Parse("529.982.247-25");

Customer? customer = await dbContext.Customers
    .SingleOrDefaultAsync(x => x.Cpf == cpf);
```

Não é necessário converter manualmente `cpf` para `string` na expressão LINQ.

## RG e Inscrição Estadual sem UF

`Rg` e `InscricaoEstadual` têm dois modos explícitos de persistência. Sem UF, a validação é estrutural e conservadora e somente `Value` é armazenado.

Mapeamento por propriedade:

```csharp
entity.Property(x => x.Rg)
    .HasBrazilianRgContextFreeSqlServer();

entity.Property(x => x.InscricaoEstadual)
    .HasBrazilianInscricaoEstadualContextFreeSqlServer();
```

Se todas as propriedades RG/IE de um modelo forem intencionalmente context-free, também é possível registrar:

```csharp
protected override void ConfigureConventions(
    ModelConfigurationBuilder configurationBuilder)
{
    configurationBuilder
        .UseBrazilianPrimitiveTypesSqlServer()
        .UseBrazilianContextFreeStateRegistrationsSqlServer();
}
```

Nesse modo:

```csharp
Rg rg = Rg.Parse("00000005X");

Console.WriteLine(rg.HasState); // false
Console.WriteLine(rg.State);    // BrazilianState.Unknown
```

Nenhuma UF é inferida a partir do texto. O schema precisa somente da coluna de valor, por exemplo `Rg VARCHAR(10)` ou `InscricaoEstadual VARCHAR(14)`.

Uma propriedade `Rg?` igual a `null` significa que o RG inteiro está ausente. Isso é diferente de um `Rg` não nulo context-free, que possui `Value`, `HasState == false` e `State == BrazilianState.Unknown`. A mesma regra vale para `InscricaoEstadual?`.

## RG e Inscrição Estadual com UF

Quando a UF é conhecida, use o modo state-aware para não perder contexto. A criação continua usando a validação estadual mais forte disponível:

```csharp
Rg rg = Rg.Parse("120300011", BrazilianState.SaoPaulo);
InscricaoEstadual ie = InscricaoEstadual.Parse(
    "110042490114",
    BrazilianState.SaoPaulo);
```

Mapeie cada value object como complex property:

```csharp
entity.ComplexProperty(x => x.Rg)
    .HasBrazilianRgStateAwareSqlServer("RgValue", "RgState");

entity.ComplexProperty(x => x.InscricaoEstadual)
    .HasBrazilianInscricaoEstadualStateAwareSqlServer(
        "InscricaoValue",
        "InscricaoState");
```

O schema passa a preservar duas informações:

```sql
RgValue VARCHAR(10) NOT NULL,
RgState VARCHAR(2) NOT NULL,
InscricaoValue VARCHAR(14) NOT NULL,
InscricaoState VARCHAR(2) NOT NULL
```

As UFs são persistidas por códigos estáveis de duas letras, como `SP`, `RJ` e `MG`. Uma instância com UF conhecida não pode ser gravada pelo converter context-free: a operação falha em vez de descartar silenciosamente o estado.

Propriedades state-aware nullable, como `Rg?`, podem representar ausência do identificador inteiro. Quando não nulas, `Value + State` são preservados juntos.

## Migrações e constraints

Os mappings fornecem conversão, tamanho e configuração não Unicode apropriados ao SQL Server. A nullabilidade segue o tipo CLR e pode ser refinada pelo modelo quando permitido.

A integração **não** cria automaticamente:

- índices;
- unique constraints;
- alternate keys;
- primary keys;
- regras específicas do seu agregado.

Essas decisões pertencem ao modelo da aplicação:

```csharp
entity.HasIndex(x => x.Cpf).IsUnique();
```

Adicione-as somente quando fizerem sentido para a regra de negócio.

## Limites de validação

Persistir ou materializar um primitive executa apenas as regras locais implementadas pelo value object. A integração EF Core não consulta Receita Federal, Correios, DICT, SEFAZ, DETRAN ou qualquer cadastro externo.

Um valor estrutural ou matematicamente válido pode não existir ou não estar ativo em uma base oficial. A camada de persistência não muda esse contrato.
