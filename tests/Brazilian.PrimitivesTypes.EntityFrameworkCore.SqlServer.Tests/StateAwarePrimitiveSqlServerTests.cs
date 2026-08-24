using Brazilian.PrimitivesTypes;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests;

[Collection(SqlServerTestCollection.Name)]
public sealed class StateAwarePrimitiveSqlServerTests
{
    private const string DatabaseName = "BrazilianPrimitivesStateAwareTests";
    private readonly SqlServerContainerFixture _fixture;

    public StateAwarePrimitiveSqlServerTests(SqlServerContainerFixture fixture)
    {
        _fixture = fixture;
    }

    public static TheoryData<BrazilianState, string> StateCodes => new()
    {
        { BrazilianState.Acre, "AC" },
        { BrazilianState.Alagoas, "AL" },
        { BrazilianState.Amapa, "AP" },
        { BrazilianState.Amazonas, "AM" },
        { BrazilianState.Bahia, "BA" },
        { BrazilianState.Ceara, "CE" },
        { BrazilianState.DistritoFederal, "DF" },
        { BrazilianState.EspiritoSanto, "ES" },
        { BrazilianState.Goias, "GO" },
        { BrazilianState.Maranhao, "MA" },
        { BrazilianState.MatoGrosso, "MT" },
        { BrazilianState.MatoGrossoDoSul, "MS" },
        { BrazilianState.MinasGerais, "MG" },
        { BrazilianState.Para, "PA" },
        { BrazilianState.Paraiba, "PB" },
        { BrazilianState.Parana, "PR" },
        { BrazilianState.Pernambuco, "PE" },
        { BrazilianState.Piaui, "PI" },
        { BrazilianState.RioDeJaneiro, "RJ" },
        { BrazilianState.RioGrandeDoNorte, "RN" },
        { BrazilianState.RioGrandeDoSul, "RS" },
        { BrazilianState.Rondonia, "RO" },
        { BrazilianState.Roraima, "RR" },
        { BrazilianState.SantaCatarina, "SC" },
        { BrazilianState.SaoPaulo, "SP" },
        { BrazilianState.Sergipe, "SE" },
        { BrazilianState.Tocantins, "TO" },
    };

    [Fact]
    public void ContextFreeConvertersRoundTripAndRefuseToDiscardKnownState()
    {
        RgValueConverter rgConverter = new();
        InscricaoEstadualValueConverter inscricaoConverter = new();

        Rg contextFreeRg = Rg.Parse("00000005x");
        InscricaoEstadual contextFreeInscricao = InscricaoEstadual.Parse("0012345678");

        Assert.Equal("00000005X", rgConverter.ConvertToProvider(contextFreeRg));
        Assert.Equal(contextFreeRg, rgConverter.ConvertFromProvider("00000005X"));
        Assert.Equal("0012345678", inscricaoConverter.ConvertToProvider(contextFreeInscricao));
        Assert.Equal(contextFreeInscricao, inscricaoConverter.ConvertFromProvider("0012345678"));

        Rg stateAwareRg = Rg.Parse("123456789", BrazilianState.Amazonas);
        InscricaoEstadual stateAwareInscricao = InscricaoEstadual.Parse("110042490114", BrazilianState.SaoPaulo);

        Assert.Throws<InvalidOperationException>(() => rgConverter.ConvertToProvider(stateAwareRg));
        Assert.Throws<InvalidOperationException>(() => inscricaoConverter.ConvertToProvider(stateAwareInscricao));
    }

    [Theory]
    [MemberData(nameof(StateCodes))]
    public void BrazilianStateConverterRoundTripsEveryStableUfCode(BrazilianState state, string code)
    {
        BrazilianStateCodeValueConverter converter = new();

        Assert.Equal(code, converter.ConvertToProvider(state));
        Assert.Equal(state, converter.ConvertFromProvider(code));
    }

    [Fact]
    public void BrazilianStateConverterRejectsUnknownStateAndInvalidCode()
    {
        BrazilianStateCodeValueConverter converter = new();

        Assert.Throws<InvalidOperationException>(() => converter.ConvertToProvider(BrazilianState.Unknown));
        Assert.Throws<FormatException>(() => converter.ConvertFromProvider("XX"));
    }

    [Fact]
    public async Task ContextFreeAndStateAwareMappingsRoundTripWithoutLosingState()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        DbContextOptions<StateAwareDbContext> options = CreateOptions();
        await using StateAwareDbContext context = new(options);

