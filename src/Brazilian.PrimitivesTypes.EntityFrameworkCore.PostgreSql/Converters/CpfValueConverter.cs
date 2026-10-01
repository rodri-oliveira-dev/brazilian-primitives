using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;

/// <summary>
/// Converts <see cref="Cpf"/> values to and from their canonical PostgreSQL string representation.
/// </summary>
public sealed class CpfValueConverter : ValueConverter<Cpf, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CpfValueConverter"/> class.
    /// </summary>
    public CpfValueConverter()
        : base(value => value.Value, value => Cpf.Parse(value, CultureInfo.InvariantCulture), PostgreSqlValueConverterMappingHints.String(11))
    {
    }
}
