using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

/// <summary>
/// Converts <see cref="PlacaVeiculo"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class PlacaVeiculoValueConverter : ValueConverter<PlacaVeiculo, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlacaVeiculoValueConverter"/> class.
    /// </summary>
    public PlacaVeiculoValueConverter()
        : base(value => value.Value, value => PlacaVeiculo.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(7))
    {
    }
}
