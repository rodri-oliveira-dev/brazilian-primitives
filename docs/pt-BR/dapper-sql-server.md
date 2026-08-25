# Dapper com SQL Server

`Brazilian.PrimitivesTypes.Dapper.SqlServer` é o pacote opcional de integração para aplicações que usam Dapper com SQL Server. Ele registra `TypeHandler<T>` para os Brazilian Primitive Types e permite enviar e materializar os value objects diretamente em comandos Dapper.

O pacote de domínio `Brazilian.PrimitivesTypes` continua independente de Dapper, SQL Server e qualquer provider ADO.NET.

## Instalação

Instale a integração:

```bash
dotnet add package Brazilian.PrimitivesTypes.Dapper.SqlServer
```

O pacote traz `Dapper` como dependência. Para abrir conexões SQL Server nos exemplos abaixo, instale também um provider ADO.NET, normalmente `Microsoft.Data.SqlClient`:

```bash
dotnet add package Microsoft.Data.SqlClient
```

Requisitos do pacote:

- .NET 10;
- Dapper 2.1.x;
- SQL Server;
- um provider ADO.NET para SQL Server no projeto consumidor, quando ele ainda não estiver presente.

## Registrar os handlers

O registro é explícito. Faça-o no bootstrap da aplicação, antes das primeiras operações Dapper:

```csharp
using Brazilian.PrimitivesTypes.Dapper.SqlServer;

BrazilianPrimitivesDapperSqlServer.Register();
```

O registro usa o registry global de type handlers do Dapper. Chamadas repetidas a `Register()` são seguras, mas a recomendação é registrar uma vez durante a inicialização da aplicação.

## Schema é responsabilidade da aplicação

Dapper não possui model builder nem migrations. Este pacote configura os parâmetros enviados ao provider e converte os valores retornados pelo banco, mas **não cria tabelas, colunas, índices ou migrations**.

O schema deve usar `varchar(n)` compatível com o contrato do primitive. Exemplo:

```sql
CREATE TABLE dbo.Customers
(
    Id BIGINT NOT NULL,
    Cpf VARCHAR(11) NOT NULL,
    Email VARCHAR(254) NULL,
    Cep VARCHAR(8) NOT NULL,
    CONSTRAINT PK_Customers PRIMARY KEY (Id)
);
```

Os handlers enviam os valores como `DbType.AnsiString`, com `Size` correspondente ao tamanho recomendado e com o `Value` canônico do primitive.

## Exemplo completo

### Modelo de leitura

```csharp
using Brazilian.PrimitivesTypes;

public sealed class CustomerRow
{
    public long Id { get; set; }

    public Cpf Cpf { get; set; }

    public Email? Email { get; set; }

    public Cep Cep { get; set; }
}
```

### Abrindo a conexão

```csharp
using Microsoft.Data.SqlClient;

await using SqlConnection connection = new(connectionString);
await connection.OpenAsync();
```

### INSERT com primitives

Depois do `Register()`, os value objects podem ser usados diretamente como parâmetros:

```csharp
using Brazilian.PrimitivesTypes;
using Dapper;

Cpf cpf = Cpf.Parse("529.982.247-25");
Email email = Email.Parse("USER@Example.COM");
Cep cep = Cep.Parse("01311-000");

await connection.ExecuteAsync(
    """
    INSERT INTO dbo.Customers (Id, Cpf, Email, Cep)
    VALUES (@Id, @Cpf, @Email, @Cep);
    """,
    new
    {
        Id = 1L,
        Cpf = cpf,
        Email = email,
        Cep = cep,
    });
```

O SQL Server recebe os valores canônicos. Por exemplo, o CPF acima é enviado como `52998224725` e o CEP como `01311000`.

### SELECT materializando diretamente os tipos fortes

```csharp
CustomerRow customer = await connection.QuerySingleAsync<CustomerRow>(
    """
    SELECT Id, Cpf, Email, Cep
    FROM dbo.Customers
    WHERE Id = @Id;
    """,
    new { Id = 1L });

Cpf cpf = customer.Cpf;
Email? email = customer.Email;
Cep cep = customer.Cep;
```

O Dapper entrega o valor de banco ao type handler correspondente, que recria o value object pelas regras do domínio. Texto não nulo e inválido no banco falha durante a materialização; ele não é transformado silenciosamente em um objeto inválido.

### WHERE parametrizado

Use o próprio primitive como parâmetro:

```csharp
Cpf cpf = Cpf.Parse("529.982.247-25");

CustomerRow? customer = await connection.QuerySingleOrDefaultAsync<CustomerRow>(
    """
    SELECT Id, Cpf, Email, Cep
    FROM dbo.Customers
    WHERE Cpf = @Cpf;
    """,
    new { Cpf = cpf });
```

Não é necessário converter `cpf` manualmente para `string` em parâmetros escalares.

### UPDATE

```csharp
Email updatedEmail = Email.Parse("new.user@example.com");
Cep updatedCep = Cep.Parse("04567-890");

await connection.ExecuteAsync(
    """
    UPDATE dbo.Customers
    SET Email = @Email,
        Cep = @Cep
    WHERE Cpf = @Cpf;
    """,
    new
    {
        Email = updatedEmail,
        Cep = updatedCep,
        Cpf = cpf,
    });
```

### Nullable e SQL NULL

Ausência deve ser representada por `T?`/`null`, não por string vazia ou por `default(T)`:

```csharp
Email? email = null;

await connection.ExecuteAsync(
    """
    UPDATE dbo.Customers
    SET Email = @Email
    WHERE Cpf = @Cpf;
    """,
    new
    {
        Email = email,
        Cpf = cpf,
    });
```

`null` é enviado como SQL `NULL`. Ao materializar uma coluna SQL `NULL` em uma propriedade nullable, o resultado volta como `null`.

