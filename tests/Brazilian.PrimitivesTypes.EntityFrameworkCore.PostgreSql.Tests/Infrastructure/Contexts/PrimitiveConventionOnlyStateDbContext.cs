using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;
using Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql.Tests.Infrastructure.Contexts;

internal sealed class PrimitiveConventionOnlyStateDbContext(
    DbContextOptions<PrimitiveConventionOnlyStateDbContext> options) : DbContext(options)
{
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.UseBrazilianPrimitiveTypesPostgreSql();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ContextFreeStateRecord>();
        entity.HasKey(record => record.Id);
    }
}
