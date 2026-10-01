using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Contexts;

internal sealed class OverrideMappingDbContext(DbContextOptions<OverrideMappingDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("integration");

        var entity = modelBuilder.Entity<OverrideRecord>();
        entity.ToTable("override_records");
        entity.HasKey(record => record.Id);
        entity.Property(record => record.Id).HasColumnName("id").ValueGeneratedNever();
        entity.Property(record => record.Email)
            .HasBrazilianEmailPostgreSql()
            .HasColumnName("contact_email")
            .HasColumnType("text")
            .IsRequired();
    }
}
