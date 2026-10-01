using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;

/// <summary>
/// Converts <see cref="TelefoneBrasileiro"/> values to and from their canonical PostgreSQL string representation.
/// </summary>
public sealed class TelefoneBrasileiroValueConverter : ValueConverter<TelefoneBrasileiro, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TelefoneBrasileiroValueConverter"/> class.
    /// </summary>
    public TelefoneBrasileiroValueConverter()
        : base(value => value.Value, value => TelefoneBrasileiro.Parse(value, CultureInfo.InvariantCulture), PostgreSqlValueConverterMappingHints.String(11))
    {
    }
}
