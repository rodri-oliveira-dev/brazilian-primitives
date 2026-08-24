using Brazilian.PrimitivesTypes;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Contexts;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Mappings;

[Collection(SqlServerTestCollection.Name)]
public sealed class StateAwareFluentMappingSqlServerTests
{
    private const string DatabaseName = "BrazilianPrimitivesStateAwareTests";
    private const string OverrideDatabaseName = "BrazilianPrimitivesStateAwareFluentTests";
    private readonly SqlServerContainerFixture _fixture;

    public StateAwareFluentMappingSqlServerTests(SqlServerContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ContextFreeAndStateAwareMappingsRoundTripWithoutLosingState()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        await using StateAwareDbContext context = new(
            SqlServerDbContextOptionsFactory.Create<StateAwareDbContext>(_fixture, DatabaseName));

        try
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
            await context.Database.EnsureCreatedAsync(cancellationToken);

            ContextFreeStateRecord contextFree = new()
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

            ContextFreeStateRecord loadedContextFree = await context.ContextFreeRecords
                .AsNoTracking()
                .SingleAsync(record => record.Id == 1, cancellationToken);
            StateAwareRecord loadedStateAware = await context.StateAwareRecords
                .AsNoTracking()
                .SingleAsync(record => record.Id == 1, cancellationToken);
            StateAwareRecord loadedNullStateAwareProperties = await context.StateAwareRecords
                .AsNoTracking()
                .SingleAsync(record => record.Id == 2, cancellationToken);

            AssertContextFreeRoundTrip(contextFree, loadedContextFree);
            AssertStateAwareRoundTrip(stateAware, loadedStateAware);
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
    public async Task StateAwareFluentExtensionsPreserveUfAndSupportNullableProperties()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        await using StateAwareOverrideDbContext context = new(
            SqlServerDbContextOptionsFactory.Create<StateAwareOverrideDbContext>(_fixture, OverrideDatabaseName));

        try
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
            await context.Database.EnsureCreatedAsync(cancellationToken);

            StateAwareRecord expected = new()
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

            StateAwareRecord actual = await context.Records.AsNoTracking()
                .SingleAsync(record => record.Id == 1, cancellationToken);

            Assert.Equal(expected.Rg, actual.Rg);
            Assert.Equal(BrazilianState.SaoPaulo, actual.Rg.State);
            Assert.Null(actual.OptionalRg);
            Assert.Equal(expected.InscricaoEstadual, actual.InscricaoEstadual);
            Assert.Equal(BrazilianState.SaoPaulo, actual.InscricaoEstadual.State);
            Assert.Null(actual.OptionalInscricaoEstadual);
            AssertOverrideMetadata(context);
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
        await using StateAwareDbContext context = new(
            SqlServerDbContextOptionsFactory.Create<StateAwareDbContext>(_fixture, DatabaseName));

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

    private static void AssertContextFreeRoundTrip(ContextFreeStateRecord expected, ContextFreeStateRecord actual)
    {
        Assert.Equal(expected.Rg, actual.Rg);
        Assert.False(actual.Rg.HasState);
        Assert.Null(actual.OptionalRg);
        Assert.Equal(expected.InscricaoEstadual, actual.InscricaoEstadual);
        Assert.False(actual.InscricaoEstadual.HasState);
        Assert.Null(actual.OptionalInscricaoEstadual);
    }

    private static void AssertStateAwareRoundTrip(StateAwareRecord expected, StateAwareRecord actual)
    {
        Assert.Equal(expected.Rg, actual.Rg);
        Assert.Equal(BrazilianState.SaoPaulo, actual.Rg.State);
        Assert.Equal(expected.OptionalRg, actual.OptionalRg);
        Assert.Equal(BrazilianState.MinasGerais, actual.OptionalRg!.Value.State);
        Assert.Equal(expected.InscricaoEstadual, actual.InscricaoEstadual);
        Assert.Equal(BrazilianState.SaoPaulo, actual.InscricaoEstadual.State);
        Assert.Equal(expected.OptionalInscricaoEstadual, actual.OptionalInscricaoEstadual);
        Assert.Equal(BrazilianState.Rondonia, actual.OptionalInscricaoEstadual!.Value.State);
    }

    private static void AssertContextFreeColumnTypes(StateAwareDbContext context)
    {
        IEntityType entityType = context.Model.FindEntityType(typeof(ContextFreeStateRecord))!;

        Assert.Equal("varchar(10)", entityType.FindProperty(nameof(ContextFreeStateRecord.Rg))!.GetRelationalTypeMapping().StoreType);
        Assert.Equal("varchar(14)", entityType.FindProperty(nameof(ContextFreeStateRecord.InscricaoEstadual))!.GetRelationalTypeMapping().StoreType);
        Assert.Equal("varchar(10)", entityType.FindProperty(nameof(ContextFreeStateRecord.OptionalRg))!.GetRelationalTypeMapping().StoreType);
        Assert.Equal("varchar(14)", entityType.FindProperty(nameof(ContextFreeStateRecord.OptionalInscricaoEstadual))!.GetRelationalTypeMapping().StoreType);
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

    private static void AssertOverrideMetadata(StateAwareOverrideDbContext context)
    {
        IEntityType entityType = context.Model.FindEntityType(typeof(StateAwareRecord))!;
        IComplexProperty rg = entityType.FindComplexProperty(nameof(StateAwareRecord.Rg))!;

        Assert.Equal("RgNumber", rg.ComplexType.FindProperty(nameof(Rg.Value))!.GetColumnName());
        Assert.Equal("RgUf", rg.ComplexType.FindProperty(nameof(Rg.State))!.GetColumnName());
        Assert.Equal(
            "varchar(12)",
            rg.ComplexType.FindProperty(nameof(Rg.Value))!.GetRelationalTypeMapping().StoreType);
        Assert.Empty(entityType.GetIndexes());
    }
}
