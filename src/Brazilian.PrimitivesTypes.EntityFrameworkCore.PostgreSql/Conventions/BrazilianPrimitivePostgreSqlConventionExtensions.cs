using Microsoft.EntityFrameworkCore;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;

/// <summary>
/// Provides opt-in EF Core pre-conventions for Brazilian primitive PostgreSQL mappings.
/// </summary>
public static class BrazilianPrimitivePostgreSqlConventionExtensions
{
    /// <summary>
    /// Registers model-wide PostgreSQL mappings for scalar Brazilian primitive value objects.
    /// </summary>
    /// <remarks>
    /// Registration is explicit and affects only contexts that call this method from
    /// <see cref="DbContext.ConfigureConventions(ModelConfigurationBuilder)"/>. RG and Inscricao Estadual are deliberately
    /// excluded because their persistence mode cannot be inferred safely. Configure those properties explicitly or call
    /// <see cref="UseBrazilianContextFreeStateRegistrationsPostgreSql"/> when every RG/IE property in the model is
    /// intentionally context-free.
    /// </remarks>
    /// <example>
    /// <code>
    /// protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    /// {
    ///     configurationBuilder.UseBrazilianPrimitiveTypesPostgreSql();
    /// }
    /// </code>
    /// </example>
    /// <param name="configurationBuilder">The EF Core model configuration builder.</param>
    /// <returns>The same configuration builder.</returns>
    public static ModelConfigurationBuilder UseBrazilianPrimitiveTypesPostgreSql(
        this ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        ScalarPrimitivePostgreSqlMappings.Cpf.Apply(configurationBuilder);
        ScalarPrimitivePostgreSqlMappings.Cnpj.Apply(configurationBuilder);
        ScalarPrimitivePostgreSqlMappings.CpfCnpj.Apply(configurationBuilder);
        ScalarPrimitivePostgreSqlMappings.Cep.Apply(configurationBuilder);
        ScalarPrimitivePostgreSqlMappings.Email.Apply(configurationBuilder);
        ScalarPrimitivePostgreSqlMappings.MobilePhone.Apply(configurationBuilder);
        ScalarPrimitivePostgreSqlMappings.LandlinePhone.Apply(configurationBuilder);
        ScalarPrimitivePostgreSqlMappings.TelefoneBrasileiro.Apply(configurationBuilder);
        ScalarPrimitivePostgreSqlMappings.ChavePix.Apply(configurationBuilder);
        ScalarPrimitivePostgreSqlMappings.Cnh.Apply(configurationBuilder);
        ScalarPrimitivePostgreSqlMappings.Cns.Apply(configurationBuilder);
        ScalarPrimitivePostgreSqlMappings.TituloEleitoral.Apply(configurationBuilder);
        ScalarPrimitivePostgreSqlMappings.Nit.Apply(configurationBuilder);
        ScalarPrimitivePostgreSqlMappings.PisPasep.Apply(configurationBuilder);
        ScalarPrimitivePostgreSqlMappings.PlacaVeiculo.Apply(configurationBuilder);
        ScalarPrimitivePostgreSqlMappings.Renavam.Apply(configurationBuilder);
        ScalarPrimitivePostgreSqlMappings.Ispb.Apply(configurationBuilder);
        ScalarPrimitivePostgreSqlMappings.CodigoCompe.Apply(configurationBuilder);

        return configurationBuilder;
    }

    /// <summary>
    /// Registers model-wide context-free single-column PostgreSQL mappings for RG and Inscricao Estadual.
    /// </summary>
    /// <remarks>
    /// This opt-in never creates or requires a UF column and rejects state-aware instances rather than discarding known
    /// UF context. State-aware RG/IE mappings remain explicit complex-property configuration.
    /// </remarks>
    /// <example>
    /// <code>
    /// protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    /// {
    ///     configurationBuilder
    ///         .UseBrazilianPrimitiveTypesPostgreSql()
    ///         .UseBrazilianContextFreeStateRegistrationsPostgreSql();
    /// }
    /// </code>
    /// </example>
    /// <param name="configurationBuilder">The EF Core model configuration builder.</param>
    /// <returns>The same configuration builder.</returns>
    public static ModelConfigurationBuilder UseBrazilianContextFreeStateRegistrationsPostgreSql(
        this ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        ContextFreeStatePostgreSqlMappings.Rg.Apply(configurationBuilder);
        ContextFreeStatePostgreSqlMappings.InscricaoEstadual.Apply(configurationBuilder);

        return configurationBuilder;
    }
}
