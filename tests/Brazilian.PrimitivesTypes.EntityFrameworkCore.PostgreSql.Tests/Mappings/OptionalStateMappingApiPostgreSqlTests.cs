using Brazilian.PrimitivesTypes;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Contexts;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Mappings;

[Collection(PostgreSqlTestCollection.Name)]
public sealed class OptionalStateMappingApiPostgreSqlTests
{
    private readonly PostgreSqlContainerFixture _fixture;

    public OptionalStateMappingApiPostgreSqlTests(PostgreSqlContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ExplicitContextFreeAndStateAwareExtensionsPreserveTheirSelectedSemantics()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        await using ExplicitStateMappingDbContext context = new(
            PostgreSqlDbContextOptionsFactory.Create<ExplicitStateMappingDbContext>(
                _fixture,
                "brazilian_primitives_explicit_state_api"));

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
                OptionalRg = null,
                InscricaoEstadual = InscricaoEstadual.Parse("00000000625213", BrazilianState.Rondonia),
                OptionalInscricaoEstadual = null,
            };

            context.ContextFreeRecords.Add(contextFree);
            context.StateAwareRecords.Add(stateAware);
            await context.SaveChangesAsync(cancellationToken);
            context.ChangeTracker.Clear();

            ContextFreeStateRecord loadedContextFree = await context.ContextFreeRecords.AsNoTracking().SingleAsync(cancellationToken);
            StateAwareRecord loadedStateAware = await context.StateAwareRecords.AsNoTracking().SingleAsync(cancellationToken);

            Assert.Equal(contextFree.Rg, loadedContextFree.Rg);
            Assert.False(loadedContextFree.Rg.HasState);
            Assert.Null(loadedContextFree.OptionalRg);
            Assert.Equal(contextFree.InscricaoEstadual, loadedContextFree.InscricaoEstadual);
            Assert.False(loadedContextFree.InscricaoEstadual.HasState);
            Assert.Null(loadedContextFree.OptionalInscricaoEstadual);

            Assert.Equal(stateAware.Rg, loadedStateAware.Rg);
            Assert.Equal(BrazilianState.SaoPaulo, loadedStateAware.Rg.State);
            Assert.Null(loadedStateAware.OptionalRg);
            Assert.Equal(stateAware.InscricaoEstadual, loadedStateAware.InscricaoEstadual);
            Assert.Equal(BrazilianState.Rondonia, loadedStateAware.InscricaoEstadual.State);
            Assert.Null(loadedStateAware.OptionalInscricaoEstadual);

            IEntityType stateAwareType = context.Model.FindEntityType(typeof(StateAwareRecord))!;
            Assert.Equal(
                "rg_value",
                stateAwareType.FindComplexProperty(nameof(StateAwareRecord.Rg))!
                    .ComplexType.FindProperty(nameof(Rg.Value))!.GetColumnName());
            Assert.Equal(
                "rg_state",
                stateAwareType.FindComplexProperty(nameof(StateAwareRecord.Rg))!
                    .ComplexType.FindProperty(nameof(Rg.State))!.GetColumnName());
            Assert.Empty(stateAwareType.GetIndexes());
        }
        finally
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
        }
    }

    [Fact]
    public async Task ContextFreeModelWideOptInDoesNotCreateUfColumns()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        await using ContextFreeConventionDbContext context = new(
            PostgreSqlDbContextOptionsFactory.Create<ContextFreeConventionDbContext>(
                _fixture,
                "brazilian_primitives_context_free_convention"));

        try
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
            await context.Database.EnsureCreatedAsync(cancellationToken);

            context.Records.Add(new ContextFreeStateRecord
            {
                Id = 1,
                Rg = Rg.Parse("123456789"),
                OptionalRg = null,
                InscricaoEstadual = InscricaoEstadual.Parse("0012345678"),
                OptionalInscricaoEstadual = null,
            });
            await context.SaveChangesAsync(cancellationToken);
            context.ChangeTracker.Clear();

            ContextFreeStateRecord actual = await context.Records.AsNoTracking().SingleAsync(cancellationToken);
            Assert.False(actual.Rg.HasState);
            Assert.False(actual.InscricaoEstadual.HasState);
            Assert.Null(actual.OptionalRg);
            Assert.Null(actual.OptionalInscricaoEstadual);

            IEntityType entityType = context.Model.FindEntityType(typeof(ContextFreeStateRecord))!;
            Assert.Null(entityType.FindProperty("RgState"));
            Assert.Null(entityType.FindProperty("InscricaoEstadualState"));
            Assert.Equal(5, entityType.GetProperties().Count());
            Assert.Empty(entityType.GetComplexProperties());
            Assert.Empty(entityType.GetIndexes());
        }
        finally
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
        }
    }
}
