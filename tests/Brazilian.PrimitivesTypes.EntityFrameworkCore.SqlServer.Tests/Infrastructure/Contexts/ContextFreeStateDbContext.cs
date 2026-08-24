using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Contexts;

internal sealed class ContextFreeStateDbContext(DbContextOptions<ContextFreeStateDbContext> options) : DbContext(options)
{
    public DbSet<ContextFreeStateRecord> Records => Set<ContextFreeStateRecord>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .UseBrazilianPrimitiveTypesSqlServer()
            .UseBrazilianContextFreeStateRegistrationsSqlServer();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        EntityConfiguration.ConfigureRecord(modelBuilder.Entity<ContextFreeStateRecord>(), "ContextFreeStateRecords");
    }
}
