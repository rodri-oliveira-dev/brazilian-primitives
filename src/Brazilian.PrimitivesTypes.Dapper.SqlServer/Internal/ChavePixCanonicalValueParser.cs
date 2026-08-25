using System.Globalization;
using Brazilian.PrimitivesTypes;

namespace Brazilian.PrimitivesTypes.Dapper.SqlServer;

internal static class ChavePixCanonicalValueParser
{
    internal static ChavePix Parse(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        return value switch
        {
            { Length: 14 } when value.StartsWith("+55", StringComparison.Ordinal) =>
                ChavePix.From(MobilePhone.Parse(value, CultureInfo.InvariantCulture)),
            { Length: 11 } =>
                ChavePix.From(Cpf.Parse(value, CultureInfo.InvariantCulture)),
            { Length: 14 } =>
                ChavePix.From(Cnpj.Parse(value, CultureInfo.InvariantCulture)),
            { Length: 36 } => ChavePix.FromChaveAleatoria(value),
            _ when value.Contains('@') =>
                ChavePix.From(Email.Parse(value, CultureInfo.InvariantCulture)),
            _ => throw new FormatException("Persisted Pix key is not in a supported canonical representation."),
        };
    }
}
