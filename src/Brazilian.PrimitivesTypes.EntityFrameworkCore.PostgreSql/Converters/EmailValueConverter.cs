using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;

/// <summary>
/// Converts <see cref="Email"/> values to and from their canonical PostgreSQL string representation.
/// </summary>
public sealed class EmailValueConverter : ValueConverter<Email, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EmailValueConverter"/> class.
    /// </summary>
    public EmailValueConverter()
        : base(value => value.Value, value => Email.Parse(value, CultureInfo.InvariantCulture), PostgreSqlValueConverterMappingHints.String(254))
    {
    }
}
