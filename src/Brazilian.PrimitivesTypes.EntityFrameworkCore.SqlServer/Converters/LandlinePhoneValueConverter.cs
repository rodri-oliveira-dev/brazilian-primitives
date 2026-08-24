using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

/// <summary>
/// Converts <see cref="LandlinePhone"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class LandlinePhoneValueConverter : ValueConverter<LandlinePhone, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LandlinePhoneValueConverter"/> class.
    /// </summary>
    public LandlinePhoneValueConverter()
        : base(value => value.Value, value => LandlinePhone.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(10))
    {
    }
}
