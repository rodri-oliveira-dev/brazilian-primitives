# Entity Framework Core + PostgreSQL

`Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql` é a integração opcional de Entity Framework Core para PostgreSQL/Npgsql. O pacote de domínio continua independente de persistência; instale esta integração apenas no projeto de infraestrutura/persistência que contém o modelo do EF Core.

## Instalação

```bash
dotnet add package Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql
```

O pacote depende de `Brazilian.PrimitivesTypes`, das APIs relacionais do EF Core e de `Npgsql.EntityFrameworkCore.PostgreSQL`. Apenas referenciar o pacote **não** modifica o modelo do EF Core: a configuração é explícita e opt-in.

## Exemplo com Customer

A entidade de domínio continua usando os tipos fortes:

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

Configure o Npgsql normalmente:

```csharp
services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));
```

### Conventions para o modelo inteiro

Para aplicar os mappings PostgreSQL padrão aos primitives escalares, registre a integração em `ConfigureConventions`:

```csharp
using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;
using Microsoft.EntityFrameworkCore;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();

    protected override void ConfigureConventions(
        ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.UseBrazilianPrimitiveTypesPostgreSql();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Cpf).HasColumnName("cpf");
            entity.Property(x => x.Email).HasColumnName("email");
            entity.Property(x => x.Cep).HasColumnName("cep");
        });
    }
}
```

As conventions configuram conversão canônica, tamanho máximo intrínseco e store types PostgreSQL `character varying(n)`. A nullabilidade CLR é preservada: `Cpf` e `Cep` são obrigatórios e `Email?` é anulável.

### Mapping explícito por propriedade

Quando você preferir enxergar o mapping propriedade por propriedade:

```csharp
entity.Property(x => x.Cpf)
    .HasBrazilianCpfPostgreSql();

entity.Property(x => x.Email)
    .HasBrazilianEmailPostgreSql();

entity.Property(x => x.Cep)
    .HasBrazilianCepPostgreSql();
```

Configurações normais do EF Core/Npgsql podem ser encadeadas depois:

```csharp
entity.Property(x => x.Email)
    .HasBrazilianEmailPostgreSql()
    .HasColumnName("contact_email")
    .HasColumnType("text");
```

O pacote não cria automaticamente índices, constraints de unicidade, chaves primárias/estrangeiras ou regras específicas do agregado.

## Representação canônica no PostgreSQL

A persistência usa o `Value` canônico de cada primitive, nunca `Formatted` nem o texto original recebido.

Exemplos:

| Valor de domínio | Valor no PostgreSQL |
| --- | --- |
| `Cpf.Parse("529.982.247-25")` | `52998224725` |
| `Cep.Parse("01311-000")` | `01311000` |
| `Email.Parse("User@Domínio.com")` | `User@xn--domnio-5va.com` |

Zeros à esquerda e a capitalização/normalização canônica de valores alfanuméricos são preservados.

`Formatted` é destinado à apresentação e não deve ser a representação persistida padrão.

## Insert, leitura, query e update

```csharp
Cpf cpf = Cpf.Parse("529.982.247-25");

Customer customer = new()
{
    Id = 1,
    Cpf = cpf,
    Email = Email.Parse("User@Domínio.com"),
    Cep = Cep.Parse("01311-000"),
};

db.Customers.Add(customer);
await db.SaveChangesAsync();

Customer loaded = await db.Customers
    .SingleAsync(x => x.Cpf == cpf);

loaded.Email = Email.Parse("updated@domínio.com");
loaded.Cep = Cep.Parse("01001-000");

await db.SaveChangesAsync();
```

A expressão LINQ compara a propriedade fortemente tipada e o EF Core envia ao PostgreSQL o valor canônico do provider.

## Primitives anuláveis e dados inválidos

`Email? == null` representa ausência e é persistido como SQL `NULL`. Na materialização o resultado volta como `null`; o provider não tenta fazer parse de um valor ausente e não cria uma instância `default`.

