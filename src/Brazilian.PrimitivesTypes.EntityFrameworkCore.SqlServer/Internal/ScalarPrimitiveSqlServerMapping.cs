using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

internal readonly record struct ScalarPrimitiveSqlServerMapping<TPrimitive, TConverter>(int MaxLength)
    where TPrimitive : struct
    where TConverter : ValueConverter, new()
{
    public PropertyBuilder Apply(PropertyBuilder builder) =>
        ScalarPropertyConfigurator.Configure<TPrimitive>(builder, new TConverter(), MaxLength);

    public void Apply(ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        configurationBuilder.Properties<TPrimitive>()
            .HaveConversion<TConverter>()
            .HaveMaxLength(MaxLength)
            .AreUnicode(false);

        configurationBuilder.Properties<TPrimitive?>()
            .HaveConversion<TConverter>()
            .HaveMaxLength(MaxLength)
            .AreUnicode(false);
    }
}
