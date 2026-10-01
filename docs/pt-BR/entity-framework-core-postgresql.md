# Entity Framework Core + PostgreSQL

O pacote `Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql` mantém a persistência PostgreSQL/Npgsql fora do pacote de domínio principal.

## RG e Inscrição Estadual

Os dois primitives possuem dois modos de persistência porque o contexto de UF é opcional no modelo de domínio.

### Modo sem contexto de UF

Use os converters de uma coluna quando o identificador não carrega uma UF:

```csharp
entity.Property(x => x.Rg)
    .HasConversion(new RgValueConverter())
    .HasMaxLength(10)
    .HasColumnType("character varying(10)");

entity.Property(x => x.InscricaoEstadual)
    .HasConversion(new InscricaoEstadualValueConverter())
    .HasMaxLength(14)
    .HasColumnType("character varying(14)");
```

Esse modo persiste somente o `Value` canônico. A materialização usa o parser sem contexto, portanto nenhuma UF é inferida. Se um valor que já possui UF for enviado a esses converters, a operação falha em vez de descartar silenciosamente o estado.

### Modo com UF

Quando o identificador possui estado explícito, persista `Value` e `State`:

```csharp
entity.ComplexProperty(
    x => x.Rg,
    complex => RgStateAwarePostgreSqlMapping.Configure(
        complex,
        "rg_value",
        "rg_state"));

entity.ComplexProperty(
    x => x.InscricaoEstadual,
    complex => InscricaoEstadualStateAwarePostgreSqlMapping.Configure(
        complex,
        "inscricao_value",
        "inscricao_state"));
```

A UF é armazenada por um código estável de duas letras, como `SP`, `MG` ou `RO`. `BrazilianState.Unknown` não é válido em um mapping state-aware.

### Nullabilidade

A nullabilidade da propriedade é independente do contexto de UF:

- `Rg? == null` ou `InscricaoEstadual? == null`: o identificador está ausente e o PostgreSQL armazena SQL `NULL`;
- valor context-free não nulo: o identificador existe, mas não possui UF;
- valor state-aware não nulo: identificador e UF são conhecidos.

Os três estados permanecem distintos. Nenhuma UF é inferida a partir do texto do identificador.

## Comportamento de validação

Valores não nulos persistidos são reidratados pelas regras públicas de criação/validação do domínio. Texto inválido no banco ou códigos de UF inválidos falham durante a materialização em vez de produzirem um value object default ou inválido.
