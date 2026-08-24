using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Contexts;

internal sealed class OverrideMappingDbContext(DbContextOptions<OverrideMappingDbContext> options) : DbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        EntityTypeBuilder<OverrideRecord> entity = modelBuilder.Entity<OverrideRecord>();
        EntityConfiguration.ConfigureRecord(entity, "OverrideRecords");
        entity.Property(record => record.Email)
            .HasBrazilianEmailSqlServer()
            .HasColumnName("ContactEmail")
            .HasColumnType("varchar(320)")
            .IsRequired();
    }
}
