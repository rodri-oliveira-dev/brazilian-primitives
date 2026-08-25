using Microsoft.Data.SqlClient;
using Testcontainers.MsSql;
using Xunit;

namespace Brazilian.PrimitivesTypes.Dapper.SqlServer.Tests.Infrastructure;

public sealed class SqlServerContainerFixture : IAsyncLifetime
{
    private const string SqlServerImage = "mcr.microsoft.com/mssql/server:2022-CU14-ubuntu-22.04";

    private readonly MsSqlContainer _container = new MsSqlBuilder(SqlServerImage).Build();

    internal async Task<SqlServerDatabase> CreateDatabaseAsync(CancellationToken cancellationToken)
    {
        string databaseName = $"BrazilianPrimitivesDapper_{Guid.NewGuid():N}";

        await using SqlConnection connection = new(_container.GetConnectionString());
        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using SqlCommand command = connection.CreateCommand();
        command.CommandText = $"CREATE DATABASE [{databaseName}]";
        await command.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);

        return new SqlServerDatabase(_container.GetConnectionString(), databaseName);
    }

    public async ValueTask InitializeAsync()
    {
        await _container.StartAsync().ConfigureAwait(false);
    }

    public ValueTask DisposeAsync() => _container.DisposeAsync();
}

internal sealed class SqlServerDatabase : IAsyncDisposable
{
    private readonly string _masterConnectionString;
    private readonly string _databaseName;

    internal SqlServerDatabase(string masterConnectionString, string databaseName)
    {
        _masterConnectionString = masterConnectionString;
        _databaseName = databaseName;

        SqlConnectionStringBuilder builder = new(masterConnectionString)
        {
            InitialCatalog = databaseName,
        };
        ConnectionString = builder.ConnectionString;
    }

    internal string ConnectionString { get; }

    public async ValueTask DisposeAsync()
    {
        await using SqlConnection connection = new(_masterConnectionString);
        await connection.OpenAsync().ConfigureAwait(false);
        await using SqlCommand command = connection.CreateCommand();
        command.CommandText = $"ALTER DATABASE [{_databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE [{_databaseName}];";
        await command.ExecuteNonQueryAsync().ConfigureAwait(false);
    }
}
