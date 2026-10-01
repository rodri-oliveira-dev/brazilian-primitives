using Microsoft.EntityFrameworkCore;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure;

internal static class PostgreSqlDbContextOptionsFactory
{
    public static DbContextOptions<TContext> Create<TContext>(
        PostgreSqlContainerFixture fixture,
        string databaseName)
        where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(fixture);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseName);

        return new DbContextOptionsBuilder<TContext>()
            .UseNpgsql(fixture.GetConnectionString(databaseName))
            .Options;
    }
}
