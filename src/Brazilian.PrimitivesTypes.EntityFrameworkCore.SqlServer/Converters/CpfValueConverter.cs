using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

/// <summary>
/// Converts <see cref="Cpf"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class CpfValueConverter : ValueConverter<Cpf, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CpfValueConverter"/> class.
    /// </summary>
    public CpfValueConverter()
        : base(value => value.Value, value => Cpf.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(11))
    {
    }
}