Uma instância `default` de um primitive não representa ausência válida e não deve ser usada como sentinela.

## DynamicParameters

`DynamicParameters` também usa os handlers registrados:

```csharp
using Dapper;

DynamicParameters parameters = new();
parameters.Add("Cpf", cpf);
parameters.Add("Email", updatedEmail);

await connection.ExecuteAsync(
    """
    UPDATE dbo.Customers
    SET Email = @Email
    WHERE Cpf = @Cpf;
    """,
    parameters);
```

Não é necessário informar manualmente `DbType.AnsiString` ou o tamanho quando o valor é um primitive suportado: o handler correspondente configura esse metadata.

## List expansion / IN

A expansão de listas do Dapper não passa cada item pelo `ITypeHandler.SetValue` na versão 2.1.x. Portanto, este pacote **não declara suporte de type handler para `IN @Values` quando `Values` é uma coleção de Brazilian Primitive Types**.

Não dependa deste padrão:

```csharp
// Não suportado como cenário de type handler por item.
var parameters = new { Cpfs = new[] { cpf1, cpf2 } };
```

Se a aplicação precisar usar list expansion, converta deliberadamente a coleção para os valores canônicos antes de entregá-la ao Dapper, assumindo a responsabilidade por esse bypass explícito:

```csharp
var parameters = new
{
    Cpfs = cpfs.Select(item => item.Value).ToArray(),
};
```

Parâmetros escalares continuam usando os handlers normalmente.

## Tipos SQL recomendados

| Primitive | SQL Server recomendado |
| --- | --- |
| `Cpf` | `varchar(11)` |
| `Cnpj` | `varchar(14)` |
| `CpfCnpj` | `varchar(14)` |
| `Cep` | `varchar(8)` |
| `Email` | `varchar(254)` |
| `MobilePhone` | `varchar(11)` |
| `LandlinePhone` | `varchar(10)` |
| `TelefoneBrasileiro` | `varchar(11)` |
| `ChavePix` | `varchar(77)` |
| `Cnh` | `varchar(11)` |
| `Cns` | `varchar(15)` |
| `TituloEleitoral` | `varchar(12)` |
| `Nit` | `varchar(11)` |
| `PisPasep` | `varchar(11)` |
| `PlacaVeiculo` | `varchar(7)` |
| `Renavam` | `varchar(11)` |
| `Ispb` | `varchar(8)` |
| `CodigoCompe` | `varchar(3)` |
| `Rg` | `varchar(10)` |
| `InscricaoEstadual` | `varchar(14)` |

Os tamanhos acima correspondem ao contrato dos handlers. O pacote não aumenta automaticamente uma coluna existente nem cria constraints de schema.

## RG e Inscrição Estadual são Value-only

Na integração Dapper, `Rg` e `InscricaoEstadual` persistem **somente `Value`**. A UF não é gravada pelo handler e não é recuperada na materialização.

Mesmo que o objeto original tenha sido criado com estado:

```csharp
Rg rg = Rg.Parse("123456789", BrazilianState.Amazonas);
```

o parâmetro Dapper contém apenas `rg.Value`. Depois da leitura, o `Rg` recriado é context-free (`HasState == false`). A mesma regra vale para `InscricaoEstadual`.

Se a UF fizer parte do contrato de persistência da aplicação, crie e mantenha uma coluna adicional explicitamente ou use a integração EF Core state-aware, que oferece mapeamento próprio para valor + UF. O pacote Dapper não tenta inferir nem persistir esse contexto automaticamente.

## Dapper x Entity Framework Core

| Aspecto | Dapper | EF Core |
| --- | --- | --- |
| Pacote | `Brazilian.PrimitivesTypes.Dapper.SqlServer` | `Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer` |
| Integração | `TypeHandler<T>` global do Dapper | conventions, converters e Fluent API |
| Schema/migrations | responsabilidade da aplicação | EF Core pode modelar e gerar migrations |
| Parâmetros | configura `AnsiString`, tamanho e valor canônico | provider deriva o parâmetro do mapeamento EF |
| Materialização | handler recria o primitive a partir da coluna | converter/mapeamento recria o primitive |
| RG/IE com UF | Value-only | suporta context-free e state-aware |
| LINQ | não aplicável; SQL é explícito | consultas LINQ com tipos fortes |

Escolha a integração que corresponde ao mecanismo de persistência da aplicação. Não é necessário referenciar os dois pacotes.

## Empacotamento e dependências

O pacote distribuído contém apenas as dependências necessárias ao contrato de integração:

- `Brazilian.PrimitivesTypes`;
- `Dapper`.

Ele não referencia Entity Framework Core. O CI do repositório valida o pacote e também verifica o grafo de dependências de um consumidor local para impedir que EF Core seja introduzido transitivamente.

## Trusted Publishing no NuGet.org

Para mantenedores: o release oficial usa Trusted Publishing do NuGet.org via OIDC no workflow `.github/workflows/release.yml`.

Na configuração atual do NuGet.org, uma policy de Trusted Publishing pertence a um usuário ou organização e se aplica aos pacotes pertencentes a esse owner; ela não precisa ser duplicada apenas porque um novo package ID foi adicionado. Antes da primeira publicação de `Brazilian.PrimitivesTypes.Dapper.SqlServer`, confirme operacionalmente que:

1. a policy está ativa para o owner correto;
2. Repository Owner, Repository e Workflow File correspondem a `rodri-oliveira-dev`, `brazilian-primitives` e `release.yml`;
3. `NUGET_USER` no GitHub corresponde ao usuário autorizado pela policy;
4. o package ID está disponível ou já pertence ao mesmo owner.

Essa configuração é feita no portal do NuGet.org e não pode ser criada ou alterada pelo código deste repositório.
