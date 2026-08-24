using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

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