        try
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
            await context.Database.EnsureCreatedAsync(cancellationToken);

            ContextFreeRecord contextFree = new()
            {
                Id = 1,
                Rg = Rg.Parse("00000005x"),
                OptionalRg = null,
                InscricaoEstadual = InscricaoEstadual.Parse("0012345678"),
                OptionalInscricaoEstadual = null,
            };

            StateAwareRecord stateAware = new()
            {
                Id = 1,
                Rg = Rg.Parse("120300011", BrazilianState.SaoPaulo),
                OptionalRg = Rg.Parse("12345678", BrazilianState.MinasGerais),
                InscricaoEstadual = InscricaoEstadual.Parse("110042490114", BrazilianState.SaoPaulo),
                OptionalInscricaoEstadual = InscricaoEstadual.Parse("00000000625213", BrazilianState.Rondonia),
            };

            StateAwareRecord nullStateAwareProperties = new()
            {
                Id = 2,
                Rg = Rg.Parse("123456789", BrazilianState.Amazonas),
                OptionalRg = null,
                InscricaoEstadual = InscricaoEstadual.Parse("12345678", BrazilianState.Bahia),
                OptionalInscricaoEstadual = null,
            };

            context.ContextFreeRecords.Add(contextFree);
            context.StateAwareRecords.AddRange(stateAware, nullStateAwareProperties);
            await context.SaveChangesAsync(cancellationToken);
            context.ChangeTracker.Clear();

            ContextFreeRecord loadedContextFree = await context.ContextFreeRecords
                .AsNoTracking()
                .SingleAsync(record => record.Id == 1, cancellationToken);
            StateAwareRecord loadedStateAware = await context.StateAwareRecords
                .AsNoTracking()
                .SingleAsync(record => record.Id == 1, cancellationToken);
            StateAwareRecord loadedNullStateAwareProperties = await context.StateAwareRecords
                .AsNoTracking()
                .SingleAsync(record => record.Id == 2, cancellationToken);

            Assert.Equal(contextFree.Rg, loadedContextFree.Rg);
            Assert.False(loadedContextFree.Rg.HasState);
            Assert.Null(loadedContextFree.OptionalRg);
            Assert.Equal(contextFree.InscricaoEstadual, loadedContextFree.InscricaoEstadual);
            Assert.False(loadedContextFree.InscricaoEstadual.HasState);
            Assert.Null(loadedContextFree.OptionalInscricaoEstadual);

            Assert.Equal(stateAware.Rg, loadedStateAware.Rg);
            Assert.Equal(BrazilianState.SaoPaulo, loadedStateAware.Rg.State);
            Assert.Equal(stateAware.OptionalRg, loadedStateAware.OptionalRg);
            Assert.Equal(BrazilianState.MinasGerais, loadedStateAware.OptionalRg!.Value.State);
            Assert.Equal(stateAware.InscricaoEstadual, loadedStateAware.InscricaoEstadual);
            Assert.Equal(BrazilianState.SaoPaulo, loadedStateAware.InscricaoEstadual.State);
            Assert.Equal(stateAware.OptionalInscricaoEstadual, loadedStateAware.OptionalInscricaoEstadual);
            Assert.Equal(BrazilianState.Rondonia, loadedStateAware.OptionalInscricaoEstadual!.Value.State);

            Assert.Null(loadedNullStateAwareProperties.OptionalRg);
            Assert.Null(loadedNullStateAwareProperties.OptionalInscricaoEstadual);

