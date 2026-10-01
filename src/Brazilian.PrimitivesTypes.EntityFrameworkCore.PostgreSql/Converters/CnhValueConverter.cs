using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;

/// <summary>
/// Converts <see cref="Cnh"/> values to and from their canonical PostgreSQL string representation.
/// </summary>
public sealed class CnhValueConverter : ValueConverter<Cnh, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CnhValueConverter"/> class.
    /// </summary>
    public CnhValueConverter()
        : base(value => value.Value, value => Cnh.Parse(value, CultureInfo.InvariantCulture), PostgreSqlValueConverterMappingHints.String(11))
    {
    }
}
