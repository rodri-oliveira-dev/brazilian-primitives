using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

/// <summary>
/// Converts <see cref="Cep"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class CepValueConverter : ValueConverter<Cep, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CepValueConverter"/> class.
    /// </summary>
    public CepValueConverter()
        : base(value => value.Value, value => Cep.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(8))
    {
    }
}
