using System.Collections;
using System.Data.Common;

namespace Brazilian.PrimitivesTypes.Dapper.SqlServer.Tests.Infrastructure;

internal sealed class TestDbParameterCollection : DbParameterCollection
{
    private readonly List<DbParameter> parameters = [];

    public override int Count => parameters.Count;

    public override object SyncRoot => ((ICollection)parameters).SyncRoot;

    public override int Add(object value)
    {
        ArgumentNullException.ThrowIfNull(value);

        parameters.Add((DbParameter)value);
        return parameters.Count - 1;
    }

    public override void AddRange(Array values)
    {
        ArgumentNullException.ThrowIfNull(values);

        foreach (object value in values)
        {
            Add(value);
        }
    }

    public override void Clear() => parameters.Clear();

    public override bool Contains(object value) => value is DbParameter parameter && parameters.Contains(parameter);

    public override bool Contains(string value) => IndexOf(value) >= 0;

    public override void CopyTo(Array array, int index) => ((ICollection)parameters).CopyTo(array, index);

    public override IEnumerator GetEnumerator() => parameters.GetEnumerator();

    public override int IndexOf(object value) => value is DbParameter parameter ? parameters.IndexOf(parameter) : -1;

    public override int IndexOf(string parameterName) => parameters.FindIndex(
        parameter => string.Equals(parameter.ParameterName, parameterName, StringComparison.OrdinalIgnoreCase));

    public override void Insert(int index, object value)
    {
        ArgumentNullException.ThrowIfNull(value);
        parameters.Insert(index, (DbParameter)value);
    }

    public override void Remove(object value)
    {
        if (value is DbParameter parameter)
        {
            parameters.Remove(parameter);
        }
    }

    public override void RemoveAt(int index) => parameters.RemoveAt(index);

    public override void RemoveAt(string parameterName)
    {
        int index = IndexOf(parameterName);
        if (index >= 0)
        {
            RemoveAt(index);
        }
    }

    protected override DbParameter GetParameter(int index) => parameters[index];

    protected override DbParameter GetParameter(string parameterName)
    {
        int index = IndexOf(parameterName);
        return index >= 0
            ? parameters[index]
            : throw new KeyNotFoundException($"Parameter '{parameterName}' was not found.");
    }

    protected override void SetParameter(int index, DbParameter value)
    {
        ArgumentNullException.ThrowIfNull(value);
        parameters[index] = value;
    }

    protected override void SetParameter(string parameterName, DbParameter value)
    {
        ArgumentNullException.ThrowIfNull(value);

        int index = IndexOf(parameterName);
        if (index >= 0)
        {
            parameters[index] = value;
        }
        else
        {
            parameters.Add(value);
        }
    }
}
