using System.Data.Common;
using System.Globalization;
using Brazilian.PrimitivesTypes;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests;

[Collection(SqlServerTestCollection.Name)]
public sealed class CustomerSqlServerEndToEndTests
{
    private const string DatabaseName = "BrazilianPrimitivesCustomerEndToEndTests";
    private readonly SqlServerContainerFixture _fixture;

    public CustomerSqlServerEndToEndTests(SqlServerContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CustomerInsertReadUpdateAndQueryRoundTripUsesCanonicalSqlServerStorage()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        DbContextOptions<CustomerDbContext> options = new DbContextOptionsBuilder<CustomerDbContext>()
            .UseSqlServer(_fixture.GetConnectionString(DatabaseName))
            .Options;
        await using CustomerDbContext context = new(options);

        try
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
            await context.Database.EnsureCreatedAsync(cancellationToken);

            Cpf cpf = Cpf.Parse("529.982.247-25", CultureInfo.InvariantCulture);
            Email email = Email.Parse("USER@Example.COM", CultureInfo.InvariantCulture);
            Cep cep = Cep.Parse("01311-000", CultureInfo.InvariantCulture);

            context.Customers.Add(new Customer
            {
                Id = 1,
                Cpf = cpf,
                Email = email,
                Cep = cep,
            });
            await context.SaveChangesAsync(cancellationToken);

            StoredCustomer inserted = await ReadStoredCustomerAsync(context, 1, cancellationToken);
            Assert.Equal("52998224725", inserted.Cpf);
            Assert.Equal(email.Value, inserted.Email);
            Assert.Equal("01311000", inserted.Cep);

            context.ChangeTracker.Clear();

            Customer loaded = await context.Customers
                .SingleAsync(customer => customer.Cpf == cpf, cancellationToken);

            Assert.Equal(cpf, loaded.Cpf);
            Assert.Equal(email, loaded.Email);
            Assert.Equal(cep, loaded.Cep);

            Cep updatedCep = Cep.Parse("04567-890", CultureInfo.InvariantCulture);
            loaded.Cep = updatedCep;
            loaded.Email = null;
            await context.SaveChangesAsync(cancellationToken);
            context.ChangeTracker.Clear();

            StoredCustomer updated = await ReadStoredCustomerAsync(context, 1, cancellationToken);
            Assert.Equal("52998224725", updated.Cpf);
            Assert.Null(updated.Email);
            Assert.Equal("04567890", updated.Cep);

            Customer materialized = await context.Customers
                .AsNoTracking()
                .SingleAsync(customer => customer.Cpf == cpf, cancellationToken);

            Assert.Equal(cpf, materialized.Cpf);
            Assert.Null(materialized.Email);
            Assert.Equal(updatedCep, materialized.Cep);

            AssertSchemaMetadata(context);
        }
        finally
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
        }
    }

    private static async Task<StoredCustomer> ReadStoredCustomerAsync(
        CustomerDbContext context,
        long id,
        CancellationToken cancellationToken)
    {
        await context.Database.OpenConnectionAsync(cancellationToken);

        try
        {
            await using DbCommand command = context.Database.GetDbConnection().CreateCommand();
            command.CommandText = "SELECT [Cpf], [Email], [Cep] FROM [Customers] WHERE [Id] = @id";

            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = "@id";
            parameter.Value = id;
            command.Parameters.Add(parameter);

            await using DbDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
            Assert.True(await reader.ReadAsync(cancellationToken));

            return new StoredCustomer(
                reader.GetString(0),
                reader.IsDBNull(1) ? null : reader.GetString(1),
                reader.GetString(2));
        }
        finally
        {
            await context.Database.CloseConnectionAsync();
        }
    }

    private static void AssertSchemaMetadata(CustomerDbContext context)
    {
        IEntityType entityType = context.Model.FindEntityType(typeof(Customer))!;
        IProperty cpf = entityType.FindProperty(nameof(Customer.Cpf))!;
        IProperty email = entityType.FindProperty(nameof(Customer.Email))!;
        IProperty cep = entityType.FindProperty(nameof(Customer.Cep))!;

        Assert.False(cpf.IsNullable);
        Assert.Equal("varchar(11)", cpf.GetRelationalTypeMapping().StoreType);
        Assert.True(email.IsNullable);
        Assert.Equal("varchar(254)", email.GetRelationalTypeMapping().StoreType);
        Assert.False(cep.IsNullable);
        Assert.Equal("varchar(8)", cep.GetRelationalTypeMapping().StoreType);
        Assert.Empty(entityType.GetIndexes());
    }

    private sealed class CustomerDbContext(DbContextOptions<CustomerDbContext> options) : DbContext(options)
    {
        public DbSet<Customer> Customers => Set<Customer>();

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.UseBrazilianPrimitiveTypesSqlServer();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("Customers");
                entity.HasKey(customer => customer.Id);
                entity.Property(customer => customer.Id)
                    .ValueGeneratedNever();
            });
        }
    }

    private sealed class Customer
    {
        public long Id
        {
            get;
            set;
        }

        public Cpf Cpf
        {
            get;
            set;
        }

        public Email? Email
        {
            get;
            set;
        }

        public Cep Cep
        {
            get;
            set;
        }
    }

    private sealed record StoredCustomer(string Cpf, string? Email, string Cep);
}
