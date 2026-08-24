using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

/// <summary>
/// Converts <see cref="MobilePhone"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class MobilePhoneValueConverter : ValueConverter<MobilePhone, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MobilePhoneValueConverter"/> class.
    /// </summary>
    public MobilePhoneValueConverter()
        : base(value => value.Value, value => MobilePhone.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(11))
    {
    }
}
