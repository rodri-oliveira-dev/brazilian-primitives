using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

/// <summary>
/// Converts <see cref="TituloEleitoral"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class TituloEleitoralValueConverter : ValueConverter<TituloEleitoral, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TituloEleitoralValueConverter"/> class.
    /// </summary>
    public TituloEleitoralValueConverter()
        : base(value => value.Value, value => TituloEleitoral.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(12))
    {
    }
}
