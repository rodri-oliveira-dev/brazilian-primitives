using Brazilian.PrimitivesTypes;
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
