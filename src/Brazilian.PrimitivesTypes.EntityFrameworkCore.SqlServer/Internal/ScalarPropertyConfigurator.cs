using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

internal static class ScalarPropertyConfigurator
{
    public static PropertyBuilder Configure<TPrimitive>(
        PropertyBuilder builder,
        ValueConverter converter,
        int maxLength)
        where TPrimitive : struct
    {
        ArgumentNullException.ThrowIfNull(builder);

        Type configuredType = Nullable.GetUnderlyingType(builder.Metadata.ClrType) ?? builder.Metadata.ClrType;
        if (configuredType != typeof(TPrimitive))
        {
            throw new InvalidOperationException(
                $"The SQL Server mapping for {typeof(TPrimitive).Name} cannot be applied to property type {builder.Metadata.ClrType.Name}.");
        }

        builder.HasConversion(converter)
            .HasMaxLength(maxLength)
            .IsUnicode(false);

        return builder;
    }
}
