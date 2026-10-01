using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;

/// <summary>
/// Converts <see cref="Ispb"/> values to and from their canonical PostgreSQL string representation.
/// </summary>
public sealed class IspbValueConverter : ValueConverter<Ispb, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="IspbValueConverter"/> class.
    /// </summary>
    public IspbValueConverter()
        : base(value => value.Value, value => Ispb.Parse(value, CultureInfo.InvariantCulture), PostgreSqlValueConverterMappingHints.String(8))
    {
    }
}
