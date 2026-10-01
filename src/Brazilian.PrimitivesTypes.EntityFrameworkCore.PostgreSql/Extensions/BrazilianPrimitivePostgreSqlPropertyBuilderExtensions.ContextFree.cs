using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;

public static partial class BrazilianPrimitivePostgreSqlPropertyBuilderExtensions
{
    /// <summary>
    /// Configures an RG property for context-free single-column PostgreSQL persistence.
    /// </summary>
    /// <remarks>
    /// This mode stores only <see cref="Rg.Value"/> and accepts only RG instances without known UF context.
    /// Use <see cref="HasBrazilianRgStateAwarePostgreSql"/> when the UF must be preserved.
    /// </remarks>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianRgContextFreePostgreSql(this PropertyBuilder builder) =>
        ContextFreeStatePostgreSqlMappings.Rg.Apply(builder);

    /// <summary>
    /// Configures an Inscricao Estadual property for context-free single-column PostgreSQL persistence.
    /// </summary>
    /// <remarks>
    /// This mode stores only <see cref="InscricaoEstadual.Value"/> and accepts only values without known UF context.
    /// Use <see cref="HasBrazilianInscricaoEstadualStateAwarePostgreSql"/> when the UF must be preserved.
    /// </remarks>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianInscricaoEstadualContextFreePostgreSql(this PropertyBuilder builder) =>
        ContextFreeStatePostgreSqlMappings.InscricaoEstadual.Apply(builder);
}
