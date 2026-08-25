using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Brazilian.PrimitivesTypes.Dapper.SqlServer.Tests.Infrastructure;

internal sealed class TestDbCommand(TestDbConnection connection) : DbCommand
{
    private readonly TestDbParameterCollection parameters = new();
    private TestDbConnection? connection = connection;

    [AllowNull]
    public override string CommandText
    {
        get;
        set;
    } = string.Empty;

    public override int CommandTimeout
    {
        get;
        set;
    }

    public override CommandType CommandType
    {
        get;
        set;
    } = CommandType.Text;

    public override bool DesignTimeVisible
    {
        get;
        set;
    }

    public override UpdateRowSource UpdatedRowSource
    {
        get;
        set;
    }

    protected override DbConnection? DbConnection
    {
        get => connection;
        set => connection = (TestDbConnection?)value;
    }

    protected override DbParameterCollection DbParameterCollection => parameters;

    protected override DbTransaction? DbTransaction
    {
        get;
        set;
    }

    internal TestDbParameterCollection CapturedParameters => parameters;

    public override void Cancel()
    {
    }

    public override int ExecuteNonQuery()
    {
        connection?.Capture(this);
        return connection?.NonQueryResult ?? 0;
    }

    public override object? ExecuteScalar()
    {
        connection?.Capture(this);
        return connection?.ScalarResult;
    }

    public override void Prepare()
    {
    }

    protected override DbParameter CreateDbParameter() => new TestDbParameter();

    protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
    {
        connection?.Capture(this);
        return connection?.ReaderFactory?.Invoke() ?? new DataTable().CreateDataReader();
    }
}
