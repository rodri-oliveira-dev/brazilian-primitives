using System.Globalization;
using Brazilian.PrimitivesTypes;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests;

[Collection(SqlServerTestCollection.Name)]
public sealed class ConventionAndFluentMappingSqlServerTests
{
    private const string ConventionDatabaseName = "BrazilianPrimitivesConventionTests";
    private const string ExplicitDatabaseName = "BrazilianPrimitivesExplicitMappingTests";
    private const string ContextFreeStateDatabaseName = "BrazilianPrimitivesContextFreeConventionTests";
    private const string StateAwareDatabaseName = "BrazilianPrimitivesStateAwareFluentTests";

    private readonly SqlServerContainerFixture _fixture;

    public ConventionAndFluentMappingSqlServerTests(SqlServerContainerFixture fixture)
    {
        _fixture = fixture;
    }

    public static TheoryData<string, Type, string, bool> ConventionMappings => new()
    {
        { nameof(AllConventionRecord.Cpf), typeof(CpfValueConverter), "varchar(11)", false },
        { nameof(AllConventionRecord.Cnpj), typeof(CnpjValueConverter), "varchar(14)", false },
        { nameof(AllConventionRecord.CpfCnpj), typeof(CpfCnpjValueConverter), "varchar(14)", false },
        { nameof(AllConventionRecord.Cep), typeof(CepValueConverter), "varchar(8)", false },
        { nameof(AllConventionRecord.Email), typeof(EmailValueConverter), "varchar(254)", false },
        { nameof(AllConventionRecord.OptionalEmail), typeof(EmailValueConverter), "varchar(254)", true },
        { nameof(AllConventionRecord.MobilePhone), typeof(MobilePhoneValueConverter), "varchar(11)", false },
        { nameof(AllConventionRecord.LandlinePhone), typeof(LandlinePhoneValueConverter), "varchar(10)", false },
        { nameof(AllConventionRecord.TelefoneBrasileiro), typeof(TelefoneBrasileiroValueConverter), "varchar(11)", false },
        { nameof(AllConventionRecord.ChavePix), typeof(ChavePixValueConverter), "varchar(77)", false },
        { nameof(AllConventionRecord.Cnh), typeof(CnhValueConverter), "varchar(11)", false },
        { nameof(AllConventionRecord.Cns), typeof(CnsValueConverter), "varchar(15)", false },
        { nameof(AllConventionRecord.TituloEleitoral), typeof(TituloEleitoralValueConverter), "varchar(12)", false },
        { nameof(AllConventionRecord.Nit), typeof(NitValueConverter), "varchar(11)", false },
        { nameof(AllConventionRecord.PisPasep), typeof(PisPasepValueConverter), "varchar(11)", false },
        { nameof(AllConventionRecord.PlacaVeiculo), typeof(PlacaVeiculoValueConverter), "varchar(7)", false },
        { nameof(AllConventionRecord.Renavam), typeof(RenavamValueConverter), "varchar(11)", false },
        { nameof(AllConventionRecord.Ispb), typeof(IspbValueConverter), "varchar(8)", false },
        { nameof(AllConventionRecord.CodigoCompe), typeof(CodigoCompeValueConverter), "varchar(3)", false },
    };

    [Theory]
    [MemberData(nameof(ConventionMappings))]
    public void ModelWideConventionRegistersExpectedScalarConverterAndSqlServerFacets(
        string propertyName,
        Type converterType,
        string storeType,
        bool isNullable)
    {
        DbContextOptions<ConventionDbContext> options = CreateOptions<ConventionDbContext>(ConventionDatabaseName);
        using ConventionDbContext context = new(options);

        IProperty property = context.Model.FindEntityType(typeof(AllConventionRecord))!
            .FindProperty(propertyName)!;

        Assert.IsType(converterType, property.GetValueConverter());
        Assert.Equal(storeType, property.GetRelationalTypeMapping().StoreType);
        Assert.Equal(isNullable, property.IsNullable);
    }

