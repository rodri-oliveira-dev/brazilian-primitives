using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Contexts;

internal sealed class ContextFreeConventionDbContext(DbContextOptions<ContextFreeConventionDbContext> options) : DbContext(options)
{
    public DbSet<ContextFreeStateRecord> Records => Set<ContextFreeStateRecord>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.UseBrazilianContextFreeStateRegistrationsPostgreSql();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("integration");
        var entity = modelBuilder.Entity<ContextFreeStateRecord>();
        entity.ToTable("context_free_convention_records");
        entity.HasKey(record => record.Id);
        entity.Property(record => record.Id).HasColumnName("id").ValueGeneratedNever();
    }
}
