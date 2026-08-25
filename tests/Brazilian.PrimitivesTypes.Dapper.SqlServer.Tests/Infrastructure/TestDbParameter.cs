using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;

namespace Brazilian.PrimitivesTypes.Dapper.SqlServer.Tests.Infrastructure;

internal sealed class TestDbParameter : DbParameter
{
    public override DbType DbType
    {
        get;
        set;
    }

    public override ParameterDirection Direction
    {
        get;
        set;
    } = ParameterDirection.Input;

    public override bool IsNullable
    {
        get;
        set;
    } = true;

    [AllowNull]
    public override string ParameterName
    {
        get;
        set;
    } = string.Empty;

    [AllowNull]
    public override string SourceColumn
    {
        get;
        set;
    } = string.Empty;

    public override object? Value
    {
        get;
        set;
    } = DBNull.Value;

    public override bool SourceColumnNullMapping
    {
        get;
        set;
    }

    public override int Size
    {
        get;
        set;
    }

    public override byte Precision
    {
        get;
        set;
    }

    public override byte Scale
    {
        get;
        set;
    }

    public override void ResetDbType() => DbType = default;
}
