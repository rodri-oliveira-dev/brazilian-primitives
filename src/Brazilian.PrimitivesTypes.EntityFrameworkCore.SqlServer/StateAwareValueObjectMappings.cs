using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

/// <summary>
/// Converts context-free <see cref="Rg"/> values to and from a single canonical SQL Server column.
/// </summary>
/// <remarks>
/// This converter intentionally rejects an <see cref="Rg"/> that has issuing-state context so a supplied UF cannot be
/// silently discarded. Use <see cref="RgStateAwareSqlServerMapping"/> when <see cref="Rg.HasState"/> is true.
/// </remarks>
public sealed class RgValueConverter : ValueConverter<Rg, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RgValueConverter"/> class.
    /// </summary>
    public RgValueConverter()
        : base(
            value => ContextFreeStatePersistence.GetRgValue(value),
            value => Rg.Parse(value),
            SqlServerValueConverterMappingHints.Ascii(10))
    {
    }
}

/// <summary>
/// Converts context-free <see cref="InscricaoEstadual"/> values to and from a single canonical SQL Server column.
/// </summary>
/// <remarks>
/// This converter intentionally rejects an <see cref="InscricaoEstadual"/> that has state context so a supplied UF
/// cannot be silently discarded. Use <see cref="InscricaoEstadualStateAwareSqlServerMapping"/> when
/// <see cref="InscricaoEstadual.HasState"/> is true.
/// </remarks>
public sealed class InscricaoEstadualValueConverter : ValueConverter<InscricaoEstadual, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InscricaoEstadualValueConverter"/> class.
    /// </summary>
    public InscricaoEstadualValueConverter()
        : base(
            value => ContextFreeStatePersistence.GetInscricaoEstadualValue(value),
            value => InscricaoEstadual.Parse(value),
            SqlServerValueConverterMappingHints.Ascii(14))
    {
    }
}

/// <summary>
/// Converts a known <see cref="BrazilianState"/> to and from its stable two-letter federative-unit code.
/// </summary>
/// <remarks>
/// <see cref="BrazilianState.Unknown"/> is intentionally not persisted by this converter. A state-aware mapping must
/// contain a real UF, while context-free primitives use their single-column converters instead.
/// </remarks>
public sealed class BrazilianStateCodeValueConverter : ValueConverter<BrazilianState, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BrazilianStateCodeValueConverter"/> class.
    /// </summary>
    public BrazilianStateCodeValueConverter()
        : base(
            state => BrazilianStateSqlServerCodes.ToCode(state),
            code => BrazilianStateSqlServerCodes.Parse(code),
            SqlServerValueConverterMappingHints.Ascii(2))
    {
    }
}

/// <summary>
/// Configures lossless SQL Server table-splitting for a state-aware <see cref="Rg"/> complex property.
/// </summary>
public static class RgStateAwareSqlServerMapping
{
    /// <summary>
    /// Configures the RG canonical value and issuing UF as two columns on the containing entity table.
    /// </summary>
    /// <param name="builder">The EF Core complex-property builder for the RG property.</param>
    /// <param name="valueColumnName">Optional override for the canonical RG value column name.</param>
    /// <param name="stateColumnName">Optional override for the two-letter UF column name.</param>
    public static void Configure(
        ComplexPropertyBuilder<Rg> builder,
        string? valueColumnName = null,
        string? stateColumnName = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        ComplexTypePropertyBuilder<string> valueProperty = builder.Property(value => value.Value)
            .HasColumnType("varchar(10)")
            .IsRequired();
        ApplyColumnName(valueProperty, valueColumnName);

        ComplexTypePropertyBuilder<BrazilianState> stateProperty = builder.Property(value => value.State)
            .HasConversion(new BrazilianStateCodeValueConverter())
            .HasColumnType("varchar(2)")
            .IsRequired();
        ApplyColumnName(stateProperty, stateColumnName);
    }

    private static void ApplyColumnName<TProperty>(
        ComplexTypePropertyBuilder<TProperty> propertyBuilder,
        string? columnName)
    {
        if (columnName is null)
        {
            return;
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);
        propertyBuilder.HasColumnName(columnName);
    }
}

