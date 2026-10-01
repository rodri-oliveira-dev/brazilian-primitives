using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Contexts;

internal sealed class OptionalStateDbContext(DbContextOptions<OptionalStateDbContext> options) : DbContext(options)
{
    public DbSet<ContextFreeStateRecord> ContextFreeRecords => Set<ContextFreeStateRecord>();
    public DbSet<StateAwareRecord> StateAwareRecords => Set<StateAwareRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("integration");

        modelBuilder.Entity<ContextFreeStateRecord>(entity =>
        {
            ConfigureId(entity, "context_free_state_records");
            ContextFreeStatePostgreSqlMappings.Rg.Apply(
                entity.Property(record => record.Rg).HasColumnName("rg"));
            ContextFreeStatePostgreSqlMappings.Rg.Apply(
                entity.Property(record => record.OptionalRg).HasColumnName("optional_rg"));
            ContextFreeStatePostgreSqlMappings.InscricaoEstadual.Apply(
                entity.Property(record => record.InscricaoEstadual).HasColumnName("inscricao_estadual"));
            ContextFreeStatePostgreSqlMappings.InscricaoEstadual.Apply(
                entity.Property(record => record.OptionalInscricaoEstadual).HasColumnName("optional_inscricao_estadual"));
        });

        modelBuilder.Entity<StateAwareRecord>(entity =>
        {
            ConfigureId(entity, "state_aware_records");
            entity.ComplexProperty(
                record => record.Rg,
                complex => RgStateAwarePostgreSqlMapping.Configure(complex, "rg_value", "rg_state"));
            entity.ComplexProperty(
                record => record.OptionalRg,
                complex => RgStateAwarePostgreSqlMapping.Configure(
                    complex,
                    "optional_rg_value",
                    "optional_rg_state"));
            entity.ComplexProperty(
                record => record.InscricaoEstadual,
                complex => InscricaoEstadualStateAwarePostgreSqlMapping.Configure(
                    complex,
                    "inscricao_value",
                    "inscricao_state"));
            entity.ComplexProperty(
                record => record.OptionalInscricaoEstadual,
                complex => InscricaoEstadualStateAwarePostgreSqlMapping.Configure(
                    complex,
                    "optional_inscricao_value",
                    "optional_inscricao_state"));
        });
    }

    private static void ConfigureId<TEntity>(
        EntityTypeBuilder<TEntity> entity,
        string tableName)
        where TEntity : class
    {
        entity.ToTable(tableName);
        entity.HasKey("Id");
        entity.Property<int>("Id").HasColumnName("id").ValueGeneratedNever();
    }
}
