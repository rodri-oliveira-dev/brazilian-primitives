using System.Globalization;
using Brazilian.PrimitivesTypes;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Contexts;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.EndToEnd;

[Collection(PostgreSqlTestCollection.Name)]
public sealed class CustomerPostgreSqlEndToEndTests
{
    private const string DatabaseName = "brazilian_primitives_customer_e2e";
    private readonly PostgreSqlContainerFixture _fixture;

    public CustomerPostgreSqlEndToEndTests(PostgreSqlContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task CustomerInsertReadUpdateAndPrimitiveQueryUseCanonicalPostgreSqlValues()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        await using CustomerPostgreSqlDbContext context = new(CreateOptions());

        try
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
            await context.Database.EnsureCreatedAsync(cancellationToken);

            Customer withEmail = new()
            {
                Id = 1,
                Cpf = Cpf.Parse("529.982.247-25", CultureInfo.InvariantCulture),
                Email = Email.Parse("User@Domínio.com", CultureInfo.InvariantCulture),
                Cep = Cep.Parse("01311-000", CultureInfo.InvariantCulture),
            };
            Customer withoutEmail = new()
            {
                Id = 2,
                Cpf = Cpf.Parse("11900000083", CultureInfo.InvariantCulture),
                Email = null,
                Cep = Cep.Parse("01001-000", CultureInfo.InvariantCulture),
            };

            context.Customers.AddRange(withEmail, withoutEmail);
            await context.SaveChangesAsync(cancellationToken);
            context.ChangeTracker.Clear();

            Cpf queryCpf = Cpf.Parse("52998224725", CultureInfo.InvariantCulture);
            Customer loaded = await context.Customers
                .AsNoTracking()
                .SingleAsync(customer => customer.Cpf == queryCpf, cancellationToken);
            Customer loadedWithoutEmail = await context.Customers
                .AsNoTracking()
                .SingleAsync(customer => customer.Id == 2, cancellationToken);

            Assert.Equal(withEmail.Cpf, loaded.Cpf);
            Assert.Equal(withEmail.Email, loaded.Email);
            Assert.Equal(withEmail.Cep, loaded.Cep);
            Assert.Null(loadedWithoutEmail.Email);

            string rawCpf = await context.Database
                .SqlQueryRaw<string>(
                    "SELECT cpf AS \"Value\" FROM integration.customers WHERE id = 1")
                .SingleAsync(cancellationToken);
            string rawEmail = await context.Database
                .SqlQueryRaw<string>(
                    "SELECT email AS \"Value\" FROM integration.customers WHERE id = 1")
                .SingleAsync(cancellationToken);
            string rawCep = await context.Database
                .SqlQueryRaw<string>(
                    "SELECT cep AS \"Value\" FROM integration.customers WHERE id = 1")
                .SingleAsync(cancellationToken);
            string? nullEmail = await context.Database
                .SqlQueryRaw<string?>(
                    "SELECT email AS \"Value\" FROM integration.customers WHERE id = 2")
                .SingleAsync(cancellationToken);

            Assert.Equal("52998224725", rawCpf);
            Assert.Equal("User@xn--domnio-5va.com", rawEmail);
            Assert.Equal("01311000", rawCep);
            Assert.Null(nullEmail);

            Customer tracked = await context.Customers.SingleAsync(customer => customer.Id == 1, cancellationToken);
            tracked.Email = Email.Parse("updated@DOMÍNIO.com", CultureInfo.InvariantCulture);
            tracked.Cep = Cep.Parse("01001-000", CultureInfo.InvariantCulture);
            await context.SaveChangesAsync(cancellationToken);
            context.ChangeTracker.Clear();

            Customer updated = await context.Customers.AsNoTracking()
                .SingleAsync(customer => customer.Id == 1, cancellationToken);

            Assert.Equal(Email.Parse("updated@domínio.com", CultureInfo.InvariantCulture), updated.Email);
            Assert.Equal(Cep.Parse("01001000", CultureInfo.InvariantCulture), updated.Cep);
            string updatedRawEmail = await context.Database
                .SqlQueryRaw<string>(
                    "SELECT email AS \"Value\" FROM integration.customers WHERE id = 1")
                .SingleAsync(cancellationToken);
            string updatedRawCep = await context.Database
                .SqlQueryRaw<string>(
                    "SELECT cep AS \"Value\" FROM integration.customers WHERE id = 1")
                .SingleAsync(cancellationToken);

            Assert.Equal("updated@xn--domnio-5va.com", updatedRawEmail);
            Assert.Equal("01001000", updatedRawCep);

            AssertSchemaMetadata(context);
        }
        finally
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
        }
    }

    [Fact]
    public async Task InvalidNonNullCustomerEmailFailsDuringMaterialization()
    {
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        await using CustomerPostgreSqlDbContext context = new(CreateOptions());

        try
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
            await context.Database.EnsureCreatedAsync(cancellationToken);

            await context.Database.ExecuteSqlRawAsync(
                """
                INSERT INTO integration.customers (id, cpf, email, cep)
                VALUES (7, '52998224725', 'not-an-email', '01311000')
                """,
                cancellationToken);

            await Assert.ThrowsAsync<FormatException>(
                () => context.Customers.AsNoTracking()
                    .SingleAsync(customer => customer.Id == 7, cancellationToken));
        }
        finally
        {
            await context.Database.EnsureDeletedAsync(cancellationToken);
        }
    }

    private DbContextOptions<CustomerPostgreSqlDbContext> CreateOptions() =>
        PostgreSqlDbContextOptionsFactory.Create<CustomerPostgreSqlDbContext>(_fixture, DatabaseName);

    private static void AssertSchemaMetadata(CustomerPostgreSqlDbContext context)
    {
        IEntityType customer = context.Model.FindEntityType(typeof(Customer))!;
        IProperty cpf = customer.FindProperty(nameof(Customer.Cpf))!;
        IProperty email = customer.FindProperty(nameof(Customer.Email))!;
        IProperty cep = customer.FindProperty(nameof(Customer.Cep))!;

        Assert.False(cpf.IsNullable);
        Assert.Equal("character varying(11)", cpf.GetRelationalTypeMapping().StoreType);
        Assert.True(email.IsNullable);
        Assert.Equal("character varying(254)", email.GetRelationalTypeMapping().StoreType);
        Assert.False(cep.IsNullable);
        Assert.Equal("character varying(8)", cep.GetRelationalTypeMapping().StoreType);
        Assert.Equal(4, customer.GetProperties().Count());
        Assert.Empty(customer.GetIndexes());
    }
}
