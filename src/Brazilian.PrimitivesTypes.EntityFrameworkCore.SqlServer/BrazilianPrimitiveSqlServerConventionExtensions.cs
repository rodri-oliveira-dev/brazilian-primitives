using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

        Configure<Cpf, CpfValueConverter>(configurationBuilder, 11);
        Configure<Cnpj, CnpjValueConverter>(configurationBuilder, 14);
        Configure<CpfCnpj, CpfCnpjValueConverter>(configurationBuilder, 14);
        Configure<Cep, CepValueConverter>(configurationBuilder, 8);
        Configure<Email, EmailValueConverter>(configurationBuilder, 254);
        Configure<MobilePhone, MobilePhoneValueConverter>(configurationBuilder, 11);
        Configure<LandlinePhone, LandlinePhoneValueConverter>(configurationBuilder, 10);
        Configure<TelefoneBrasileiro, TelefoneBrasileiroValueConverter>(configurationBuilder, 11);
        Configure<ChavePix, ChavePixValueConverter>(configurationBuilder, 77);
        Configure<Cnh, CnhValueConverter>(configurationBuilder, 11);
        Configure<Cns, CnsValueConverter>(configurationBuilder, 15);
        Configure<TituloEleitoral, TituloEleitoralValueConverter>(configurationBuilder, 12);
        Configure<Nit, NitValueConverter>(configurationBuilder, 11);
        Configure<PisPasep, PisPasepValueConverter>(configurationBuilder, 11);
        Configure<PlacaVeiculo, PlacaVeiculoValueConverter>(configurationBuilder, 7);
        Configure<Renavam, RenavamValueConverter>(configurationBuilder, 11);
        Configure<Ispb, IspbValueConverter>(configurationBuilder, 8);
        Configure<CodigoCompe, CodigoCompeValueConverter>(configurationBuilder, 3);

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

        Configure<Rg, RgValueConverter>(configurationBuilder, 10);
        Configure<InscricaoEstadual, InscricaoEstadualValueConverter>(configurationBuilder, 14);

        return configurationBuilder;
    }

    private static void Configure<TPrimitive, TConverter>(
        ModelConfigurationBuilder configurationBuilder,
        int maxLength)
        where TPrimitive : struct
        where TConverter : ValueConverter
    {
        configurationBuilder.Properties<TPrimitive>()
            .HaveConversion<TConverter>()
            .HaveMaxLength(maxLength)
            .AreUnicode(false);

        configurationBuilder.Properties<TPrimitive?>()
            .HaveConversion<TConverter>()
            .HaveMaxLength(maxLength)
            .AreUnicode(false);
    }
}
