using Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests.Infrastructure.Contexts;

internal static class EntityConfiguration
{
    public static void ConfigureRecord<TEntity>(EntityTypeBuilder<TEntity> entity, string tableName)
        where TEntity : class, IRecordWithId
    {
        entity.ToTable(tableName);
        entity.HasKey(record => record.Id);
        entity.Property(record => record.Id).ValueGeneratedNever();
    }
}
