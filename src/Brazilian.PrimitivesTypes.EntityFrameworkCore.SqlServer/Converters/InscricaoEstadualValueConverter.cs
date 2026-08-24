using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

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
