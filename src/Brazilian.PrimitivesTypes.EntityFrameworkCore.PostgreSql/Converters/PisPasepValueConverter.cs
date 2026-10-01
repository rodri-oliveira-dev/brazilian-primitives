using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;

/// <summary>
/// Converts <see cref="PisPasep"/> values to and from their canonical PostgreSQL string representation.
/// </summary>
public sealed class PisPasepValueConverter : ValueConverter<PisPasep, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PisPasepValueConverter"/> class.
    /// </summary>
    public PisPasepValueConverter()
        : base(value => value.Value, value => PisPasep.Parse(value, CultureInfo.InvariantCulture), PostgreSqlValueConverterMappingHints.String(11))
    {
    }
}
