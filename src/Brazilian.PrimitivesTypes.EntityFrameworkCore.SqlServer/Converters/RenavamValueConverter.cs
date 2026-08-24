using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

/// <summary>
/// Converts <see cref="Renavam"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class RenavamValueConverter : ValueConverter<Renavam, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RenavamValueConverter"/> class.
    /// </summary>
    public RenavamValueConverter()
        : base(value => value.Value, value => Renavam.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(11))
    {
    }
}
