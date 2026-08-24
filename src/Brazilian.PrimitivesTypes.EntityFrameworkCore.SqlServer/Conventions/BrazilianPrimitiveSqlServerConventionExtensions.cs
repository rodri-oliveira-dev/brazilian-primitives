using Microsoft.EntityFrameworkCore;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

/// <summary>
/// Provides opt-in EF Core pre-conventions for Brazilian primitive SQL Server mappings.
/// </summary>
public static class BrazilianPrimitiveSqlServerConventionExtensions
{
    /// <summary>
    /// Registers model-wide SQL Server mappings for scalar Brazilian primitive value objects.
    /// </summary>
    /// <remarks>
    /// Registration is explicit and affects only contexts that call this method from
    /// <see cref="DbContext.ConfigureConventions(ModelConfigurationBuilder)"/>. RG and Inscricao Estadual are deliberately
    /// excluded because their persistence mode cannot be inferred safely: use property-level context-free/state-aware
    /// extensions, or additionally call <see cref="UseBrazilianContextFreeStateRegistrationsSqlServer"/> when every RG/IE
    /// property in the model is intentionally context-free.
    /// </remarks>
    /// <example>
    /// <code>
    /// protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    /// {
    ///     configurationBuilder.UseBrazilianPrimitiveTypesSqlServer();
    /// }
    /// </code>
    /// </example>
    /// <param name="configurationBuilder">The EF Core model configuration builder.</param>
    /// <returns>The same configuration builder.</returns>
    public static ModelConfigurationBuilder UseBrazilianPrimitiveTypesSqlServer(
        this ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        ScalarPrimitiveSqlServerMappings.Cpf.Apply(configurationBuilder);
        ScalarPrimitiveSqlServerMappings.Cnpj.Apply(configurationBuilder);
        ScalarPrimitiveSqlServerMappings.CpfCnpj.Apply(configurationBuilder);
        ScalarPrimitiveSqlServerMappings.Cep.Apply(configurationBuilder);
        ScalarPrimitiveSqlServerMappings.Email.Apply(configurationBuilder);
        ScalarPrimitiveSqlServerMappings.MobilePhone.Apply(configurationBuilder);
        ScalarPrimitiveSqlServerMappings.LandlinePhone.Apply(configurationBuilder);
        ScalarPrimitiveSqlServerMappings.TelefoneBrasileiro.Apply(configurationBuilder);
        ScalarPrimitiveSqlServerMappings.ChavePix.Apply(configurationBuilder);
        ScalarPrimitiveSqlServerMappings.Cnh.Apply(configurationBuilder);
        ScalarPrimitiveSqlServerMappings.Cns.Apply(configurationBuilder);
        ScalarPrimitiveSqlServerMappings.TituloEleitoral.Apply(configurationBuilder);
        ScalarPrimitiveSqlServerMappings.Nit.Apply(configurationBuilder);
        ScalarPrimitiveSqlServerMappings.PisPasep.Apply(configurationBuilder);
        ScalarPrimitiveSqlServerMappings.PlacaVeiculo.Apply(configurationBuilder);
        ScalarPrimitiveSqlServerMappings.Renavam.Apply(configurationBuilder);
        ScalarPrimitiveSqlServerMappings.Ispb.Apply(configurationBuilder);
        ScalarPrimitiveSqlServerMappings.CodigoCompe.Apply(configurationBuilder);

        return configurationBuilder;
    }

    /// <summary>
    /// Registers model-wide context-free single-column SQL Server mappings for RG and Inscricao Estadual.
    /// </summary>
    /// <remarks>
    /// This opt-in never creates a UF column and rejects state-aware instances rather than discarding known UF context.
    /// Do not use it for a model that needs state-aware RG/IE complex properties; configure those properties explicitly
    /// with <see cref="BrazilianPrimitiveSqlServerPropertyBuilderExtensions.HasBrazilianRgStateAwareSqlServer"/> or
    /// <see cref="BrazilianPrimitiveSqlServerPropertyBuilderExtensions.HasBrazilianInscricaoEstadualStateAwareSqlServer"/>.
    /// </remarks>
    /// <example>
    /// <code>
    /// protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    /// {
    ///     configurationBuilder
    ///         .UseBrazilianPrimitiveTypesSqlServer()
    ///         .UseBrazilianContextFreeStateRegistrationsSqlServer();
    /// }
    /// </code>
    /// </example>
    /// <param name="configurationBuilder">The EF Core model configuration builder.</param>
    /// <returns>The same configuration builder.</returns>
    public static ModelConfigurationBuilder UseBrazilianContextFreeStateRegistrationsSqlServer(
        this ModelConfigurationBuilder configurationBuilder)
    {
        ArgumentNullException.ThrowIfNull(configurationBuilder);

        ContextFreeStateSqlServerMappings.Rg.Apply(configurationBuilder);
        ContextFreeStateSqlServerMappings.InscricaoEstadual.Apply(configurationBuilder);

        return configurationBuilder;
    }
}
