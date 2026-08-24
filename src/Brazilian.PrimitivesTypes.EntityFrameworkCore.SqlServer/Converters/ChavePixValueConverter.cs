using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

/// <summary>
/// Converts <see cref="ChavePix"/> values to and from their canonical SQL Server string representation.
/// </summary>
/// <remarks>
/// Pix mobile keys are persisted in E.164 form. This lets canonical persistence distinguish a mobile key from a CPF
/// even when the original national phone digits are also a mathematically valid CPF.
/// </remarks>
public sealed class ChavePixValueConverter : ValueConverter<ChavePix, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChavePixValueConverter"/> class.
    /// </summary>
    public ChavePixValueConverter()
        : base(value => value.Value, value => ChavePixCanonicalValueParser.Parse(value), SqlServerValueConverterMappingHints.Ascii(77))
    {
    }
}
