using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;

public static partial class BrazilianPrimitivePostgreSqlPropertyBuilderExtensions
{
    /// <summary>
    /// Configures an RG complex property for lossless state-aware PostgreSQL persistence.
    /// </summary>
    /// <example>
    /// <code>
    /// entity.ComplexProperty(x =&gt; x.Rg)
    ///     .HasBrazilianRgStateAwarePostgreSql("rg_value", "rg_state");
    /// </code>
    /// </example>
    /// <param name="builder">The RG complex-property builder.</param>
    /// <param name="valueColumnName">Optional canonical-value column name.</param>
    /// <param name="stateColumnName">Optional two-letter UF column name.</param>
    /// <returns>The same complex-property builder.</returns>
    public static ComplexPropertyBuilder<Rg> HasBrazilianRgStateAwarePostgreSql(
        this ComplexPropertyBuilder<Rg> builder,
        string? valueColumnName = null,
        string? stateColumnName = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        RgStateAwarePostgreSqlMapping.Configure(builder, valueColumnName, stateColumnName);
        return builder;
    }

    /// <summary>
    /// Configures an Inscricao Estadual complex property for lossless state-aware PostgreSQL persistence.
    /// </summary>
    /// <example>
    /// <code>
    /// entity.ComplexProperty(x =&gt; x.InscricaoEstadual)
    ///     .HasBrazilianInscricaoEstadualStateAwarePostgreSql("inscricao_value", "inscricao_state");
    /// </code>
    /// </example>
    /// <param name="builder">The Inscricao Estadual complex-property builder.</param>
    /// <param name="valueColumnName">Optional canonical-value column name.</param>
    /// <param name="stateColumnName">Optional two-letter UF column name.</param>
    /// <returns>The same complex-property builder.</returns>
    public static ComplexPropertyBuilder<InscricaoEstadual> HasBrazilianInscricaoEstadualStateAwarePostgreSql(
        this ComplexPropertyBuilder<InscricaoEstadual> builder,
        string? valueColumnName = null,
        string? stateColumnName = null)
    {
        ArgumentNullException.ThrowIfNull(builder);
        InscricaoEstadualStateAwarePostgreSqlMapping.Configure(builder, valueColumnName, stateColumnName);
        return builder;
    }
}
