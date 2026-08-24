using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Contexts;

internal sealed class ConventionDbContext(DbContextOptions<ConventionDbContext> options) : DbContext(options)
{
    public DbSet<AllConventionRecord> AllConventionRecords => Set<AllConventionRecord>();

    public DbSet<NullableConventionRecord> NullableConventionRecords => Set<NullableConventionRecord>();

    public DbSet<MappingRecord> MappingRecords => Set<MappingRecord>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.UseBrazilianPrimitiveTypesSqlServer();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        EntityConfiguration.ConfigureRecord(modelBuilder.Entity<AllConventionRecord>(), "AllConventionRecords");
        EntityConfiguration.ConfigureRecord(modelBuilder.Entity<NullableConventionRecord>(), "NullableConventionRecords");
        EntityConfiguration.ConfigureRecord(modelBuilder.Entity<MappingRecord>(), "MappingRecords");
    }
}
