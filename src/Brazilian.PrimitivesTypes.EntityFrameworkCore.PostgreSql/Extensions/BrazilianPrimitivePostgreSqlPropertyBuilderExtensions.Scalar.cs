using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;

/// <summary>
/// Provides explicit PostgreSQL fluent mappings for Brazilian primitive value objects.
/// </summary>
/// <remarks>
/// These extensions are opt-in and do not create indexes, keys, uniqueness constraints, or aggregate-specific rules.
/// Normal EF Core configuration can be chained after them to override column names, types, lengths, and required/optional
/// metadata where the underlying CLR type permits it.
/// </remarks>
public static partial class BrazilianPrimitivePostgreSqlPropertyBuilderExtensions
{
    /// <summary>Configures a Cpf property for canonical PostgreSQL persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianCpfPostgreSql(this PropertyBuilder builder) =>
        ScalarPrimitivePostgreSqlMappings.Cpf.Apply(builder);

    /// <summary>Configures a Cnpj property for canonical PostgreSQL persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianCnpjPostgreSql(this PropertyBuilder builder) =>
        ScalarPrimitivePostgreSqlMappings.Cnpj.Apply(builder);

    /// <summary>Configures a CPF/CNPJ union property for canonical PostgreSQL persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianCpfCnpjPostgreSql(this PropertyBuilder builder) =>
        ScalarPrimitivePostgreSqlMappings.CpfCnpj.Apply(builder);

    /// <summary>Configures a CEP property for canonical PostgreSQL persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianCepPostgreSql(this PropertyBuilder builder) =>
        ScalarPrimitivePostgreSqlMappings.Cep.Apply(builder);

    /// <summary>Configures a e-mail property for canonical PostgreSQL persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianEmailPostgreSql(this PropertyBuilder builder) =>
        ScalarPrimitivePostgreSqlMappings.Email.Apply(builder);

    /// <summary>Configures a Brazilian mobile-phone property for canonical PostgreSQL persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianMobilePhonePostgreSql(this PropertyBuilder builder) =>
        ScalarPrimitivePostgreSqlMappings.MobilePhone.Apply(builder);

    /// <summary>Configures a Brazilian landline-phone property for canonical PostgreSQL persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianLandlinePhonePostgreSql(this PropertyBuilder builder) =>
        ScalarPrimitivePostgreSqlMappings.LandlinePhone.Apply(builder);

    /// <summary>Configures a Brazilian telephone union property for canonical PostgreSQL persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianTelefonePostgreSql(this PropertyBuilder builder) =>
        ScalarPrimitivePostgreSqlMappings.TelefoneBrasileiro.Apply(builder);

    /// <summary>Configures a Pix-key property for canonical PostgreSQL persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianChavePixPostgreSql(this PropertyBuilder builder) =>
        ScalarPrimitivePostgreSqlMappings.ChavePix.Apply(builder);

    /// <summary>Configures a CNH property for canonical PostgreSQL persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianCnhPostgreSql(this PropertyBuilder builder) =>
        ScalarPrimitivePostgreSqlMappings.Cnh.Apply(builder);

    /// <summary>Configures a CNS property for canonical PostgreSQL persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianCnsPostgreSql(this PropertyBuilder builder) =>
        ScalarPrimitivePostgreSqlMappings.Cns.Apply(builder);

    /// <summary>Configures a Titulo Eleitoral property for canonical PostgreSQL persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianTituloEleitoralPostgreSql(this PropertyBuilder builder) =>
        ScalarPrimitivePostgreSqlMappings.TituloEleitoral.Apply(builder);

    /// <summary>Configures a NIT property for canonical PostgreSQL persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianNitPostgreSql(this PropertyBuilder builder) =>
        ScalarPrimitivePostgreSqlMappings.Nit.Apply(builder);

    /// <summary>Configures a PIS/PASEP property for canonical PostgreSQL persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianPisPasepPostgreSql(this PropertyBuilder builder) =>
        ScalarPrimitivePostgreSqlMappings.PisPasep.Apply(builder);

    /// <summary>Configures a Brazilian vehicle-plate property for canonical PostgreSQL persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianPlacaVeiculoPostgreSql(this PropertyBuilder builder) =>
        ScalarPrimitivePostgreSqlMappings.PlacaVeiculo.Apply(builder);

    /// <summary>Configures a RENAVAM property for canonical PostgreSQL persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianRenavamPostgreSql(this PropertyBuilder builder) =>
        ScalarPrimitivePostgreSqlMappings.Renavam.Apply(builder);

    /// <summary>Configures a ISPB property for canonical PostgreSQL persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianIspbPostgreSql(this PropertyBuilder builder) =>
        ScalarPrimitivePostgreSqlMappings.Ispb.Apply(builder);

    /// <summary>Configures a COMPE code property for canonical PostgreSQL persistence.</summary>
    /// <param name="builder">The property builder.</param>
    /// <returns>The same property builder.</returns>
    public static PropertyBuilder HasBrazilianCodigoCompePostgreSql(this PropertyBuilder builder) =>
        ScalarPrimitivePostgreSqlMappings.CodigoCompe.Apply(builder);

}
