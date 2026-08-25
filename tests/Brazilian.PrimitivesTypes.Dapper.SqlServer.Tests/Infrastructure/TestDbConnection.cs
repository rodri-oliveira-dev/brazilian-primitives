using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Brazilian.PrimitivesTypes.Dapper.SqlServer.Tests.Infrastructure;

internal sealed class TestDbConnection : DbConnection
{
    private ConnectionState state = ConnectionState.Closed;

    [AllowNull]
    public override string ConnectionString
    {
        get;
        set;
    } = string.Empty;

    public override string Database => "TestDatabase";

    public override string DataSource => "TestDataSource";

    public override string ServerVersion => "1.0";

    public override ConnectionState State => state;

    internal Func<DbDataReader>? ReaderFactory
    {
        get;
        set;
    }

    internal object? ScalarResult
    {
        get;
        set;
    }

    internal int NonQueryResult
    {
        get;
        set;
    } = 1;

    internal TestDbCommand? LastCommand
    {
        get;
        private set;
    }

    public override void ChangeDatabase(string databaseName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseName);
    }

    public override void Close() => state = ConnectionState.Closed;

    public override void Open() => state = ConnectionState.Open;

    protected override DbTransaction BeginDbTransaction(IsolationLevel isolationLevel) =>
        throw new NotSupportedException("Transactions are not required by these tests.");

    protected override DbCommand CreateDbCommand()
    {
        LastCommand = new TestDbCommand(this);
        return LastCommand;
    }
}
