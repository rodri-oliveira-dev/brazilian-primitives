using System.Globalization;
using Brazilian.PrimitivesTypes;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests;

[Collection(SqlServerTestCollection.Name)]
public sealed class ScalarPrimitiveSqlServerTests
{
    private const string RoundTripDatabaseName = "BrazilianPrimitivesScalarRoundTripTests";
    private const string NullableDatabaseName = "BrazilianPrimitivesNullableTests";
    private const string InvalidDatabaseName = "BrazilianPrimitivesInvalidPersistenceTests";

    private readonly SqlServerContainerFixture _fixture;

    public ScalarPrimitiveSqlServerTests(SqlServerContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task ScalarPrimitivesRoundTripThroughSqlServerAndRemainQueryableByStrongType()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        DbContextOptions<ScalarPrimitiveDbContext> options = CreateOptions(RoundTripDatabaseName);
        await using ScalarPrimitiveDbContext context = new(options);

        try
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
            await context.Database.EnsureCreatedAsync(cancellationToken);

            ScalarPrimitiveRecord expected = CreateRecord(id: 1, optionalEmail: null);
            context.Records.Add(expected);
            await context.SaveChangesAsync(cancellationToken);
            context.ChangeTracker.Clear();

            ScalarPrimitiveRecord actual = await context.Records
                .AsNoTracking()
                .SingleAsync(record => record.Cpf == expected.Cpf, cancellationToken);

            Assert.Equal(expected.Cpf, actual.Cpf);
            Assert.Equal(expected.Cnpj, actual.Cnpj);
            Assert.Equal(expected.CpfCnpj, actual.CpfCnpj);
            Assert.Equal(expected.Cep, actual.Cep);
            Assert.Equal(expected.Email, actual.Email);
            Assert.Null(actual.OptionalEmail);
            Assert.Equal(expected.MobilePhone, actual.MobilePhone);
            Assert.Equal(expected.LandlinePhone, actual.LandlinePhone);
            Assert.Equal(expected.TelefoneBrasileiro, actual.TelefoneBrasileiro);
            Assert.Equal(expected.ChavePix, actual.ChavePix);
            Assert.Equal(TipoChavePix.Cpf, actual.ChavePix.Tipo);
            Assert.Equal(expected.Cnh, actual.Cnh);
            Assert.Equal(expected.Cns, actual.Cns);
            Assert.Equal(expected.TituloEleitoral, actual.TituloEleitoral);
            Assert.Equal(expected.Nit, actual.Nit);
            Assert.Equal(expected.PisPasep, actual.PisPasep);
            Assert.Equal(expected.PlacaVeiculo, actual.PlacaVeiculo);
            Assert.Equal(expected.Renavam, actual.Renavam);
            Assert.Equal(expected.Ispb, actual.Ispb);
            Assert.Equal(expected.CodigoCompe, actual.CodigoCompe);

            AssertSqlServerStoreType(context, nameof(ScalarPrimitiveRecord.Cpf), "varchar(11)");
            AssertSqlServerStoreType(context, nameof(ScalarPrimitiveRecord.Cnpj), "varchar(14)");
            AssertSqlServerStoreType(context, nameof(ScalarPrimitiveRecord.Email), "varchar(254)");
            AssertSqlServerStoreType(context, nameof(ScalarPrimitiveRecord.ChavePix), "varchar(77)");
            AssertSqlServerStoreType(context, nameof(ScalarPrimitiveRecord.CodigoCompe), "varchar(3)");
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
        DbContextOptions<ScalarPrimitiveDbContext> options = CreateOptions(NullableDatabaseName);
        await using ScalarPrimitiveDbContext context = new(options);

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
            Assert.Equal("varchar(254)", emailProperty.GetRelationalTypeMapping().StoreType);
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
        DbContextOptions<ScalarPrimitiveDbContext> options = CreateOptions(InvalidDatabaseName);
        await using ScalarPrimitiveDbContext context = new(options);

        try
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
            await context.Database.EnsureCreatedAsync(cancellationToken);
            await context.Database.ExecuteSqlRawAsync(
                "INSERT INTO [EmailRecords] ([Id], [Email]) VALUES (1, 'not-an-email')",
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

    private DbContextOptions<ScalarPrimitiveDbContext> CreateOptions(string databaseName)
    {
        return new DbContextOptionsBuilder<ScalarPrimitiveDbContext>()
            .UseSqlServer(_fixture.GetConnectionString(databaseName))
            .Options;
    }

    private static ScalarPrimitiveRecord CreateRecord(int id, Email? optionalEmail)
    {
        return new ScalarPrimitiveRecord
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
    }

    private static void AssertSqlServerStoreType(
        ScalarPrimitiveDbContext context,
        string propertyName,
        string expectedStoreType)
    {
        IProperty property = context.Model.FindEntityType(typeof(ScalarPrimitiveRecord))!
            .FindProperty(propertyName)!;

        Assert.Equal(expectedStoreType, property.GetRelationalTypeMapping().StoreType);
    }

    private sealed class ScalarPrimitiveDbContext(DbContextOptions<ScalarPrimitiveDbContext> options) : DbContext(options)
    {
        public DbSet<ScalarPrimitiveRecord> Records => Set<ScalarPrimitiveRecord>();

        public DbSet<EmailRecord> EmailRecords => Set<EmailRecord>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ScalarPrimitiveRecord>(entity =>
            {
                entity.ToTable("ScalarPrimitiveRecords");
                entity.HasKey(record => record.Id);
                entity.Property(record => record.Id)
                    .ValueGeneratedNever();
                entity.Property(record => record.Cpf)
                    .HasConversion(new CpfValueConverter());
                entity.Property(record => record.Cnpj)
                    .HasConversion(new CnpjValueConverter());
                entity.Property(record => record.CpfCnpj)
                    .HasConversion(new CpfCnpjValueConverter());
                entity.Property(record => record.Cep)
                    .HasConversion(new CepValueConverter());
                entity.Property(record => record.Email)
                    .HasConversion(new EmailValueConverter());
                entity.Property(record => record.OptionalEmail)
                    .HasConversion(new EmailValueConverter());
                entity.Property(record => record.MobilePhone)
                    .HasConversion(new MobilePhoneValueConverter());
                entity.Property(record => record.LandlinePhone)
                    .HasConversion(new LandlinePhoneValueConverter());
                entity.Property(record => record.TelefoneBrasileiro)
                    .HasConversion(new TelefoneBrasileiroValueConverter());
                entity.Property(record => record.ChavePix)
                    .HasConversion(new ChavePixValueConverter());
                entity.Property(record => record.Cnh)
                    .HasConversion(new CnhValueConverter());
                entity.Property(record => record.Cns)
                    .HasConversion(new CnsValueConverter());
                entity.Property(record => record.TituloEleitoral)
                    .HasConversion(new TituloEleitoralValueConverter());
                entity.Property(record => record.Nit)
                    .HasConversion(new NitValueConverter());
                entity.Property(record => record.PisPasep)
                    .HasConversion(new PisPasepValueConverter());
                entity.Property(record => record.PlacaVeiculo)
                    .HasConversion(new PlacaVeiculoValueConverter());
                entity.Property(record => record.Renavam)
                    .HasConversion(new RenavamValueConverter());
                entity.Property(record => record.Ispb)
                    .HasConversion(new IspbValueConverter());
                entity.Property(record => record.CodigoCompe)
                    .HasConversion(new CodigoCompeValueConverter());
            });

            modelBuilder.Entity<EmailRecord>(entity =>
            {
                entity.ToTable("EmailRecords");
                entity.HasKey(record => record.Id);
                entity.Property(record => record.Id)
                    .ValueGeneratedNever();
                entity.Property(record => record.Email)
                    .HasConversion(new EmailValueConverter());
            });
        }
    }

    private sealed class ScalarPrimitiveRecord
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

    private sealed class EmailRecord
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
}
