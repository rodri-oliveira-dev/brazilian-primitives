using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;

internal readonly record struct ScalarPrimitivePostgreSqlMapping<TPrimitive, TConverter>(int MaxLength)
    where TPrimitive : struct
    where TConverter : ValueConverter, new()
{
    public PropertyBuilder Apply(PropertyBuilder builder) =>
        ScalarPropertyConfigurator.Configure<TPrimitive>(builder, new TConverter(), MaxLength);

    public void Apply(ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        string columnType = $"character varying({MaxLength})";

        configurationBuilder.Properties<TPrimitive>()
            .HaveConversion<TConverter>()
            .HaveMaxLength(MaxLength)
            .HaveColumnType(columnType);

        configurationBuilder.Properties<TPrimitive?>()
            .HaveConversion<TConverter>()
            .HaveMaxLength(MaxLength)
            .HaveColumnType(columnType);
    }
}
