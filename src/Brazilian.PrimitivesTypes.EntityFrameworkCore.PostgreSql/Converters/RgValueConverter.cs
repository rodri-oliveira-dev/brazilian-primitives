using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;

/// <summary>
/// Converts context-free <see cref="Rg"/> values to and from a single canonical PostgreSQL column.
/// </summary>
/// <remarks>
/// This converter rejects an <see cref="Rg"/> that already has issuing-state context so a supplied UF cannot be
/// silently discarded. Use <see cref="RgStateAwarePostgreSqlMapping"/> when <see cref="Rg.HasState"/> is true.
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
            PostgreSqlValueConverterMappingHints.String(10))
    {
    }
}
