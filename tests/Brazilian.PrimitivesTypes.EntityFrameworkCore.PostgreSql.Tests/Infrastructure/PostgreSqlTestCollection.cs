using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure;

internal static class PostgreSqlTestCollection
{
    public const string Name = "PostgreSQL integration";
}

[CollectionDefinition(PostgreSqlTestCollection.Name, DisableParallelization = true)]
public sealed class PostgreSqlTestCollectionDefinition : ICollectionFixture<PostgreSqlContainerFixture>
{
}
