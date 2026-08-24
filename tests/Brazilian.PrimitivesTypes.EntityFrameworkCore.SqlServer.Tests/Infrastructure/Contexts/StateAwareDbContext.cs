using Brazilian.PrimitivesTypes;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Contexts;

internal sealed class StateAwareDbContext(DbContextOptions<StateAwareDbContext> options) : DbContext(options)
{
    public DbSet<ContextFreeStateRecord> ContextFreeRecords => Set<ContextFreeStateRecord>();

    public DbSet<StateAwareRecord> StateAwareRecords => Set<StateAwareRecord>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.UseBrazilianPrimitiveTypesSqlServer();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContextFreeStateRecord>(entity =>
        {
            EntityConfiguration.ConfigureRecord(entity, "ContextFreeRecords");
            entity.Property(record => record.Rg).HasBrazilianRgContextFreeSqlServer();
            entity.Property(record => record.OptionalRg).HasBrazilianRgContextFreeSqlServer();
            entity.Property(record => record.InscricaoEstadual).HasBrazilianInscricaoEstadualContextFreeSqlServer();
            entity.Property(record => record.OptionalInscricaoEstadual).HasBrazilianInscricaoEstadualContextFreeSqlServer();
        });

        modelBuilder.Entity<StateAwareRecord>(entity =>
        {
            EntityConfiguration.ConfigureRecord(entity, "StateAwareRecords");
            ConfigureStateAwareProperties(entity);
        });
    }

    private static void ConfigureStateAwareProperties(EntityTypeBuilder<StateAwareRecord> entity)
    {
        entity.ComplexProperty(
            record => record.Rg,
            complex => complex.HasBrazilianRgStateAwareSqlServer("RgValue", "RgState"));
        entity.ComplexProperty(
            record => record.OptionalRg,
            complex => complex.HasBrazilianRgStateAwareSqlServer("OptionalRgValue", "OptionalRgState"));
        entity.ComplexProperty(
            record => record.InscricaoEstadual,
            complex => complex.HasBrazilianInscricaoEstadualStateAwareSqlServer("InscricaoValue", "InscricaoState"));
        entity.ComplexProperty(
            record => record.OptionalInscricaoEstadual,
            complex => complex.HasBrazilianInscricaoEstadualStateAwareSqlServer(
                "OptionalInscricaoValue",
                "OptionalInscricaoState"));
    }
}
