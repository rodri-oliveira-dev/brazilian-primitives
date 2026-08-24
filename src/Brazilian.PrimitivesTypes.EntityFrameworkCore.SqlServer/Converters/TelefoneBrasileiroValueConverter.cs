using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

/// <summary>
/// Converts <see cref="TelefoneBrasileiro"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class TelefoneBrasileiroValueConverter : ValueConverter<TelefoneBrasileiro, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TelefoneBrasileiroValueConverter"/> class.
    /// </summary>
    public TelefoneBrasileiroValueConverter()
        : base(value => value.Value, value => TelefoneBrasileiro.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(11))
    {
    }
}
