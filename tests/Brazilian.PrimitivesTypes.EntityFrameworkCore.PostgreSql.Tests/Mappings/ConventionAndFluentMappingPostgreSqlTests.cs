using System.Globalization;
using Brazilian.PrimitivesTypes;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Contexts;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Mappings;

[Collection(PostgreSqlTestCollection.Name)]
public sealed class ConventionAndFluentMappingPostgreSqlTests
{
    private const string ConventionDatabaseName = "brazilian_primitives_convention_api";
    private const string ExplicitDatabaseName = "brazilian_primitives_explicit_api";
    private const string NullableDatabaseName = "brazilian_primitives_nullable_convention";

    private readonly PostgreSqlContainerFixture _fixture;

    public ConventionAndFluentMappingPostgreSqlTests(PostgreSqlContainerFixture fixture)
    {
        _fixture = fixture;
    }

    public static TheoryData<string, Type, string, bool> ConventionMappings => new()
    {
        { nameof(ScalarPrimitiveRecord.Cpf), typeof(CpfValueConverter), "character varying(11)", false },
        { nameof(ScalarPrimitiveRecord.Cnpj), typeof(CnpjValueConverter), "character varying(14)", false },
        { nameof(ScalarPrimitiveRecord.CpfCnpj), typeof(CpfCnpjValueConverter), "character varying(14)", false },
        { nameof(ScalarPrimitiveRecord.Cep), typeof(CepValueConverter), "character varying(8)", false },
        { nameof(ScalarPrimitiveRecord.Email), typeof(EmailValueConverter), "character varying(254)", false },
        { nameof(ScalarPrimitiveRecord.OptionalEmail), typeof(EmailValueConverter), "character varying(254)", true },
        { nameof(ScalarPrimitiveRecord.MobilePhone), typeof(MobilePhoneValueConverter), "character varying(11)", false },
        { nameof(ScalarPrimitiveRecord.LandlinePhone), typeof(LandlinePhoneValueConverter), "character varying(10)", false },
        { nameof(ScalarPrimitiveRecord.TelefoneBrasileiro), typeof(TelefoneBrasileiroValueConverter), "character varying(11)", false },
        { nameof(ScalarPrimitiveRecord.ChavePix), typeof(ChavePixValueConverter), "character varying(77)", false },
        { nameof(ScalarPrimitiveRecord.Cnh), typeof(CnhValueConverter), "character varying(11)", false },
        { nameof(ScalarPrimitiveRecord.Cns), typeof(CnsValueConverter), "character varying(15)", false },
        { nameof(ScalarPrimitiveRecord.TituloEleitoral), typeof(TituloEleitoralValueConverter), "character varying(12)", false },
        { nameof(ScalarPrimitiveRecord.Nit), typeof(NitValueConverter), "character varying(11)", false },
        { nameof(ScalarPrimitiveRecord.PisPasep), typeof(PisPasepValueConverter), "character varying(11)", false },
        { nameof(ScalarPrimitiveRecord.PlacaVeiculo), typeof(PlacaVeiculoValueConverter), "character varying(7)", false },
        { nameof(ScalarPrimitiveRecord.Renavam), typeof(RenavamValueConverter), "character varying(11)", false },
        { nameof(ScalarPrimitiveRecord.Ispb), typeof(IspbValueConverter), "character varying(8)", false },
        { nameof(ScalarPrimitiveRecord.CodigoCompe), typeof(CodigoCompeValueConverter), "character varying(3)", false },
    };

    private static readonly string[] NullablePropertyNames =
    [
        nameof(NullableConventionRecord.Cpf),
        nameof(NullableConventionRecord.Cnpj),
        nameof(NullableConventionRecord.CpfCnpj),
        nameof(NullableConventionRecord.Cep),
        nameof(NullableConventionRecord.Email),
        nameof(NullableConventionRecord.MobilePhone),
        nameof(NullableConventionRecord.LandlinePhone),
        nameof(NullableConventionRecord.TelefoneBrasileiro),
        nameof(NullableConventionRecord.ChavePix),
        nameof(NullableConventionRecord.Cnh),
        nameof(NullableConventionRecord.Cns),
        nameof(NullableConventionRecord.TituloEleitoral),
        nameof(NullableConventionRecord.Nit),
        nameof(NullableConventionRecord.PisPasep),
        nameof(NullableConventionRecord.PlacaVeiculo),
        nameof(NullableConventionRecord.Renavam),
        nameof(NullableConventionRecord.Ispb),
        nameof(NullableConventionRecord.CodigoCompe),
    ];

