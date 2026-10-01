using Brazilian.PrimitivesTypes;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Contexts;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Mappings;

[Collection(PostgreSqlTestCollection.Name)]
public sealed class OptionalStatePostgreSqlTests
{
    private const string DatabaseName = "brazilian_primitives_optional_state";
    private readonly PostgreSqlContainerFixture _fixture;

    public OptionalStatePostgreSqlTests(PostgreSqlContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ContextFreeAndStateAwareValuesRoundTripWithoutLosingState()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        await using OptionalStateDbContext context = new(CreateOptions());

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
                OptionalInscricaoEstadual =
                    InscricaoEstadual.Parse("00000000625213", BrazilianState.Rondonia),
            };

            StateAwareRecord nullOptional = new()
            {
                Id = 2,
                Rg = Rg.Parse("123456789", BrazilianState.Amazonas),
                OptionalRg = null,
                InscricaoEstadual = InscricaoEstadual.Parse("12345678", BrazilianState.Bahia),
                OptionalInscricaoEstadual = null,
            };

            context.ContextFreeRecords.Add(contextFree);
            context.StateAwareRecords.AddRange(stateAware, nullOptional);
            await context.SaveChangesAsync(cancellationToken);
            context.ChangeTracker.Clear();

            ContextFreeStateRecord loadedContextFree = await context.ContextFreeRecords
                .AsNoTracking()
                .SingleAsync(record => record.Id == 1, cancellationToken);
            StateAwareRecord loadedStateAware = await context.StateAwareRecords
                .AsNoTracking()
                .SingleAsync(record => record.Id == 1, cancellationToken);
            StateAwareRecord loadedNullOptional = await context.StateAwareRecords
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

            Assert.Null(loadedNullOptional.OptionalRg);
            Assert.Null(loadedNullOptional.OptionalInscricaoEstadual);

            Assert.NotEqual(loadedContextFree.Rg, loadedNullOptional.Rg);
            Assert.False(loadedContextFree.Rg.HasState);
            Assert.True(loadedNullOptional.Rg.HasState);

            AssertColumnMetadata(context);
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
        await using OptionalStateDbContext context = new(CreateOptions());

        try
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
            await context.Database.EnsureCreatedAsync(cancellationToken);

            await context.Database.ExecuteSqlRawAsync(
                """
                INSERT INTO integration.state_aware_records
                    (id, rg_value, rg_state, inscricao_value, inscricao_state)
                VALUES
                    (7, '120300012', 'SP', '110042490114', 'SP')
                """,
                cancellationToken);
            context.ChangeTracker.Clear();

            await Assert.ThrowsAnyAsync<Exception>(
                () => context.StateAwareRecords
                    .AsNoTracking()
                    .SingleAsync(record => record.Id == 7, cancellationToken));
        }
        finally
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
        }
    }

    private DbContextOptions<OptionalStateDbContext> CreateOptions() =>
        PostgreSqlDbContextOptionsFactory.Create<OptionalStateDbContext>(_fixture, DatabaseName);

    private static void AssertColumnMetadata(OptionalStateDbContext context)
    {
        IEntityType contextFree = context.Model.FindEntityType(typeof(ContextFreeStateRecord))!;
        Assert.Equal(
            "character varying(10)",
            contextFree.FindProperty(nameof(ContextFreeStateRecord.Rg))!.GetRelationalTypeMapping().StoreType);
        Assert.Equal(
            "character varying(14)",
            contextFree.FindProperty(nameof(ContextFreeStateRecord.InscricaoEstadual))!.GetRelationalTypeMapping().StoreType);

        IEntityType stateAware = context.Model.FindEntityType(typeof(StateAwareRecord))!;
        Assert.Equal(
            "character varying(10)",
            stateAware.FindComplexProperty(nameof(StateAwareRecord.Rg))!
                .ComplexType.FindProperty(nameof(Rg.Value))!.GetRelationalTypeMapping().StoreType);
        Assert.Equal(
            "character varying(2)",
            stateAware.FindComplexProperty(nameof(StateAwareRecord.Rg))!
                .ComplexType.FindProperty(nameof(Rg.State))!.GetRelationalTypeMapping().StoreType);
        Assert.Equal(
            "character varying(14)",
            stateAware.FindComplexProperty(nameof(StateAwareRecord.InscricaoEstadual))!
                .ComplexType.FindProperty(nameof(InscricaoEstadual.Value))!.GetRelationalTypeMapping().StoreType);
        Assert.Equal(
            "character varying(2)",
            stateAware.FindComplexProperty(nameof(StateAwareRecord.InscricaoEstadual))!
                .ComplexType.FindProperty(nameof(InscricaoEstadual.State))!.GetRelationalTypeMapping().StoreType);
    }
}
