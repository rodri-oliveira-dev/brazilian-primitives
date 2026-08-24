using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Contexts;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Mappings;

[Collection(SqlServerTestCollection.Name)]
public sealed class ConventionMappingSqlServerTests
{
    private const string DatabaseName = "BrazilianPrimitivesConventionTests";
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

    private readonly SqlServerContainerFixture _fixture;

    public ConventionMappingSqlServerTests(SqlServerContainerFixture fixture)
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

    public static TheoryData<string, Type, string> NullableConventionMappings => new()
    {
        { nameof(NullableConventionRecord.Cpf), typeof(CpfValueConverter), "varchar(11)" },
        { nameof(NullableConventionRecord.Cnpj), typeof(CnpjValueConverter), "varchar(14)" },
        { nameof(NullableConventionRecord.CpfCnpj), typeof(CpfCnpjValueConverter), "varchar(14)" },
        { nameof(NullableConventionRecord.Cep), typeof(CepValueConverter), "varchar(8)" },
        { nameof(NullableConventionRecord.Email), typeof(EmailValueConverter), "varchar(254)" },
        { nameof(NullableConventionRecord.MobilePhone), typeof(MobilePhoneValueConverter), "varchar(11)" },
        { nameof(NullableConventionRecord.LandlinePhone), typeof(LandlinePhoneValueConverter), "varchar(10)" },
        { nameof(NullableConventionRecord.TelefoneBrasileiro), typeof(TelefoneBrasileiroValueConverter), "varchar(11)" },
        { nameof(NullableConventionRecord.ChavePix), typeof(ChavePixValueConverter), "varchar(77)" },
        { nameof(NullableConventionRecord.Cnh), typeof(CnhValueConverter), "varchar(11)" },
        { nameof(NullableConventionRecord.Cns), typeof(CnsValueConverter), "varchar(15)" },
        { nameof(NullableConventionRecord.TituloEleitoral), typeof(TituloEleitoralValueConverter), "varchar(12)" },
        { nameof(NullableConventionRecord.Nit), typeof(NitValueConverter), "varchar(11)" },
        { nameof(NullableConventionRecord.PisPasep), typeof(PisPasepValueConverter), "varchar(11)" },
        { nameof(NullableConventionRecord.PlacaVeiculo), typeof(PlacaVeiculoValueConverter), "varchar(7)" },
        { nameof(NullableConventionRecord.Renavam), typeof(RenavamValueConverter), "varchar(11)" },
        { nameof(NullableConventionRecord.Ispb), typeof(IspbValueConverter), "varchar(8)" },
        { nameof(NullableConventionRecord.CodigoCompe), typeof(CodigoCompeValueConverter), "varchar(3)" },
    };

    [Theory]
    [MemberData(nameof(ConventionMappings))]
    public void ModelWideConventionRegistersExpectedScalarConverterAndSqlServerFacets(
        string propertyName,
        Type converterType,
        string storeType,
        bool isNullable)
    {
        using ConventionDbContext context = new(CreateOptions());

        IProperty property = context.Model.FindEntityType(typeof(AllConventionRecord))!
            .FindProperty(propertyName)!;

        Assert.IsType(converterType, property.GetValueConverter());
        Assert.Equal(storeType, property.GetRelationalTypeMapping().StoreType);
        Assert.Equal(isNullable, property.IsNullable);
    }

    [Fact]
    public void ModelWidePrimitiveConventionLeavesStateRegistrationsExplicit()
    {
        using PrimitiveConventionOnlyStateRegistrationDbContext context = new(
            SqlServerDbContextOptionsFactory.Create<PrimitiveConventionOnlyStateRegistrationDbContext>(
                _fixture,
                DatabaseName));

        InvalidOperationException exception = Assert.Throws<InvalidOperationException>(() => _ = context.Model);

        Assert.True(
            exception.Message.Contains(nameof(ContextFreeStateRecord.Rg), StringComparison.Ordinal) ||
            exception.Message.Contains(nameof(ContextFreeStateRecord.InscricaoEstadual), StringComparison.Ordinal),
            exception.Message);
    }

    [Theory]
    [MemberData(nameof(NullableConventionMappings))]
    public void ModelWideConventionRegistersNullableScalarPrimitives(
        string propertyName,
        Type converterType,
        string storeType)
    {
        using ConventionDbContext context = new(CreateOptions());

        IProperty property = context.Model.FindEntityType(typeof(NullableConventionRecord))!
            .FindProperty(propertyName)!;

        Assert.IsType(converterType, property.GetValueConverter());
        Assert.Equal(storeType, property.GetRelationalTypeMapping().StoreType);
        Assert.True(property.IsNullable);
    }

    [Fact]
    public async Task NullableScalarConventionPropertiesRoundTripSqlNulls()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        await using ConventionDbContext context = new(CreateOptions());

        try
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
            await context.Database.EnsureCreatedAsync(cancellationToken);

            context.NullableConventionRecords.Add(new NullableConventionRecord { Id = 1 });
            await context.SaveChangesAsync(cancellationToken);
            context.ChangeTracker.Clear();

            NullableConventionRecord actual = await context.NullableConventionRecords
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

    private Microsoft.EntityFrameworkCore.DbContextOptions<ConventionDbContext> CreateOptions() =>
        SqlServerDbContextOptionsFactory.Create<ConventionDbContext>(_fixture, DatabaseName);
}
