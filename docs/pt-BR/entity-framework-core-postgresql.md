# Entity Framework Core + PostgreSQL

O pacote `Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql` mantém a persistência PostgreSQL/Npgsql fora do pacote de domínio principal. A configuração é explícita: apenas referenciar o pacote não modifica o modelo do EF Core.

## Opt-in para o modelo inteiro

Registre os primitives escalares em `ConfigureConventions`:

```csharp
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    configurationBuilder.UseBrazilianPrimitiveTypesPostgreSql();
}
```

Isso aplica converters, tamanhos máximos e store types PostgreSQL `character varying(n)` aos primitives escalares suportados. Propriedades CLR obrigatórias `T` e anuláveis `T?` mantêm a semântica normal de nullabilidade do EF Core.

`Rg` e `InscricaoEstadual` ficam intencionalmente fora dessa convenção porque o contexto de UF não pode ser inferido com segurança.

Se todos os RGs/IEs do modelo forem intencionalmente context-free, faça um opt-in separado:

```csharp
protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
{
    configurationBuilder
        .UseBrazilianPrimitiveTypesPostgreSql()
        .UseBrazilianContextFreeStateRegistrationsPostgreSql();
}
```

Esse segundo registro continua usando apenas uma coluna por identificador e nunca cria nem exige uma coluna de UF.

## Mappings explícitos por propriedade

Para controle pontual, use as extensões PostgreSQL:

```csharp
entity.Property(x => x.Cpf)
    .HasBrazilianCpfPostgreSql();

entity.Property(x => x.Email)
    .HasBrazilianEmailPostgreSql()
    .HasColumnName("contact_email");
```

Configurações normais do EF Core/Npgsql podem ser encadeadas depois. O consumidor pode sobrescrever nomes de coluna, nullabilidade, tipos PostgreSQL e outras facets. A biblioteca não cria índices, constraints de unicidade, chaves ou regras específicas de agregado.

## RG e Inscrição Estadual

Os dois primitives possuem dois modos de persistência porque o contexto de UF é opcional no modelo de domínio.

### Modo sem contexto de UF

Use os mappings explícitos de uma coluna quando o identificador não carrega UF:

```csharp
entity.Property(x => x.Rg)
    .HasBrazilianRgContextFreePostgreSql();

entity.Property(x => x.InscricaoEstadual)
    .HasBrazilianInscricaoEstadualContextFreePostgreSql();
```

Esse modo persiste somente o `Value` canônico. A materialização usa o parser sem contexto, portanto nenhuma UF é inferida. Se um valor com UF for enviado a esse mapping, a operação falha em vez de descartar silenciosamente o estado.

### Modo com UF

Quando o identificador possui estado explícito, mapeie-o como complex property para persistir `Value` e `State`:

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

A UF é armazenada por código estável de duas letras, como `SP`, `MG` ou `RO`. `BrazilianState.Unknown` não é válido em mapping state-aware.

### Nullabilidade

A nullabilidade da propriedade é independente do contexto de UF:

- `Rg? == null` ou `InscricaoEstadual? == null`: o identificador está ausente e o PostgreSQL armazena SQL `NULL`;
- valor context-free não nulo: o identificador existe, mas não possui UF;
- valor state-aware não nulo: identificador e UF são conhecidos.

Os três estados permanecem distintos. Nenhuma UF é inferida a partir do texto do identificador.

## Comportamento de validação

Valores persistidos não nulos são reidratados pelas regras públicas de criação/validação do domínio. Texto inválido no banco ou códigos de UF inválidos falham durante a materialização em vez de produzirem um value object default ou inválido.
