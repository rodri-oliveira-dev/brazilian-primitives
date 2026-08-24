using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

public static partial class BrazilianPrimitiveSqlServerPropertyBuilderExtensions
{
    /// <summary>
    /// Configures an RG complex property for lossless state-aware SQL Server persistence.
    /// </summary>
    /// <example>
    /// <code>
    /// entity.ComplexProperty(x =&gt; x.Rg)
    ///     .HasBrazilianRgStateAwareSqlServer("RgValue", "RgState");
    /// </code>
    /// </example>
    /// <param name="builder">The RG complex-property builder.</param>
    /// <param name="valueColumnName">Optional canonical-value column name.</param>
    /// <param name="stateColumnName">Optional two-letter UF column name.</param>
    /// <returns>The same complex-property builder.</returns>
    public static ComplexPropertyBuilder<Rg> HasBrazilianRgStateAwareSqlServer(
        this ComplexPropertyBuilder<Rg> builder,
        string? valueColumnName = null,
        string? stateColumnName = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        RgStateAwareSqlServerMapping.Configure(builder, valueColumnName, stateColumnName);
        return builder;
    }

    /// <summary>
    /// Configures an Inscricao Estadual complex property for lossless state-aware SQL Server persistence.
    /// </summary>
    /// <example>
    /// <code>
    /// entity.ComplexProperty(x =&gt; x.InscricaoEstadual)
    ///     .HasBrazilianInscricaoEstadualStateAwareSqlServer("InscricaoValue", "InscricaoState");
    /// </code>
    /// </example>
    /// <param name="builder">The Inscricao Estadual complex-property builder.</param>
    /// <param name="valueColumnName">Optional canonical-value column name.</param>
    /// <param name="stateColumnName">Optional two-letter UF column name.</param>
    /// <returns>The same complex-property builder.</returns>
    public static ComplexPropertyBuilder<InscricaoEstadual> HasBrazilianInscricaoEstadualStateAwareSqlServer(
        this ComplexPropertyBuilder<InscricaoEstadual> builder,
        string? valueColumnName = null,
        string? stateColumnName = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        InscricaoEstadualStateAwareSqlServerMapping.Configure(builder, valueColumnName, stateColumnName);
        return builder;
    }
}
