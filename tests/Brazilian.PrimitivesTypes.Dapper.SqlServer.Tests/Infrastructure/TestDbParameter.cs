using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Brazilian.PrimitivesTypes.Dapper.SqlServer.Tests.Infrastructure;

internal sealed class TestDbParameter : IDbDataParameter
{
    public DbType DbType
    {
        get;
        set;
    }

    public ParameterDirection Direction
    {
        get;
        set;
    } = ParameterDirection.Input;

    public bool IsNullable => true;

    [AllowNull]
    public string ParameterName
    {
        get;
        set;
    } = string.Empty;

    [AllowNull]
    public string SourceColumn
    {
        get;
        set;
    } = string.Empty;

    public DataRowVersion SourceVersion
    {
        get;
        set;
    } = DataRowVersion.Current;

    public object? Value
    {
        get;
        set;
    } = DBNull.Value;

    public byte Precision
    {
        get;
        set;
    }

    public byte Scale
    {
        get;
        set;
    }

    public int Size
    {
        get;
        set;
    }
}