    [Fact]
    public async Task ModelWideAndExplicitMappingsHaveEquivalentSqlServerPersistenceSemantics()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        DbContextOptions<ConventionDbContext> conventionOptions = CreateOptions<ConventionDbContext>(ConventionDatabaseName);
        DbContextOptions<ExplicitDbContext> explicitOptions = CreateOptions<ExplicitDbContext>(ExplicitDatabaseName);

        await using ConventionDbContext conventionContext = new(conventionOptions);
        await using ExplicitDbContext explicitContext = new(explicitOptions);

        try
        {
            await conventionContext.Database.EnsureDeletedAsync(cancellationToken);
            await explicitContext.Database.EnsureDeletedAsync(cancellationToken);
            await conventionContext.Database.EnsureCreatedAsync(cancellationToken);
            await explicitContext.Database.EnsureCreatedAsync(cancellationToken);

            Cpf cpf = Cpf.Parse("529.982.247-25", CultureInfo.InvariantCulture);
            Cep cep = Cep.Parse("01311-000", CultureInfo.InvariantCulture);

            conventionContext.MappingRecords.Add(new MappingRecord
            {
                Id = 1,
                Cpf = cpf,
                Email = null,
                Cep = cep,
            });
            explicitContext.MappingRecords.Add(new MappingRecord
            {
                Id = 1,
                Cpf = cpf,
                Email = null,
                Cep = cep,
            });

            await conventionContext.SaveChangesAsync(cancellationToken);
            await explicitContext.SaveChangesAsync(cancellationToken);
            conventionContext.ChangeTracker.Clear();
            explicitContext.ChangeTracker.Clear();

            MappingRecord conventionRecord = await conventionContext.MappingRecords
                .AsNoTracking()
                .SingleAsync(record => record.Cpf == cpf, cancellationToken);
            MappingRecord explicitRecord = await explicitContext.MappingRecords
                .AsNoTracking()
                .SingleAsync(record => record.Cpf == cpf, cancellationToken);

            Assert.Equal(conventionRecord.Cpf, explicitRecord.Cpf);
            Assert.Equal(conventionRecord.Cep, explicitRecord.Cep);
            Assert.Null(conventionRecord.Email);
            Assert.Null(explicitRecord.Email);

            AssertEquivalentMappingMetadata(conventionContext, explicitContext);
        }
        finally
        {
            await conventionContext.Database.EnsureDeletedAsync(cancellationToken);
            await explicitContext.Database.EnsureDeletedAsync(cancellationToken);
        }
    }

    [Fact]
    public void ExplicitFluentMappingAllowsConsumerFacetOverridesWithoutCreatingIndexes()
    {
        DbContextOptions<OverrideDbContext> options = CreateOptions<OverrideDbContext>(ExplicitDatabaseName);
        using OverrideDbContext context = new(options);

        IEntityType entityType = context.Model.FindEntityType(typeof(OverrideRecord))!;
        IProperty emailProperty = entityType.FindProperty(nameof(OverrideRecord.Email))!;

        Assert.Equal("ContactEmail", emailProperty.GetColumnName());
        Assert.Equal("varchar(320)", emailProperty.GetRelationalTypeMapping().StoreType);
        Assert.False(emailProperty.IsNullable);
        Assert.Empty(entityType.GetIndexes());
        Assert.Single(entityType.GetKeys());
    }

    [Fact]
    public async Task ContextFreeStateRegistrationConventionUsesOneColumnAndKeepsNullDistinct()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        DbContextOptions<ContextFreeStateConventionDbContext> options =
            CreateOptions<ContextFreeStateConventionDbContext>(ContextFreeStateDatabaseName);
        await using ContextFreeStateConventionDbContext context = new(options);

        try
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
            await context.Database.EnsureCreatedAsync(cancellationToken);

            ContextFreeStateRecord expected = new()
            {
                Id = 1,
                Rg = Rg.Parse("00000005x"),
                OptionalRg = null,
                InscricaoEstadual = InscricaoEstadual.Parse("0012345678"),
                OptionalInscricaoEstadual = null,
            };

            context.Records.Add(expected);
            await context.SaveChangesAsync(cancellationToken);
            context.ChangeTracker.Clear();

            ContextFreeStateRecord actual = await context.Records.AsNoTracking()
                .SingleAsync(record => record.Id == 1, cancellationToken);

            Assert.Equal(expected.Rg, actual.Rg);
            Assert.False(actual.Rg.HasState);
            Assert.Null(actual.OptionalRg);
            Assert.Equal(expected.InscricaoEstadual, actual.InscricaoEstadual);
            Assert.False(actual.InscricaoEstadual.HasState);
            Assert.Null(actual.OptionalInscricaoEstadual);

            IEntityType entityType = context.Model.FindEntityType(typeof(ContextFreeStateRecord))!;
            Assert.Equal(
                "varchar(10)",
                entityType.FindProperty(nameof(ContextFreeStateRecord.Rg))!.GetRelationalTypeMapping().StoreType);
            Assert.Equal(
                "varchar(14)",
                entityType.FindProperty(nameof(ContextFreeStateRecord.InscricaoEstadual))!
                    .GetRelationalTypeMapping().StoreType);
            Assert.Null(entityType.FindComplexProperty(nameof(ContextFreeStateRecord.Rg)));
            Assert.Null(entityType.FindComplexProperty(nameof(ContextFreeStateRecord.InscricaoEstadual)));
        }
        finally
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
        }
    }

    [Fact]
    public async Task StateAwareFluentExtensionsPreserveUfAndSupportNullableProperties()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        DbContextOptions<StateAwareFluentDbContext> options = CreateOptions<StateAwareFluentDbContext>(StateAwareDatabaseName);
        await using StateAwareFluentDbContext context = new(options);

        try
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
            await context.Database.EnsureCreatedAsync(cancellationToken);

            StateAwareFluentRecord expected = new()
            {
                Id = 1,
                Rg = Rg.Parse("120300011", BrazilianState.SaoPaulo),
                OptionalRg = null,
                InscricaoEstadual = InscricaoEstadual.Parse("110042490114", BrazilianState.SaoPaulo),
                OptionalInscricaoEstadual = null,
            };

            context.Records.Add(expected);
            await context.SaveChangesAsync(cancellationToken);
            context.ChangeTracker.Clear();

            StateAwareFluentRecord actual = await context.Records.AsNoTracking()
                .SingleAsync(record => record.Id == 1, cancellationToken);

            Assert.Equal(expected.Rg, actual.Rg);
            Assert.Equal(BrazilianState.SaoPaulo, actual.Rg.State);
            Assert.Null(actual.OptionalRg);
            Assert.Equal(expected.InscricaoEstadual, actual.InscricaoEstadual);
            Assert.Equal(BrazilianState.SaoPaulo, actual.InscricaoEstadual.State);
            Assert.Null(actual.OptionalInscricaoEstadual);

            IEntityType entityType = context.Model.FindEntityType(typeof(StateAwareFluentRecord))!;
            IComplexProperty rg = entityType.FindComplexProperty(nameof(StateAwareFluentRecord.Rg))!;
            Assert.Equal("RgNumber", rg.ComplexType.FindProperty(nameof(Rg.Value))!.GetColumnName());
            Assert.Equal("RgUf", rg.ComplexType.FindProperty(nameof(Rg.State))!.GetColumnName());
            Assert.Equal(
                "varchar(12)",
                rg.ComplexType.FindProperty(nameof(Rg.Value))!.GetRelationalTypeMapping().StoreType);
            Assert.Empty(entityType.GetIndexes());
        }
        finally
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
        }
    }

    private DbContextOptions<TContext> CreateOptions<TContext>(string databaseName)
        where TContext : DbContext
    {
        return new DbContextOptionsBuilder<TContext>()
            .UseSqlServer(_fixture.GetConnectionString(databaseName))
            .Options;
    }

    private static void AssertEquivalentMappingMetadata(
        ConventionDbContext conventionContext,
        ExplicitDbContext explicitContext)
    {
        IEntityType conventionType = conventionContext.Model.FindEntityType(typeof(MappingRecord))!;
        IEntityType explicitType = explicitContext.Model.FindEntityType(typeof(MappingRecord))!;

        foreach (string propertyName in new[]
                 {
                     nameof(MappingRecord.Cpf),
                     nameof(MappingRecord.Email),
                     nameof(MappingRecord.Cep),
                 })
        {
            IProperty conventionProperty = conventionType.FindProperty(propertyName)!;
            IProperty explicitProperty = explicitType.FindProperty(propertyName)!;

            Assert.Equal(
                conventionProperty.GetRelationalTypeMapping().StoreType,
                explicitProperty.GetRelationalTypeMapping().StoreType);
            Assert.Equal(conventionProperty.IsNullable, explicitProperty.IsNullable);
            Assert.Equal(
                conventionProperty.GetValueConverter()!.GetType(),
                explicitProperty.GetValueConverter()!.GetType());
        }

        Assert.Empty(conventionType.GetIndexes());
        Assert.Empty(explicitType.GetIndexes());
    }

    private sealed class ConventionDbContext(DbContextOptions<ConventionDbContext> options) : DbContext(options)
    {
        public DbSet<AllConventionRecord> AllConventionRecords => Set<AllConventionRecord>();

        public DbSet<MappingRecord> MappingRecords => Set<MappingRecord>();

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.UseBrazilianPrimitiveTypesSqlServer();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureEntity(modelBuilder.Entity<AllConventionRecord>(), "AllConventionRecords");
            ConfigureEntity(modelBuilder.Entity<MappingRecord>(), "MappingRecords");
        }
    }

    private sealed class ExplicitDbContext(DbContextOptions<ExplicitDbContext> options) : DbContext(options)
    {
        public DbSet<MappingRecord> MappingRecords => Set<MappingRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            EntityTypeBuilder<MappingRecord> entity = modelBuilder.Entity<MappingRecord>();
            ConfigureEntity(entity, "MappingRecords");
            entity.Property(record => record.Cpf).HasBrazilianCpfSqlServer();
            entity.Property(record => record.Email).HasBrazilianEmailSqlServer();
            entity.Property(record => record.Cep).HasBrazilianCepSqlServer();
        }
    }

    private sealed class OverrideDbContext(DbContextOptions<OverrideDbContext> options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            EntityTypeBuilder<OverrideRecord> entity = modelBuilder.Entity<OverrideRecord>();
            ConfigureEntity(entity, "OverrideRecords");
            entity.Property(record => record.Email)
                .HasBrazilianEmailSqlServer()
                .HasColumnName("ContactEmail")
                .HasColumnType("varchar(320)")
                .IsRequired();
        }
    }

    private sealed class ContextFreeStateConventionDbContext(DbContextOptions<ContextFreeStateConventionDbContext> options)
        : DbContext(options)
    {
        public DbSet<ContextFreeStateRecord> Records => Set<ContextFreeStateRecord>();

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder
                .UseBrazilianPrimitiveTypesSqlServer()
                .UseBrazilianContextFreeStateRegistrationsSqlServer();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            ConfigureEntity(modelBuilder.Entity<ContextFreeStateRecord>(), "ContextFreeStateRecords");
        }
    }

    private sealed class StateAwareFluentDbContext(DbContextOptions<StateAwareFluentDbContext> options) : DbContext(options)
    {
        public DbSet<StateAwareFluentRecord> Records => Set<StateAwareFluentRecord>();

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.UseBrazilianPrimitiveTypesSqlServer();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            EntityTypeBuilder<StateAwareFluentRecord> entity = modelBuilder.Entity<StateAwareFluentRecord>();
            ConfigureEntity(entity, "StateAwareFluentRecords");

            ComplexPropertyBuilder<Rg> rg = entity.ComplexProperty(record => record.Rg)
                .HasBrazilianRgStateAwareSqlServer("RgNumber", "RgUf");
            rg.Property(value => value.Value).HasColumnType("varchar(12)");

            entity.ComplexProperty(record => record.OptionalRg)
                .HasBrazilianRgStateAwareSqlServer("OptionalRgNumber", "OptionalRgUf");
            entity.ComplexProperty(record => record.InscricaoEstadual)
                .HasBrazilianInscricaoEstadualStateAwareSqlServer("IeNumber", "IeUf");
            entity.ComplexProperty(record => record.OptionalInscricaoEstadual)
                .HasBrazilianInscricaoEstadualStateAwareSqlServer("OptionalIeNumber", "OptionalIeUf");
        }
    }

    private static void ConfigureEntity<TEntity>(EntityTypeBuilder<TEntity> entity, string tableName)
        where TEntity : class, IRecordWithId
    {
        entity.ToTable(tableName);
        entity.HasKey(record => record.Id);
        entity.Property(record => record.Id).ValueGeneratedNever();
    }

    private interface IRecordWithId
    {
        int Id
        {
            get;
            set;
        }
    }

    private sealed class MappingRecord : IRecordWithId
    {
        public int Id
        {
            get;
            set;
        }

        public Cpf Cpf
        {
            get;
            set;
        }

        public Email? Email
        {
            get;
            set;
        }

        public Cep Cep
        {
            get;
            set;
        }
    }

    private sealed class OverrideRecord : IRecordWithId
    {
        public int Id
        {
            get;
            set;
        }

        public Email? Email
        {
            get;
            set;
        }
    }

    private sealed class ContextFreeStateRecord : IRecordWithId
    {
        public int Id
        {
            get;
            set;
        }

        public Rg Rg
        {
            get;
            set;
        }

        public Rg? OptionalRg
        {
            get;
            set;
        }

        public InscricaoEstadual InscricaoEstadual
        {
            get;
            set;
        }

        public InscricaoEstadual? OptionalInscricaoEstadual
        {
            get;
            set;
        }
    }

    private sealed class StateAwareFluentRecord : IRecordWithId
    {
        public int Id
        {
            get;
            set;
        }

        public Rg Rg
        {
            get;
            set;
        }

        public Rg? OptionalRg
        {
            get;
            set;
        }

        public InscricaoEstadual InscricaoEstadual
        {
            get;
            set;
        }

        public InscricaoEstadual? OptionalInscricaoEstadual
        {
            get;
            set;
        }
    }

    private sealed class AllConventionRecord : IRecordWithId
    {
        public int Id
        {
            get;
            set;
        }

        public Cpf Cpf
        {
            get;
            set;
        }

        public Cnpj Cnpj
        {
            get;
            set;
        }

        public CpfCnpj CpfCnpj
        {
            get;
            set;
        }

        public Cep Cep
        {
            get;
            set;
        }

        public Email Email
        {
            get;
            set;
        }

        public Email? OptionalEmail
        {
            get;
            set;
        }

        public MobilePhone MobilePhone
        {
            get;
            set;
        }

        public LandlinePhone LandlinePhone
        {
            get;
            set;
        }

        public TelefoneBrasileiro TelefoneBrasileiro
        {
            get;
            set;
        }

        public ChavePix ChavePix
        {
            get;
            set;
        }

        public Cnh Cnh
        {
            get;
            set;
        }

        public Cns Cns
        {
            get;
            set;
        }

        public TituloEleitoral TituloEleitoral
        {
            get;
            set;
        }

        public Nit Nit
        {
            get;
            set;
        }

        public PisPasep PisPasep
        {
            get;
            set;
        }

        public PlacaVeiculo PlacaVeiculo
        {
            get;
            set;
        }

        public Renavam Renavam
        {
            get;
            set;
        }

        public Ispb Ispb
        {
            get;
            set;
        }

        public CodigoCompe CodigoCompe
        {
            get;
            set;
        }
    }
}
