using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;

/// <summary>
/// Converts <see cref="CodigoCompe"/> values to and from their canonical PostgreSQL string representation.
/// </summary>
public sealed class CodigoCompeValueConverter : ValueConverter<CodigoCompe, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CodigoCompeValueConverter"/> class.
    /// </summary>
    public CodigoCompeValueConverter()
        : base(value => value.Value, value => CodigoCompe.Parse(value, CultureInfo.InvariantCulture), PostgreSqlValueConverterMappingHints.String(3))
    {
    }
}
