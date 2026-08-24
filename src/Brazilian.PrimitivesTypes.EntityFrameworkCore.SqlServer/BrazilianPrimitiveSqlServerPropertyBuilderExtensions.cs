using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

/// <summary>
/// Provides explicit SQL Server fluent mappings for Brazilian primitive value objects.
/// </summary>
/// <remarks>
/// These extensions are opt-in and do not create indexes, keys, uniqueness constraints, or aggregate-specific rules.
/// Normal EF Core configuration can be chained after them to override column names, types, lengths, Unicode settings,
/// and required/optional metadata where the underlying CLR type permits it.
/// </remarks>
public static class BrazilianPrimitiveSqlServerPropertyBuilderExtensions
{
    /// <summary>Configures a CPF property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianCpfSqlServer(this PropertyBuilder builder) =>
        ConfigureScalar<Cpf>(builder, new CpfValueConverter(), 11);

    /// <summary>Configures a CNPJ property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianCnpjSqlServer(this PropertyBuilder builder) =>
        ConfigureScalar<Cnpj>(builder, new CnpjValueConverter(), 14);

    /// <summary>Configures a CPF/CNPJ union property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianCpfCnpjSqlServer(this PropertyBuilder builder) =>
        ConfigureScalar<CpfCnpj>(builder, new CpfCnpjValueConverter(), 14);

    /// <summary>Configures a CEP property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianCepSqlServer(this PropertyBuilder builder) =>
        ConfigureScalar<Cep>(builder, new CepValueConverter(), 8);

    /// <summary>Configures an e-mail property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianEmailSqlServer(this PropertyBuilder builder) =>
        ConfigureScalar<Email>(builder, new EmailValueConverter(), 254);

    /// <summary>Configures a Brazilian mobile-phone property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianMobilePhoneSqlServer(this PropertyBuilder builder) =>
        ConfigureScalar<MobilePhone>(builder, new MobilePhoneValueConverter(), 11);

    /// <summary>Configures a Brazilian landline-phone property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianLandlinePhoneSqlServer(this PropertyBuilder builder) =>
        ConfigureScalar<LandlinePhone>(builder, new LandlinePhoneValueConverter(), 10);

    /// <summary>Configures a Brazilian telephone union property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianTelefoneSqlServer(this PropertyBuilder builder) =>
        ConfigureScalar<TelefoneBrasileiro>(builder, new TelefoneBrasileiroValueConverter(), 11);

    /// <summary>Configures a Pix-key property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianChavePixSqlServer(this PropertyBuilder builder) =>
        ConfigureScalar<ChavePix>(builder, new ChavePixValueConverter(), 77);

    /// <summary>Configures a CNH property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianCnhSqlServer(this PropertyBuilder builder) =>
        ConfigureScalar<Cnh>(builder, new CnhValueConverter(), 11);

    /// <summary>Configures a CNS property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianCnsSqlServer(this PropertyBuilder builder) =>
        ConfigureScalar<Cns>(builder, new CnsValueConverter(), 15);

    /// <summary>Configures a Titulo Eleitoral property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianTituloEleitoralSqlServer(this PropertyBuilder builder) =>
        ConfigureScalar<TituloEleitoral>(builder, new TituloEleitoralValueConverter(), 12);

    /// <summary>Configures a NIT property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianNitSqlServer(this PropertyBuilder builder) =>
        ConfigureScalar<Nit>(builder, new NitValueConverter(), 11);

    /// <summary>Configures a PIS/PASEP property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianPisPasepSqlServer(this PropertyBuilder builder) =>
        ConfigureScalar<PisPasep>(builder, new PisPasepValueConverter(), 11);

    /// <summary>Configures a Brazilian vehicle-plate property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianPlacaVeiculoSqlServer(this PropertyBuilder builder) =>
        ConfigureScalar<PlacaVeiculo>(builder, new PlacaVeiculoValueConverter(), 7);

    /// <summary>Configures a RENAVAM property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianRenavamSqlServer(this PropertyBuilder builder) =>
        ConfigureScalar<Renavam>(builder, new RenavamValueConverter(), 11);

    /// <summary>Configures an ISPB property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianIspbSqlServer(this PropertyBuilder builder) =>
        ConfigureScalar<Ispb>(builder, new IspbValueConverter(), 8);

    /// <summary>Configures a COMPE code property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianCodigoCompeSqlServer(this PropertyBuilder builder) =>
        ConfigureScalar<CodigoCompe>(builder, new CodigoCompeValueConverter(), 3);

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
        ConfigureScalar<Rg>(builder, new RgValueConverter(), 10);

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
        ConfigureScalar<InscricaoEstadual>(builder, new InscricaoEstadualValueConverter(), 14);

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

    private static PropertyBuilder ConfigureScalar<TPrimitive>(
        PropertyBuilder builder,
        ValueConverter converter,
        int maxLength)
        where TPrimitive : struct
    {
        ArgumentNullException.ThrowIfNull(builder);

        Type configuredType = Nullable.GetUnderlyingType(builder.Metadata.ClrType) ?? builder.Metadata.ClrType;
        if (configuredType != typeof(TPrimitive))
        {
            throw new InvalidOperationException(
                $"The SQL Server mapping for {typeof(TPrimitive).Name} cannot be applied to property type {builder.Metadata.ClrType.Name}.");
        }

        builder.HasConversion(converter)
            .HasMaxLength(maxLength)
            .IsUnicode(false);

        return builder;
    }
}