/// <summary>
/// Configures lossless SQL Server table-splitting for a state-aware <see cref="InscricaoEstadual"/> complex property.
/// </summary>
public static class InscricaoEstadualStateAwareSqlServerMapping
{
    /// <summary>
    /// Configures the canonical state-registration value and UF as two columns on the containing entity table.
    /// </summary>
    /// <param name="builder">The EF Core complex-property builder for the state-registration property.</param>
    /// <param name="valueColumnName">Optional override for the canonical registration value column name.</param>
    /// <param name="stateColumnName">Optional override for the two-letter UF column name.</param>
    public static void Configure(
        ComplexPropertyBuilder<InscricaoEstadual> builder,
        string? valueColumnName = null,
        string? stateColumnName = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        ComplexTypePropertyBuilder<string> valueProperty = builder.Property(value => value.Value)
            .HasColumnType("varchar(14)")
            .IsRequired();
        ApplyColumnName(valueProperty, valueColumnName);

        ComplexTypePropertyBuilder<BrazilianState> stateProperty = builder.Property(value => value.State)
            .HasConversion(new BrazilianStateCodeValueConverter())
            .HasColumnType("varchar(2)")
            .IsRequired();
        ApplyColumnName(stateProperty, stateColumnName);
    }

    private static void ApplyColumnName<TProperty>(
        ComplexTypePropertyBuilder<TProperty> propertyBuilder,
        string? columnName)
    {
        if (columnName is null)
        {
            return;
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(columnName);
        propertyBuilder.HasColumnName(columnName);
    }
}

internal static class ContextFreeStatePersistence
{
    public static string GetRgValue(Rg value)
    {
        if (value.HasState)
        {
            throw new InvalidOperationException(
                "A state-aware RG cannot be persisted with the single-column converter because that would discard its UF context.");
        }

        return value.Value;
    }

    public static string GetInscricaoEstadualValue(InscricaoEstadual value)
    {
        if (value.HasState)
        {
            throw new InvalidOperationException(
                "A state-aware Inscricao Estadual cannot be persisted with the single-column converter because that would discard its UF context.");
        }

        return value.Value;
    }
}

internal static class BrazilianStateSqlServerCodes
{
    public static string ToCode(BrazilianState state)
    {
        return state switch
        {
            BrazilianState.Acre => "AC",
            BrazilianState.Alagoas => "AL",
            BrazilianState.Amapa => "AP",
            BrazilianState.Amazonas => "AM",
            BrazilianState.Bahia => "BA",
            BrazilianState.Ceara => "CE",
            BrazilianState.DistritoFederal => "DF",
            BrazilianState.EspiritoSanto => "ES",
            BrazilianState.Goias => "GO",
            BrazilianState.Maranhao => "MA",
            BrazilianState.MatoGrosso => "MT",
            BrazilianState.MatoGrossoDoSul => "MS",
            BrazilianState.MinasGerais => "MG",
            BrazilianState.Para => "PA",
            BrazilianState.Paraiba => "PB",
            BrazilianState.Parana => "PR",
            BrazilianState.Pernambuco => "PE",
            BrazilianState.Piaui => "PI",
            BrazilianState.RioDeJaneiro => "RJ",
            BrazilianState.RioGrandeDoNorte => "RN",
            BrazilianState.RioGrandeDoSul => "RS",
            BrazilianState.Rondonia => "RO",
            BrazilianState.Roraima => "RR",
            BrazilianState.SantaCatarina => "SC",
            BrazilianState.SaoPaulo => "SP",
            BrazilianState.Sergipe => "SE",
            BrazilianState.Tocantins => "TO",
            _ => throw new InvalidOperationException("Only a known Brazilian federative unit can be persisted in a state-aware mapping."),
        };
    }

    public static BrazilianState Parse(string code)
    {
        return code switch
        {
            "AC" => BrazilianState.Acre,
            "AL" => BrazilianState.Alagoas,
            "AP" => BrazilianState.Amapa,
            "AM" => BrazilianState.Amazonas,
            "BA" => BrazilianState.Bahia,
            "CE" => BrazilianState.Ceara,
            "DF" => BrazilianState.DistritoFederal,
            "ES" => BrazilianState.EspiritoSanto,
            "GO" => BrazilianState.Goias,
            "MA" => BrazilianState.Maranhao,
            "MT" => BrazilianState.MatoGrosso,
            "MS" => BrazilianState.MatoGrossoDoSul,
            "MG" => BrazilianState.MinasGerais,
            "PA" => BrazilianState.Para,
            "PB" => BrazilianState.Paraiba,
            "PR" => BrazilianState.Parana,
            "PE" => BrazilianState.Pernambuco,
            "PI" => BrazilianState.Piaui,
            "RJ" => BrazilianState.RioDeJaneiro,
            "RN" => BrazilianState.RioGrandeDoNorte,
            "RS" => BrazilianState.RioGrandeDoSul,
            "RO" => BrazilianState.Rondonia,
            "RR" => BrazilianState.Roraima,
            "SC" => BrazilianState.SantaCatarina,
            "SP" => BrazilianState.SaoPaulo,
            "SE" => BrazilianState.Sergipe,
            "TO" => BrazilianState.Tocantins,
            _ => throw new FormatException("Persisted Brazilian state code must be a supported two-letter UF code."),
        };
    }
}
