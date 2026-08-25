using System.Data;
using Dapper;

namespace Brazilian.PrimitivesTypes.Dapper.SqlServer;

internal sealed class BrazilianPrimitiveTypeHandler<T> : SqlMapper.TypeHandler<T>
{
    private readonly Func<T, string> _serialize;
    private readonly Func<string, T> _parse;
    private readonly int _size;

    internal BrazilianPrimitiveTypeHandler(
        int size,
        Func<T, string> serialize,
        Func<string, T> parse)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(size);
        ArgumentNullException.ThrowIfNull(serialize);
        ArgumentNullException.ThrowIfNull(parse);

        _size = size;
        _serialize = serialize;
        _parse = parse;
    }

    public override void SetValue(IDbDataParameter parameter, T? value)
    {
        ArgumentNullException.ThrowIfNull(parameter);

        parameter.DbType = DbType.AnsiString;
        parameter.Size = _size;
        parameter.Value = value is null ? DBNull.Value : _serialize(value);
    }

    public override T? Parse(object value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value is not string text)
        {
            throw new DataException(
                $"Cannot materialize {typeof(T).Name} from database value of type {value.GetType().FullName}.");
        }

        return _parse(text);
    }
}
