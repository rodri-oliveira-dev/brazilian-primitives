using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;

/// <summary>
/// Converts <see cref="Cnpj"/> values to and from their canonical PostgreSQL string representation.
/// </summary>
public sealed class CnpjValueConverter : ValueConverter<Cnpj, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CnpjValueConverter"/> class.
    /// </summary>
    public CnpjValueConverter()
        : base(value => value.Value, value => Cnpj.Parse(value, CultureInfo.InvariantCulture), PostgreSqlValueConverterMappingHints.String(14))
    {
    }
}
