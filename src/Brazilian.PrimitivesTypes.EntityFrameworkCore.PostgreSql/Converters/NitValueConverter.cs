using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;

/// <summary>
/// Converts <see cref="Nit"/> values to and from their canonical PostgreSQL string representation.
/// </summary>
public sealed class NitValueConverter : ValueConverter<Nit, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NitValueConverter"/> class.
    /// </summary>
    public NitValueConverter()
        : base(value => value.Value, value => Nit.Parse(value, CultureInfo.InvariantCulture), PostgreSqlValueConverterMappingHints.String(11))
    {
    }
}
