using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Contexts;

internal sealed class ExplicitMappingDbContext(DbContextOptions<ExplicitMappingDbContext> options) : DbContext(options)
{
    public DbSet<MappingRecord> MappingRecords => Set<MappingRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        EntityTypeBuilder<MappingRecord> entity = modelBuilder.Entity<MappingRecord>();
        EntityConfiguration.ConfigureRecord(entity, "MappingRecords");
        entity.Property(record => record.Cpf).HasBrazilianCpfSqlServer();
        entity.Property(record => record.Email).HasBrazilianEmailSqlServer();
        entity.Property(record => record.Cep).HasBrazilianCepSqlServer();
    }
}
