using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Contexts;

internal sealed class ConventionMappingDbContext(DbContextOptions<ConventionMappingDbContext> options) : DbContext(options)
{
    public DbSet<ScalarPrimitiveRecord> ScalarRecords => Set<ScalarPrimitiveRecord>();
    public DbSet<NullableConventionRecord> NullableRecords => Set<NullableConventionRecord>();
    public DbSet<MappingRecord> MappingRecords => Set<MappingRecord>();

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.UseBrazilianPrimitiveTypesPostgreSql();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("integration");
        ConfigureEntity(modelBuilder.Entity<ScalarPrimitiveRecord>(), "convention_scalar_records");
        ConfigureEntity(modelBuilder.Entity<NullableConventionRecord>(), "convention_nullable_records");
        ConfigureEntity(modelBuilder.Entity<MappingRecord>(), "convention_mapping_records");
    }

    private static void ConfigureEntity<TEntity>(EntityTypeBuilder<TEntity> entity, string tableName)
        where TEntity : class
    {
        entity.ToTable(tableName);
        entity.HasKey("Id");
        entity.Property<int>("Id").HasColumnName("id").ValueGeneratedNever();
    }
}
