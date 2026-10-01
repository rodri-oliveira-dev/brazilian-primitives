using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;

/// <summary>
/// Converts <see cref="ChavePix"/> values to and from their canonical PostgreSQL string representation.
/// </summary>
/// <remarks>
/// Pix mobile keys are persisted in E.164 form so the canonical provider value retains the original key discriminator.
/// </remarks>
public sealed class ChavePixValueConverter : ValueConverter<ChavePix, string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ChavePixValueConverter"/> class.
    /// </summary>
    public ChavePixValueConverter()
        : base(value => value.Value, value => ChavePixCanonicalValueParser.Parse(value), PostgreSqlValueConverterMappingHints.String(77))
    {
    }
}