Isso é diferente de um valor **não nulo e inválido** armazenado no banco. Se a coluna contiver, por exemplo, `not-an-email`, a materialização falha através do parser público que preserva as invariantes do primitive. O provider não fabrica um objeto inválido/default.

## RG e Inscrição Estadual

Os dois tipos possuem contexto opcional de `BrazilianState`. Nullabilidade da propriedade e presença de UF são conceitos independentes.

### Sem UF: uma coluna

Quando o identificador existe, mas a UF não é conhecida:

```csharp
entity.Property(x => x.Rg)
    .HasBrazilianRgContextFreePostgreSql();

entity.Property(x => x.InscricaoEstadual)
    .HasBrazilianInscricaoEstadualContextFreePostgreSql();
```

Somente o `Value` canônico é persistido:

```text
RG                 -> character varying(10)
InscricaoEstadual  -> character varying(14)
```

A materialização chama `Rg.Parse(value)` / `InscricaoEstadual.Parse(value)`. Nenhuma UF é inferida a partir do identificador.

Se uma instância que já possui UF for enviada ao converter context-free, a operação é rejeitada em vez de descartar silenciosamente o estado.

Quando todos os RGs/IEs de um modelo forem intencionalmente context-free, a escolha também pode ser global:

```csharp
protected override void ConfigureConventions(
    ModelConfigurationBuilder configurationBuilder)
{
    configurationBuilder
        .UseBrazilianPrimitiveTypesPostgreSql()
        .UseBrazilianContextFreeStateRegistrationsPostgreSql();
}
```

### Com UF: Value + estado

Quando a UF é explicitamente conhecida, mapeie o primitive como complex property:

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

O PostgreSQL armazena as duas informações:

```text
rg_value        character varying(10)
rg_state        character varying(2)
inscricao_value character varying(14)
inscricao_state character varying(2)
```

A UF usa códigos estáveis de duas letras, como `SP`, `MG` e `RO`. A materialização preserva o estado e, portanto, a semântica de igualdade original do domínio.

Quando o primitive do core possui uma regra de validação específica para determinada UF, informar o estado habilita essa validação local mais forte. O provider de persistência nunca consulta DETRAN, SEFAZ, SINTEGRA ou outros cadastros externos.

### Três estados diferentes

Estes casos são diferentes por definição:

1. `Rg? == null`: o identificador inteiro está ausente e é SQL `NULL`.
2. `Rg` não nulo com `HasState == false` / `State == BrazilianState.Unknown`: o identificador existe sem UF.
3. `Rg` não nulo com `HasState == true`: identificador e UF explícita são conhecidos.

A mesma distinção vale para `InscricaoEstadual`.

## Schema PostgreSQL esperado

Para o exemplo de `Customer`, os mappings padrão geram semântica equivalente a:

```sql
CREATE TABLE customers (
    id bigint NOT NULL,
    cpf character varying(11) NOT NULL,
    email character varying(254) NULL,
    cep character varying(8) NOT NULL,
    CONSTRAINT pk_customers PRIMARY KEY (id)
);
```

No modo context-free, RG/IE possuem somente a coluna de valor. No modo state-aware há uma coluna adicional `character varying(2)` para UF. A integração não adiciona índices nem unicidade automática para CPF, CNPJ, e-mail, RG ou outros primitives.

## PostgreSQL versus SQL Server

A integração PostgreSQL não copia o modelo ANSI/Unicode do SQL Server. Os tipos textuais do PostgreSQL usam a codificação do banco; o provider aplica `character varying(n)` para limites intrínsecos em vez de SQL Server `varchar(n)` com `IsUnicode(false)`.

Os dois providers preservam as mesmas invariantes de domínio, mas possuem metadata relacional específica de cada banco. As integrações SQL Server e PostgreSQL são pacotes separados e não dependem uma da outra.

## Limite de validação

Todo parse e validação realizados pelos value objects são locais e determinísticos. Persistir um valor não prova que o identificador existe, está ativo, pertence a alguém ou está cadastrado em uma base oficial.
