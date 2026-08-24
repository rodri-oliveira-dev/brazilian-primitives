using System.Linq.Expressions;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

internal static class StateAwareComplexPropertyConfigurator
{
    public static void Configure<TComplex>(
        ComplexPropertyBuilder<TComplex> builder,
        Expression<Func<TComplex, string>> valueExpression,
        Expression<Func<TComplex, BrazilianState>> stateExpression,
        string valueColumnType,
        string? valueColumnName,
        string? stateColumnName)
        where TComplex : notnull
    {
        ArgumentNullException.ThrowIfNull(builder);

        ComplexTypePropertyBuilder<string> valueProperty = builder.Property(valueExpression)
            .HasColumnType(valueColumnType)
            .IsRequired();
        ApplyColumnName(valueProperty, valueColumnName);

        ComplexTypePropertyBuilder<BrazilianState> stateProperty = builder.Property(stateExpression)
            .HasConversion(new BrazilianStateCodeValueConverter())
            .HasColumnType("varchar(2)")
            .IsRequired();
        ApplyColumnName(stateProperty, stateColumnName);
    }

    private static void ApplyColumnName<TProperty>(
        ComplexTypePropertyBuilder<TProperty> propertyBuilder,
        string? columnName)
    {
        if (columnName is null)
        {
            return;
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);
        propertyBuilder.HasColumnName(columnName);
    }
}
