using System.Data.Common;
using Testcontainers.PostgreSql;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure;

public sealed class PostgreSqlContainerFixture : IAsyncLifetime
{
    private const string PostgreSqlImage = "postgres:18.0";

    private readonly PostgreSqlContainer _container = new PostgreSqlBuilder(PostgreSqlImage).Build();

    public string GetConnectionString(string databaseName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseName);

        DbConnectionStringBuilder connectionStringBuilder = new()
        {
            ConnectionString = _container.GetConnectionString(),
        };

        connectionStringBuilder.Remove("Database");
        connectionStringBuilder["Database"] = databaseName;

        return connectionStringBuilder.ConnectionString;
    }

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync().ConfigureAwait(false);
    }

    public ValueTask DisposeAsync() => _container.DisposeAsync();
}
