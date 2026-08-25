namespace Brazilian.PrimitivesTypes.Dapper.SqlServer;

internal sealed class BrazilianPrimitiveSqlServerMapping<T>
{
    internal BrazilianPrimitiveSqlServerMapping(
        int size,
        Func<T, string> serialize,
        Func<string, T> parse)
    {
        Size = size;
        Handler = new BrazilianPrimitiveTypeHandler<T>(size, serialize, parse);
    }

    internal int Size
    {
        get;
    }

    internal BrazilianPrimitiveTypeHandler<T> Handler
    {
        get;
    }
}
