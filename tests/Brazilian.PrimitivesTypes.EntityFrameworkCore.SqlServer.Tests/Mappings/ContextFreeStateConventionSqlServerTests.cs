using Brazilian.PrimitivesTypes;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Contexts;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Mappings;

[Collection(SqlServerTestCollection.Name)]
public sealed class ContextFreeStateConventionSqlServerTests
{
    private const string DatabaseName = "BrazilianPrimitivesContextFreeConventionTests";
    private readonly SqlServerContainerFixture _fixture;

    public ContextFreeStateConventionSqlServerTests(SqlServerContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ConventionUsesOneColumnAndKeepsNullDistinct()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        await using ContextFreeStateDbContext context = new(
            SqlServerDbContextOptionsFactory.Create<ContextFreeStateDbContext>(_fixture, DatabaseName));

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
            AssertContextFreeMetadata(context);
        }
        finally
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
        }
    }

    private static void AssertContextFreeMetadata(ContextFreeStateDbContext context)
    {
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
}
