using System.Reflection;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests;

public sealed class ArchitectureDependencyTests
{
    [Fact]
    public void EntityFrameworkCoreAdapterMustDependOnCoreAndNotOnDapperAdapter()
    {
        Assembly assembly = Assembly.Load("Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer");

        string[] references = assembly
            .GetReferencedAssemblies()
            .Select(reference => reference.Name ?? string.Empty)
            .ToArray();

        Assert.Contains("Brazilian.PrimitivesTypes", references);
        Assert.DoesNotContain("Brazilian.PrimitivesTypes.Dapper.SqlServer", references);
    }
}
