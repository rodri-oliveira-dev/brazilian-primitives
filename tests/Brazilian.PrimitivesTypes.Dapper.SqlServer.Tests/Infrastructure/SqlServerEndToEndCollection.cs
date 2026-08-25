using Xunit;

namespace Brazilian.PrimitivesTypes.Dapper.SqlServer.Tests.Infrastructure;

internal static class SqlServerEndToEndCollection
{
    internal const string Name = "Dapper SQL Server end-to-end";
}

[CollectionDefinition(SqlServerEndToEndCollection.Name, DisableParallelization = true)]
public sealed class SqlServerEndToEndCollectionDefinition : ICollectionFixture<SqlServerContainerFixture>
{
}
