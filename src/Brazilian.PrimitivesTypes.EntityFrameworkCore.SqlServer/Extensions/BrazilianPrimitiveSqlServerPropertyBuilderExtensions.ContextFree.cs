using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

public static partial class BrazilianPrimitiveSqlServerPropertyBuilderExtensions
{
    /// <summary>
    /// Configures an RG property for context-free single-column SQL Server persistence.
    /// </summary>
    /// <remarks>
    /// This mode stores only <see cref="Rg.Value"/> and therefore accepts only RG instances without known UF context.
    /// Use <see cref="HasBrazilianRgStateAwareSqlServer"/> when the UF must be preserved.
    /// </remarks>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianRgContextFreeSqlServer(this PropertyBuilder builder) =>
        ContextFreeStateSqlServerMappings.Rg.Apply(builder);

    /// <summary>
    /// Configures an Inscricao Estadual property for context-free single-column SQL Server persistence.
    /// </summary>
    /// <remarks>
    /// This mode stores only <see cref="InscricaoEstadual.Value"/> and therefore accepts only instances without known UF
    /// context. Use <see cref="HasBrazilianInscricaoEstadualStateAwareSqlServer"/> when the UF must be preserved.
    /// </remarks>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianInscricaoEstadualContextFreeSqlServer(this PropertyBuilder builder) =>
        ContextFreeStateSqlServerMappings.InscricaoEstadual.Apply(builder);
}
