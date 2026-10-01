using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;

/// <summary>
/// Converts <see cref="Cep"/> values to and from their canonical PostgreSQL string representation.
/// </summary>
public sealed class CepValueConverter : ValueConverter<Cep, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CepValueConverter"/> class.
    /// </summary>
    public CepValueConverter()
        : base(value => value.Value, value => Cep.Parse(value, CultureInfo.InvariantCulture), PostgreSqlValueConverterMappingHints.String(8))
    {
    }
}
