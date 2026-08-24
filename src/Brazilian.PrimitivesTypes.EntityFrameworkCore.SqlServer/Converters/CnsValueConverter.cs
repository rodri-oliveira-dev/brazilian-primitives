using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

/// <summary>
/// Converts <see cref="Cns"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class CnsValueConverter : ValueConverter<Cns, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CnsValueConverter"/> class.
    /// </summary>
    public CnsValueConverter()
        : base(value => value.Value, value => Cns.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(15))
    {
    }
}
