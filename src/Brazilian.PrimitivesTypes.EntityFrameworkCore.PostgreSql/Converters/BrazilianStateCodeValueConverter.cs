using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;

/// <summary>
/// Converts a known <see cref="BrazilianState"/> to and from its stable two-letter federative-unit code.
/// </summary>
/// <remarks>
/// <see cref="BrazilianState.Unknown"/> is intentionally not persisted by this converter. State-aware mappings require
/// a real UF; context-free RG and Inscricao Estadual values use their single-column converters instead.
/// </remarks>
public sealed class BrazilianStateCodeValueConverter : ValueConverter<BrazilianState, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BrazilianStateCodeValueConverter"/> class.
    /// </summary>
    public BrazilianStateCodeValueConverter()
        : base(
            state => BrazilianStatePostgreSqlCodes.ToCode(state),
            code => BrazilianStatePostgreSqlCodes.Parse(code),
            PostgreSqlValueConverterMappingHints.String(2))
    {
    }
}
