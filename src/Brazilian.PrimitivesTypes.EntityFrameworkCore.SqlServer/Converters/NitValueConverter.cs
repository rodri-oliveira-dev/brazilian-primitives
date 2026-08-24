using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

/// <summary>
/// Converts <see cref="Nit"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class NitValueConverter : ValueConverter<Nit, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NitValueConverter"/> class.
    /// </summary>
    public NitValueConverter()
        : base(value => value.Value, value => Nit.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(11))
    {
    }
}
