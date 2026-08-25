using System.Globalization;
using Brazilian.PrimitivesTypes;

namespace Brazilian.PrimitivesTypes.Dapper.SqlServer;

internal static class ChavePixCanonicalValueParser
{
    internal static ChavePix Parse(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (value.StartsWith("+55", StringComparison.Ordinal))
        {
            return ChavePix.From(MobilePhone.Parse(value, CultureInfo.InvariantCulture));
        }

        if (value.Contains('@'))
        {
            return ChavePix.From(Email.Parse(value, CultureInfo.InvariantCulture));
        }

        if (value.Length == 11)
        {
            return ChavePix.From(Cpf.Parse(value, CultureInfo.InvariantCulture));
        }

        if (value.Length == 14)
        {
            return ChavePix.From(Cnpj.Parse(value, CultureInfo.InvariantCulture));
        }

        if (value.Length == 36)
        {
            return ChavePix.FromChaveAleatoria(value);
        }

        throw new FormatException("Persisted Pix key is not in a supported canonical representation.");
    }
}
