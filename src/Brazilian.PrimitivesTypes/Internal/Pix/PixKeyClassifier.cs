namespace Brazilian.PrimitivesTypes;

internal static class PixKeyClassifier
{
    internal static bool TryClassify(ReadOnlySpan<char> value, IFormatProvider? provider, out ChavePix result)
    {
        if (value.Contains('@'))
        {
            if (Email.TryParse(value, provider, out Email email))
            {
                return ChavePix.TryCreateFromEmail(email, out result);
            }

            result = default;
            return false;
        }

        int matchCount = 0;
        ChavePix candidate = default;

        if (MobilePhone.TryParse(value, provider, out MobilePhone celular))
        {
            candidate = ChavePix.From(celular);
            matchCount++;
        }

        if (PixRandomKeyNormalizer.TryNormalize(value, out string chaveAleatoria))
        {
            candidate = ChavePix.Create(chaveAleatoria, TipoChavePix.Aleatoria);
            matchCount++;
        }

        bool isCpf = Cpf.TryParse(value, provider, out Cpf cpf);
        bool isCnpj = Cnpj.TryParse(value, provider, out Cnpj cnpj);

        if (isCpf)
        {
            candidate = ChavePix.From(cpf);
            matchCount++;
        }

        if (isCnpj)
        {
            candidate = ChavePix.From(cnpj);
            matchCount++;
        }

        if (matchCount != 1)
        {
            result = default;
            return false;
        }

        result = candidate;
        return true;
    }
}
