using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

/// <summary>
/// Configures lossless SQL Server table-splitting for a state-aware <see cref="Rg"/> complex property.
/// </summary>
public static class RgStateAwareSqlServerMapping
{
    /// <summary>
    /// Configures the RG canonical value and issuing UF as two columns on the containing entity table.
    /// </summary>
    /// <param name="builder">The EF Core complex-property builder for the RG property.</param>
    /// <param name="valueColumnName">Optional override for the canonical RG value column name.</param>
    /// <param name="stateColumnName">Optional override for the two-letter UF column name.</param>
    public static void Configure(
        ComplexPropertyBuilder<Rg> builder,
        string? valueColumnName = null,
        string? stateColumnName = null)
    {
        StateAwareComplexPropertyConfigurator.Configure(
            builder,
            value => value.Value,
            value => value.State,
            "varchar(10)",
            valueColumnName,
            stateColumnName);
    }
}
