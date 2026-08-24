using System.Globalization;
using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

/// <summary>
/// Converts <see cref="CpfCnpj"/> values to and from their canonical SQL Server string representation.
/// </summary>
public sealed class CpfCnpjValueConverter : ValueConverter<CpfCnpj, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CpfCnpjValueConverter"/> class.
    /// </summary>
    public CpfCnpjValueConverter()
        : base(value => value.Value, value => CpfCnpj.Parse(value, CultureInfo.InvariantCulture), SqlServerValueConverterMappingHints.Ascii(14))
    {
    }
}