    [Theory]
    [MemberData(nameof(ConventionMappings))]
    public void ModelWideConventionRegistersExpectedPostgreSqlMappings(
        string propertyName,
        Type converterType,
        string storeType,
        bool nullable)
    {
        using ConventionMappingDbContext context = new(CreateConventionOptions(ConventionDatabaseName));

        IProperty property = context.Model.FindEntityType(typeof(ScalarPrimitiveRecord))!
            .FindProperty(propertyName)!;

        Assert.IsType(converterType, property.GetValueConverter());
        Assert.Equal(storeType, property.GetRelationalTypeMapping().StoreType);
        Assert.Equal(nullable, property.IsNullable);
    }

    [Fact]
    public void PackageReferenceWithoutOptInDoesNotConfigureBrazilianPrimitives()
    {
        using NoOptInDbContext context = new(
            PostgreSqlDbContextOptionsFactory.Create<NoOptInDbContext>(_fixture, "brazilian_primitives_no_opt_in"));

        Assert.Throws<InvalidOperationException>(() => _ = context.Model);
    }

    [Fact]
    public async Task ModelWideAndExplicitMappingsHaveEquivalentPersistenceSemantics()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        await using ConventionMappingDbContext conventionContext = new(CreateConventionOptions(ConventionDatabaseName));
        await using ExplicitMappingDbContext explicitContext = new(
            PostgreSqlDbContextOptionsFactory.Create<ExplicitMappingDbContext>(_fixture, ExplicitDatabaseName));

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

            AssertEquivalentMetadata(conventionContext, explicitContext);
        }
        finally
        {
            await conventionContext.Database.EnsureDeletedAsync(cancellationToken);
            await explicitContext.Database.EnsureDeletedAsync(cancellationToken);
        }
    }

    [Fact]
    public async Task NullableConventionPropertiesRoundTripSqlNull()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        await using ConventionMappingDbContext context = new(CreateConventionOptions(NullableDatabaseName));

        try
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
            await context.Database.EnsureCreatedAsync(cancellationToken);

            context.NullableRecords.Add(new NullableConventionRecord { Id = 1 });
            await context.SaveChangesAsync(cancellationToken);
            context.ChangeTracker.Clear();

            NullableConventionRecord actual = await context.NullableRecords
                .AsNoTracking()
                .SingleAsync(record => record.Id == 1, cancellationToken);

            Assert.All(
                NullablePropertyNames,
                propertyName => Assert.Null(typeof(NullableConventionRecord).GetProperty(propertyName)!.GetValue(actual)));
        }
        finally
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
        }
    }

    [Fact]
    public void ExplicitMappingAllowsConsumerFacetOverridesWithoutCreatingIndexes()
    {
        using OverrideMappingDbContext context = new(
            PostgreSqlDbContextOptionsFactory.Create<OverrideMappingDbContext>(_fixture, ExplicitDatabaseName));

        IEntityType entityType = context.Model.FindEntityType(typeof(OverrideRecord))!;
        IProperty emailProperty = entityType.FindProperty(nameof(OverrideRecord.Email))!;

        Assert.Equal("contact_email", emailProperty.GetColumnName());
        Assert.Equal("text", emailProperty.GetRelationalTypeMapping().StoreType);
        Assert.False(emailProperty.IsNullable);
        Assert.Empty(entityType.GetIndexes());
        Assert.Single(entityType.GetKeys());
    }

    private DbContextOptions<ConventionMappingDbContext> CreateConventionOptions(string databaseName) =>
        PostgreSqlDbContextOptionsFactory.Create<ConventionMappingDbContext>(_fixture, databaseName);

    private static void AssertEquivalentMetadata(
        ConventionMappingDbContext conventionContext,
        ExplicitMappingDbContext explicitContext)
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
}
