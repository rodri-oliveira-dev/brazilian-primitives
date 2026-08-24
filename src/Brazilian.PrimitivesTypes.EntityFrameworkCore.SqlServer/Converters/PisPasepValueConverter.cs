using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

/// <summary>
/// Converts <see cref="PisPasep"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class PisPasepValueConverter : ValueConverter<PisPasep, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PisPasepValueConverter"/> class.
    /// </summary>
    public PisPasepValueConverter()
        : base(value => value.Value, value => PisPasep.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(11))
    {
    }
}
