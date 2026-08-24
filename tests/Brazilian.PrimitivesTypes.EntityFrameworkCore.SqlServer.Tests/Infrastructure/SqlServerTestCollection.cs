using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure;

internal static class SqlServerTestCollection
{
    public const string Name = "SQL Server integration";
}

[CollectionDefinition(SqlServerTestCollection.Name, DisableParallelization = true)]
public sealed class SqlServerTestCollectionDefinition : ICollectionFixture<SqlServerContainerFixture>
{
}
