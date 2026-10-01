using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Contexts;

internal sealed class ExplicitStateMappingDbContext(DbContextOptions<ExplicitStateMappingDbContext> options) : DbContext(options)
{
    public DbSet<ContextFreeStateRecord> ContextFreeRecords => Set<ContextFreeStateRecord>();
    public DbSet<StateAwareRecord> StateAwareRecords => Set<StateAwareRecord>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("integration");

        var contextFree = modelBuilder.Entity<ContextFreeStateRecord>();
        contextFree.ToTable("explicit_context_free_records");
        contextFree.HasKey(record => record.Id);
        contextFree.Property(record => record.Id).HasColumnName("id").ValueGeneratedNever();
        contextFree.Property(record => record.Rg).HasColumnName("rg").HasBrazilianRgContextFreePostgreSql();
        contextFree.Property(record => record.OptionalRg).HasColumnName("optional_rg").HasBrazilianRgContextFreePostgreSql();
        contextFree.Property(record => record.InscricaoEstadual)
            .HasColumnName("inscricao_estadual")
            .HasBrazilianInscricaoEstadualContextFreePostgreSql();
        contextFree.Property(record => record.OptionalInscricaoEstadual)
            .HasColumnName("optional_inscricao_estadual")
            .HasBrazilianInscricaoEstadualContextFreePostgreSql();

        var stateAware = modelBuilder.Entity<StateAwareRecord>();
        stateAware.ToTable("explicit_state_aware_records");
        stateAware.HasKey(record => record.Id);
        stateAware.Property(record => record.Id).HasColumnName("id").ValueGeneratedNever();
        stateAware.ComplexProperty(
            record => record.Rg,
            complex => complex.HasBrazilianRgStateAwarePostgreSql("rg_value", "rg_state"));
        stateAware.ComplexProperty(
            record => record.OptionalRg,
            complex => complex.HasBrazilianRgStateAwarePostgreSql("optional_rg_value", "optional_rg_state"));
        stateAware.ComplexProperty(
            record => record.InscricaoEstadual,
            complex => complex.HasBrazilianInscricaoEstadualStateAwarePostgreSql("inscricao_value", "inscricao_state"));
        stateAware.ComplexProperty(
            record => record.OptionalInscricaoEstadual,
            complex => complex.HasBrazilianInscricaoEstadualStateAwarePostgreSql(
                "optional_inscricao_value",
                "optional_inscricao_state"));
    }
}
