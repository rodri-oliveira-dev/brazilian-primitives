using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Contexts;

internal sealed class ExplicitMappingDbContext(DbContextOptions<ExplicitMappingDbContext> options) : DbContext(options)
{
    public DbSet<MappingRecord> MappingRecords => Set<MappingRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("integration");

        var entity = modelBuilder.Entity<MappingRecord>();
        entity.ToTable("explicit_mapping_records");
        entity.HasKey(record => record.Id);
        entity.Property(record => record.Id).HasColumnName("id").ValueGeneratedNever();
        entity.Property(record => record.Cpf).HasColumnName("cpf").HasBrazilianCpfPostgreSql();
        entity.Property(record => record.Email).HasColumnName("email").HasBrazilianEmailPostgreSql();
        entity.Property(record => record.Cep).HasColumnName("cep").HasBrazilianCepPostgreSql();
    }
}
