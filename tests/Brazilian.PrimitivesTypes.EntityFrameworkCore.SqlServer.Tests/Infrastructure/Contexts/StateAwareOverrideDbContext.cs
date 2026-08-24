using Brazilian.PrimitivesTypes;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Contexts;

internal sealed class StateAwareOverrideDbContext(DbContextOptions<StateAwareOverrideDbContext> options) : DbContext(options)
{
    public DbSet<StateAwareRecord> Records => Set<StateAwareRecord>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.UseBrazilianPrimitiveTypesSqlServer();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        EntityTypeBuilder<StateAwareRecord> entity = modelBuilder.Entity<StateAwareRecord>();
        EntityConfiguration.ConfigureRecord(entity, "StateAwareFluentRecords");

        ComplexPropertyBuilder<Rg> rg = entity.ComplexProperty(record => record.Rg)
            .HasBrazilianRgStateAwareSqlServer("RgNumber", "RgUf");
        rg.Property(value => value.Value).HasColumnType("varchar(12)");

        entity.ComplexProperty(record => record.OptionalRg)
            .HasBrazilianRgStateAwareSqlServer("OptionalRgNumber", "OptionalRgUf");
        entity.ComplexProperty(record => record.InscricaoEstadual)
            .HasBrazilianInscricaoEstadualStateAwareSqlServer("IeNumber", "IeUf");
        entity.ComplexProperty(record => record.OptionalInscricaoEstadual)
            .HasBrazilianInscricaoEstadualStateAwareSqlServer("OptionalIeNumber", "OptionalIeUf");
    }
}