            AssertContextFreeColumnTypes(context);
            AssertStateAwareColumnTypes(context);
        }
        finally
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
        }
    }

    [Fact]
    public async Task InvalidPersistedStateAwareRgFailsDuringMaterialization()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        DbContextOptions<StateAwareDbContext> options = CreateOptions();
        await using StateAwareDbContext context = new(options);

        try
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
            await context.Database.EnsureCreatedAsync(cancellationToken);
            await context.Database.ExecuteSqlRawAsync(
                "INSERT INTO [StateAwareRecords] ([Id], [RgValue], [RgState], [InscricaoValue], [InscricaoState]) VALUES (7, '120300012', 'SP', '110042490114', 'SP')",
                cancellationToken);
            context.ChangeTracker.Clear();

            await Assert.ThrowsAnyAsync<Exception>(
                () => context.StateAwareRecords.AsNoTracking().SingleAsync(record => record.Id == 7, cancellationToken));
        }
        finally
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
        }
    }

    private DbContextOptions<StateAwareDbContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<StateAwareDbContext>()
            .UseSqlServer(_fixture.GetConnectionString(DatabaseName))
            .Options;
    }

    private static void AssertContextFreeColumnTypes(StateAwareDbContext context)
    {
        IEntityType entityType = context.Model.FindEntityType(typeof(ContextFreeRecord))!;

        Assert.Equal("varchar(10)", entityType.FindProperty(nameof(ContextFreeRecord.Rg))!.GetRelationalTypeMapping().StoreType);
        Assert.Equal("varchar(14)", entityType.FindProperty(nameof(ContextFreeRecord.InscricaoEstadual))!.GetRelationalTypeMapping().StoreType);
        Assert.Equal("varchar(10)", entityType.FindProperty(nameof(ContextFreeRecord.OptionalRg))!.GetRelationalTypeMapping().StoreType);
        Assert.Equal("varchar(14)", entityType.FindProperty(nameof(ContextFreeRecord.OptionalInscricaoEstadual))!.GetRelationalTypeMapping().StoreType);
    }

    private static void AssertStateAwareColumnTypes(StateAwareDbContext context)
    {
        IEntityType entityType = context.Model.FindEntityType(typeof(StateAwareRecord))!;

        Assert.Equal("varchar(10)", entityType.FindComplexProperty(nameof(StateAwareRecord.Rg))!
            .ComplexType.FindProperty(nameof(Rg.Value))!.GetRelationalTypeMapping().StoreType);
        Assert.Equal("varchar(2)", entityType.FindComplexProperty(nameof(StateAwareRecord.Rg))!
            .ComplexType.FindProperty(nameof(Rg.State))!.GetRelationalTypeMapping().StoreType);
        Assert.Equal("varchar(14)", entityType.FindComplexProperty(nameof(StateAwareRecord.InscricaoEstadual))!
            .ComplexType.FindProperty(nameof(InscricaoEstadual.Value))!.GetRelationalTypeMapping().StoreType);
        Assert.Equal("varchar(2)", entityType.FindComplexProperty(nameof(StateAwareRecord.InscricaoEstadual))!
            .ComplexType.FindProperty(nameof(InscricaoEstadual.State))!.GetRelationalTypeMapping().StoreType);
    }

    private sealed class StateAwareDbContext(DbContextOptions<StateAwareDbContext> options) : DbContext(options)
    {
        public DbSet<ContextFreeRecord> ContextFreeRecords => Set<ContextFreeRecord>();

        public DbSet<StateAwareRecord> StateAwareRecords => Set<StateAwareRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ContextFreeRecord>(entity =>
            {
                entity.ToTable("ContextFreeRecords");
                entity.HasKey(record => record.Id);
                entity.Property(record => record.Id).ValueGeneratedNever();
                entity.Property(record => record.Rg)
                    .HasConversion(new RgValueConverter());
                entity.Property(record => record.OptionalRg)
                    .HasConversion(new RgValueConverter());
                entity.Property(record => record.InscricaoEstadual)
                    .HasConversion(new InscricaoEstadualValueConverter());
                entity.Property(record => record.OptionalInscricaoEstadual)
                    .HasConversion(new InscricaoEstadualValueConverter());
            });

            modelBuilder.Entity<StateAwareRecord>(entity =>
            {
                entity.ToTable("StateAwareRecords");
                entity.HasKey(record => record.Id);
                entity.Property(record => record.Id).ValueGeneratedNever();
                entity.ComplexProperty(
                    record => record.Rg,
                    complex => RgStateAwareSqlServerMapping.Configure(complex, "RgValue", "RgState"));
                entity.ComplexProperty(
                    record => record.OptionalRg,
                    complex => RgStateAwareSqlServerMapping.Configure(complex, "OptionalRgValue", "OptionalRgState"));
                entity.ComplexProperty(
                    record => record.InscricaoEstadual,
                    complex => InscricaoEstadualStateAwareSqlServerMapping.Configure(complex, "InscricaoValue", "InscricaoState"));
                entity.ComplexProperty(
                    record => record.OptionalInscricaoEstadual,
                    complex => InscricaoEstadualStateAwareSqlServerMapping.Configure(
                        complex,
                        "OptionalInscricaoValue",
                        "OptionalInscricaoState"));
            });
        }
    }

    private sealed class ContextFreeRecord
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

    private sealed class StateAwareRecord
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
}
