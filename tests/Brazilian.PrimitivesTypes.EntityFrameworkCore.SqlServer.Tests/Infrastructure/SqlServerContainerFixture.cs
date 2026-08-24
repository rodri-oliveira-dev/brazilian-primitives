using System.Data.Common;
using Testcontainers.MsSql;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure;

public sealed class SqlServerContainerFixture : IAsyncLifetime
{
    private const string SqlServerImage = "mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04";

    private readonly MsSqlContainer _container = new MsSqlBuilder(SqlServerImage).Build();

    public string GetConnectionString(string databaseName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseName);

        DbConnectionStringBuilder connectionStringBuilder = new()
        {
            ConnectionString = _container.GetConnectionString(),
        };

        connectionStringBuilder.Remove("Database");
        connectionStringBuilder.Remove("Initial Catalog");
        connectionStringBuilder["Initial Catalog"] = databaseName;

        return connectionStringBuilder.ConnectionString;
    }

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync().ConfigureAwait(false);
    }

    public ValueTask DisposeAsync() => _container.DisposeAsync();
}
