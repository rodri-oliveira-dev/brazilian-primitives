using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

/// <summary>
/// Configures lossless SQL Server table-splitting for a state-aware <see cref="InscricaoEstadual"/> complex property.
/// </summary>
public static class InscricaoEstadualStateAwareSqlServerMapping
{
    /// <summary>
    /// Configures the canonical state-registration value and UF as two columns on the containing entity table.
    /// </summary>
    /// <param name="builder">The EF Core complex-property builder for the state-registration property.</param>
    /// <param name="valueColumnName">Optional override for the canonical registration value column name.</param>
    /// <param name="stateColumnName">Optional override for the two-letter UF column name.</param>
    public static void Configure(
        ComplexPropertyBuilder<InscricaoEstadual> builder,
        string? valueColumnName = null,
        string? stateColumnName = null)
    {
        StateAwareComplexPropertyConfigurator.Configure(
            builder,
            value => value.Value,
            value => value.State,
            "varchar(14)",
            valueColumnName,
            stateColumnName);
    }
}
