using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

/// <summary>
/// Provides explicit SQL Server fluent mappings for Brazilian primitive value objects.
/// </summary>
/// <remarks>
/// These extensions are opt-in and do not create indexes, keys, uniqueness constraints, or aggregate-specific rules.
/// Normal EF Core configuration can be chained after them to override column names, types, lengths, Unicode settings,
/// and required/optional metadata where the underlying CLR type permits it.
/// </remarks>
public static partial class BrazilianPrimitiveSqlServerPropertyBuilderExtensions
{
    /// <summary>Configures a CPF property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianCpfSqlServer(this PropertyBuilder builder) =>
        ScalarPrimitiveSqlServerMappings.Cpf.Apply(builder);

    /// <summary>Configures a CNPJ property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianCnpjSqlServer(this PropertyBuilder builder) =>
        ScalarPrimitiveSqlServerMappings.Cnpj.Apply(builder);

    /// <summary>Configures a CPF/CNPJ union property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianCpfCnpjSqlServer(this PropertyBuilder builder) =>
        ScalarPrimitiveSqlServerMappings.CpfCnpj.Apply(builder);

    /// <summary>Configures a CEP property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianCepSqlServer(this PropertyBuilder builder) =>
        ScalarPrimitiveSqlServerMappings.Cep.Apply(builder);

    /// <summary>Configures an e-mail property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianEmailSqlServer(this PropertyBuilder builder) =>
        ScalarPrimitiveSqlServerMappings.Email.Apply(builder);

    /// <summary>Configures a Brazilian mobile-phone property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianMobilePhoneSqlServer(this PropertyBuilder builder) =>
        ScalarPrimitiveSqlServerMappings.MobilePhone.Apply(builder);

    /// <summary>Configures a Brazilian landline-phone property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianLandlinePhoneSqlServer(this PropertyBuilder builder) =>
        ScalarPrimitiveSqlServerMappings.LandlinePhone.Apply(builder);

    /// <summary>Configures a Brazilian telephone union property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianTelefoneSqlServer(this PropertyBuilder builder) =>
        ScalarPrimitiveSqlServerMappings.TelefoneBrasileiro.Apply(builder);

    /// <summary>Configures a Pix-key property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianChavePixSqlServer(this PropertyBuilder builder) =>
        ScalarPrimitiveSqlServerMappings.ChavePix.Apply(builder);

    /// <summary>Configures a CNH property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianCnhSqlServer(this PropertyBuilder builder) =>
        ScalarPrimitiveSqlServerMappings.Cnh.Apply(builder);

    /// <summary>Configures a CNS property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianCnsSqlServer(this PropertyBuilder builder) =>
        ScalarPrimitiveSqlServerMappings.Cns.Apply(builder);

    /// <summary>Configures a Titulo Eleitoral property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianTituloEleitoralSqlServer(this PropertyBuilder builder) =>
        ScalarPrimitiveSqlServerMappings.TituloEleitoral.Apply(builder);

    /// <summary>Configures a NIT property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianNitSqlServer(this PropertyBuilder builder) =>
        ScalarPrimitiveSqlServerMappings.Nit.Apply(builder);

    /// <summary>Configures a PIS/PASEP property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianPisPasepSqlServer(this PropertyBuilder builder) =>
        ScalarPrimitiveSqlServerMappings.PisPasep.Apply(builder);

    /// <summary>Configures a Brazilian vehicle-plate property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianPlacaVeiculoSqlServer(this PropertyBuilder builder) =>
        ScalarPrimitiveSqlServerMappings.PlacaVeiculo.Apply(builder);

    /// <summary>Configures a RENAVAM property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianRenavamSqlServer(this PropertyBuilder builder) =>
        ScalarPrimitiveSqlServerMappings.Renavam.Apply(builder);

    /// <summary>Configures an ISPB property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianIspbSqlServer(this PropertyBuilder builder) =>
        ScalarPrimitiveSqlServerMappings.Ispb.Apply(builder);

    /// <summary>Configures a COMPE code property for canonical SQL Server persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianCodigoCompeSqlServer(this PropertyBuilder builder) =>
        ScalarPrimitiveSqlServerMappings.CodigoCompe.Apply(builder);
}
