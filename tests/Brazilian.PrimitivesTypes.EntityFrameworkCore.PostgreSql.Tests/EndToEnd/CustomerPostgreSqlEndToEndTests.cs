using System.Data;
using System.Data.Common;
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

            Assert.Equal("52998224725", await ReadScalarAsync(context, "cpf", 1, cancellationToken));
            Assert.Equal("User@xn--domnio-5va.com", await ReadScalarAsync(context, "email", 1, cancellationToken));
            Assert.Equal("01311000", await ReadScalarAsync(context, "cep", 1, cancellationToken));
            Assert.Null(await ReadScalarAsync(context, "email", 2, cancellationToken));

            Customer tracked = await context.Customers.SingleAsync(customer => customer.Id == 1, cancellationToken);
            tracked.Email = Email.Parse("updated@DOMÍNIO.com", CultureInfo.InvariantCulture);
            tracked.Cep = Cep.Parse("01001-000", CultureInfo.InvariantCulture);
            await context.SaveChangesAsync(cancellationToken);
            context.ChangeTracker.Clear();

            Customer updated = await context.Customers.AsNoTracking()
                .SingleAsync(customer => customer.Id == 1, cancellationToken);

            Assert.Equal(Email.Parse("updated@domínio.com", CultureInfo.InvariantCulture), updated.Email);
            Assert.Equal(Cep.Parse("01001000", CultureInfo.InvariantCulture), updated.Cep);
            Assert.Equal("updated@xn--domnio-5va.com", await ReadScalarAsync(context, "email", 1, cancellationToken));
            Assert.Equal("01001000", await ReadScalarAsync(context, "cep", 1, cancellationToken));

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
        Assert.Empty(customer.GetIndexes());
    }

    private static async Task<string?> ReadScalarAsync(
        CustomerPostgreSqlDbContext context,
        string columnName,
        long id,
        CancellationToken cancellationToken)
    {
        DbConnection connection = context.Database.GetDbConnection();
        bool openedHere = connection.State != ConnectionState.Open;

        if (openedHere)
        {
            await connection.OpenAsync(cancellationToken);
        }

        try
        {
            await using DbCommand command = connection.CreateCommand();
            command.CommandText = $"SELECT {columnName} FROM integration.customers WHERE id = @id";

            DbParameter parameter = command.CreateParameter();
            parameter.ParameterName = "id";
            parameter.Value = id;
            command.Parameters.Add(parameter);

            object? result = await command.ExecuteScalarAsync(cancellationToken);
            return result is null or DBNull ? null : (string)result;
        }
        finally
        {
            if (openedHere)
            {
                await connection.CloseAsync();
            }
        }
    }
}
