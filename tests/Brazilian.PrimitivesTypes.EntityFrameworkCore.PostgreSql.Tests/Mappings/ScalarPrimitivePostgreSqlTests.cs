using System.Globalization;
using Brazilian.PrimitivesTypes;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Contexts;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Mappings;

[Collection(PostgreSqlTestCollection.Name)]
public sealed class ScalarPrimitivePostgreSqlTests
{
    private const string RoundTripDatabaseName = "brazilian_primitives_scalar_roundtrip";
    private const string NullableDatabaseName = "brazilian_primitives_nullable";
    private const string InvalidDatabaseName = "brazilian_primitives_invalid_persistence";

    private readonly PostgreSqlContainerFixture _fixture;

    public ScalarPrimitivePostgreSqlTests(PostgreSqlContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ScalarPrimitivesRoundTripThroughPostgreSqlAndRemainQueryableByStrongType()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        await using ScalarPrimitiveDbContext context = new(CreateOptions(RoundTripDatabaseName));

        try
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
            await context.Database.EnsureCreatedAsync(cancellationToken);

            ScalarPrimitiveRecord expected = CreateRecord(1, optionalEmail: null);
            context.Records.Add(expected);
            await context.SaveChangesAsync(cancellationToken);
            context.ChangeTracker.Clear();

            ScalarPrimitiveRecord actual = await context.Records
                .AsNoTracking()
                .SingleAsync(record => record.Cpf == expected.Cpf, cancellationToken);

            AssertScalarRoundTrip(expected, actual);
            AssertPostgreSqlStoreType(context, nameof(ScalarPrimitiveRecord.Cpf), "character varying(11)");
            AssertPostgreSqlStoreType(context, nameof(ScalarPrimitiveRecord.Cnpj), "character varying(14)");
            AssertPostgreSqlStoreType(context, nameof(ScalarPrimitiveRecord.Email), "character varying(254)");
            AssertPostgreSqlStoreType(context, nameof(ScalarPrimitiveRecord.ChavePix), "character varying(77)");
            AssertPostgreSqlStoreType(context, nameof(ScalarPrimitiveRecord.CodigoCompe), "character varying(3)");
        }
        finally
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
        }
    }

    [Fact]
    public async Task NullableEmailRoundTripsSqlNullAndCanonicalNonNullValue()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        await using ScalarPrimitiveDbContext context = new(CreateOptions(NullableDatabaseName));

        try
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
            await context.Database.EnsureCreatedAsync(cancellationToken);

            Email expectedEmail = Email.Parse("USER@Example.COM", CultureInfo.InvariantCulture);
            context.EmailRecords.AddRange(
                new EmailRecord { Id = 1, Email = null },
                new EmailRecord { Id = 2, Email = expectedEmail });
            await context.SaveChangesAsync(cancellationToken);
            context.ChangeTracker.Clear();

            EmailRecord nullRecord = await context.EmailRecords.AsNoTracking().SingleAsync(record => record.Id == 1, cancellationToken);
            EmailRecord valueRecord = await context.EmailRecords.AsNoTracking().SingleAsync(record => record.Id == 2, cancellationToken);

            Assert.Null(nullRecord.Email);
            Assert.Equal(expectedEmail, valueRecord.Email);

            IProperty emailProperty = context.Model.FindEntityType(typeof(EmailRecord))!
                .FindProperty(nameof(EmailRecord.Email))!;
            Assert.True(emailProperty.IsNullable);
            Assert.Equal("character varying(254)", emailProperty.GetRelationalTypeMapping().StoreType);
        }
        finally
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
        }
    }

    [Fact]
    public async Task InvalidNonNullPersistedEmailFailsDuringMaterialization()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        await using ScalarPrimitiveDbContext context = new(CreateOptions(InvalidDatabaseName));

        try
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
            await context.Database.EnsureCreatedAsync(cancellationToken);
            await context.Database.ExecuteSqlRawAsync(
                "INSERT INTO integration.email_records (id, email) VALUES (1, 'not-an-email')",
                cancellationToken);
            context.ChangeTracker.Clear();

            await Assert.ThrowsAsync<FormatException>(
                () => context.EmailRecords.AsNoTracking().SingleAsync(record => record.Id == 1, cancellationToken));
        }
        finally
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
        }
    }

    private DbContextOptions<ScalarPrimitiveDbContext> CreateOptions(string databaseName) =>
        PostgreSqlDbContextOptionsFactory.Create<ScalarPrimitiveDbContext>(_fixture, databaseName);

    private static ScalarPrimitiveRecord CreateRecord(int id, Email? optionalEmail) =>
        new()
        {
            Id = id,
            Cpf = Cpf.Parse("529.982.247-25", CultureInfo.InvariantCulture),
            Cnpj = Cnpj.Parse("00000000e08g12", CultureInfo.InvariantCulture),
            CpfCnpj = CpfCnpj.Parse("00.000.000/e08g-12", CultureInfo.InvariantCulture),
            Cep = Cep.Parse("01311-000", CultureInfo.InvariantCulture),
            Email = Email.Parse("usuario@domínio.com", CultureInfo.InvariantCulture),
            OptionalEmail = optionalEmail,
            MobilePhone = MobilePhone.Parse("(11) 98765-4321", CultureInfo.InvariantCulture),
            LandlinePhone = LandlinePhone.Parse("(11) 3234-5678", CultureInfo.InvariantCulture),
            TelefoneBrasileiro = TelefoneBrasileiro.Parse("(11) 98765-4321", CultureInfo.InvariantCulture),
            ChavePix = ChavePix.From(Cpf.Parse("11900000083", CultureInfo.InvariantCulture)),
            Cnh = Cnh.Parse("02650306461", CultureInfo.InvariantCulture),
            Cns = Cns.Parse("123456789010000", CultureInfo.InvariantCulture),
            TituloEleitoral = TituloEleitoral.Parse("000123450159", CultureInfo.InvariantCulture),
            Nit = Nit.Parse("00000000001", CultureInfo.InvariantCulture),
            PisPasep = PisPasep.Parse("01234567897", CultureInfo.InvariantCulture),
            PlacaVeiculo = PlacaVeiculo.Parse("abc1d23", CultureInfo.InvariantCulture),
            Renavam = Renavam.Parse("00123456789", CultureInfo.InvariantCulture),
            Ispb = Ispb.Parse("00000001", CultureInfo.InvariantCulture),
            CodigoCompe = CodigoCompe.Parse("001", CultureInfo.InvariantCulture),
        };

    private static void AssertScalarRoundTrip(ScalarPrimitiveRecord expected, ScalarPrimitiveRecord actual)
    {
        Assert.Equal(expected.Cpf, actual.Cpf);
        Assert.Equal(expected.Cnpj, actual.Cnpj);
        Assert.Equal(expected.CpfCnpj, actual.CpfCnpj);
        Assert.Equal(TipoCpfCnpj.Cnpj, actual.CpfCnpj.Tipo);
        Assert.Equal(expected.Cep, actual.Cep);
        Assert.Equal(expected.Email, actual.Email);
        Assert.Null(actual.OptionalEmail);
        Assert.Equal(expected.MobilePhone, actual.MobilePhone);
        Assert.Equal(expected.LandlinePhone, actual.LandlinePhone);
        Assert.Equal(expected.TelefoneBrasileiro, actual.TelefoneBrasileiro);
        Assert.Equal(TipoTelefoneBrasileiro.Celular, actual.TelefoneBrasileiro.Tipo);
        Assert.Equal(expected.ChavePix, actual.ChavePix);
        Assert.Equal(TipoChavePix.Cpf, actual.ChavePix.Tipo);
        Assert.Equal(expected.Cnh, actual.Cnh);
        Assert.Equal(expected.Cns, actual.Cns);
        Assert.Equal(expected.TituloEleitoral, actual.TituloEleitoral);
        Assert.Equal(expected.Nit, actual.Nit);
        Assert.Equal(expected.PisPasep, actual.PisPasep);
        Assert.Equal(expected.PlacaVeiculo, actual.PlacaVeiculo);
        Assert.Equal(PadraoPlacaVeiculo.Mercosul, actual.PlacaVeiculo.Padrao);
        Assert.Equal(expected.Renavam, actual.Renavam);
        Assert.Equal(expected.Ispb, actual.Ispb);
        Assert.Equal(expected.CodigoCompe, actual.CodigoCompe);
    }

    private static void AssertPostgreSqlStoreType(
        ScalarPrimitiveDbContext context,
        string propertyName,
        string expectedStoreType)
    {
        IProperty property = context.Model.FindEntityType(typeof(ScalarPrimitiveRecord))!
            .FindProperty(propertyName)!;

        Assert.Equal(expectedStoreType, property.GetRelationalTypeMapping().StoreType);
    }
}
