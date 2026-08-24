using Microsoft.EntityFrameworkCore;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure;

internal static class SqlServerDbContextOptionsFactory
{
    public static DbContextOptions<TContext> Create<TContext>(
        SqlServerContainerFixture fixture,
        string databaseName)
        where TContext : DbContext
    {
        ArgumentNullException.ThrowIfNull(fixture);

        return new DbContextOptionsBuilder<TContext>()
            .UseSqlServer(fixture.GetConnectionString(databaseName))
            .Options;
    }
}
