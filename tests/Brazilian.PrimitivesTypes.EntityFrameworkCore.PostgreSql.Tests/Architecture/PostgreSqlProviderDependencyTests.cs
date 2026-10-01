using System.Reflection;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Architecture;

public sealed class PostgreSqlProviderDependencyTests
{
    [Fact]
    public void PostgreSqlProviderDoesNotReferenceSqlServerOrDapperIntegrations()
    {
        Assembly assembly = typeof(CpfValueConverter).Assembly;

        string[] references = assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty)
            .ToArray();

        Assert.DoesNotContain(
            references,
            reference => reference.StartsWith("Microsoft.Data.SqlClient", StringComparison.Ordinal));
        Assert.DoesNotContain(
            references,
            reference => reference.StartsWith(
                "Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer",
                StringComparison.Ordinal));
        Assert.DoesNotContain(
            references,
            reference => reference.StartsWith("Dapper", StringComparison.Ordinal));
    }
}
