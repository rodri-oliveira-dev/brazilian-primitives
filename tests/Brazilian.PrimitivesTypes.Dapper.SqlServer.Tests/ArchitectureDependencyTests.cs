using System.Reflection;
using Xunit;

namespace Brazilian.PrimitivesTypes.Dapper.SqlServer.Tests;

public sealed class ArchitectureDependencyTests
{
    [Fact]
    public void DapperAdapterMustDependOnCoreAndDapperAndNotOnEntityFrameworkCoreAdapter()
    {
        Assembly assembly = Assembly.Load("Brazilian.PrimitivesTypes.Dapper.SqlServer");

        string[] references = assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty)
            .ToArray();

        Assert.Contains("Brazilian.PrimitivesTypes", references);
        Assert.Contains("Dapper", references);
        Assert.DoesNotContain("Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer", references);
    }
}
