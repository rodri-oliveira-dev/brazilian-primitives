using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

/// <summary>
/// Converts <see cref="Email"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class EmailValueConverter : ValueConverter<Email, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailValueConverter"/> class.
    /// </summary>
    public EmailValueConverter()
        : base(value => value.Value, value => Email.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(254))
    {
    }
}
