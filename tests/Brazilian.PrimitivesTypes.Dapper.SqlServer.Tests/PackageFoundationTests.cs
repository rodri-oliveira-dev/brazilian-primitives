using System.Reflection;
using Xunit;

namespace Brazilian.PrimitivesTypes.Dapper.SqlServer.Tests;

public sealed class PackageFoundationTests
{
    [Fact]
    public void DapperSqlServerIntegrationAssemblyCanBeLoaded()
    {
        Assembly assembly = Assembly.Load("Brazilian.PrimitivesTypes.Dapper.SqlServer");

        Assert.Equal("Brazilian.PrimitivesTypes.Dapper.SqlServer", assembly.GetName().Name);
    }
}
