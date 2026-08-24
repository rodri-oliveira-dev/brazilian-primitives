using System.Globalization;
using Brazilian.PrimitivesTypes;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Contexts;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Mappings;

[Collection(SqlServerTestCollection.Name)]
public sealed class ExplicitFluentMappingSqlServerTests
{
    private const string ConventionDatabaseName = "BrazilianPrimitivesConventionTests";
    private const string ExplicitDatabaseName = "BrazilianPrimitivesExplicitMappingTests";
    private readonly SqlServerContainerFixture _fixture;

    public ExplicitFluentMappingSqlServerTests(SqlServerContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ModelWideAndExplicitMappingsHaveEquivalentSqlServerPersistenceSemantics()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        await using ConventionDbContext conventionContext = new(
            SqlServerDbContextOptionsFactory.Create<ConventionDbContext>(_fixture, ConventionDatabaseName));
        await using ExplicitMappingDbContext explicitContext = new(
            SqlServerDbContextOptionsFactory.Create<ExplicitMappingDbContext>(_fixture, ExplicitDatabaseName));

        try
        {
            await conventionContext.Database.EnsureDeletedAsync(cancellationToken);
            await explicitContext.Database.EnsureDeletedAsync(cancellationToken);
            await conventionContext.Database.EnsureCreatedAsync(cancellationToken);
            await explicitContext.Database.EnsureCreatedAsync(cancellationToken);

            Cpf cpf = Cpf.Parse("529.982.247-25", CultureInfo.InvariantCulture);
            Cep cep = Cep.Parse("01311-000", CultureInfo.InvariantCulture);

            conventionContext.MappingRecords.Add(new MappingRecord { Id = 1, Cpf = cpf, Email = null, Cep = cep });
            explicitContext.MappingRecords.Add(new MappingRecord { Id = 1, Cpf = cpf, Email = null, Cep = cep });

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
        using OverrideMappingDbContext context = new(
            SqlServerDbContextOptionsFactory.Create<OverrideMappingDbContext>(_fixture, ExplicitDatabaseName));

        IEntityType entityType = context.Model.FindEntityType(typeof(OverrideRecord))!;
        IProperty emailProperty = entityType.FindProperty(nameof(OverrideRecord.Email))!;

        Assert.Equal("ContactEmail", emailProperty.GetColumnName());
        Assert.Equal("varchar(320)", emailProperty.GetRelationalTypeMapping().StoreType);
        Assert.False(emailProperty.IsNullable);
        Assert.Empty(entityType.GetIndexes());
        Assert.Single(entityType.GetKeys());
    }

    private static void AssertEquivalentMappingMetadata(
        ConventionDbContext conventionContext,
        ExplicitMappingDbContext explicitContext)
    {
        IEntityType conventionType = conventionContext.Model.FindEntityType(typeof(MappingRecord))!;
        IEntityType explicitType = explicitContext.Model.FindEntityType(typeof(MappingRecord))!;

        foreach (string propertyName in new[] { nameof(MappingRecord.Cpf), nameof(MappingRecord.Email), nameof(MappingRecord.Cep) })
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
}
