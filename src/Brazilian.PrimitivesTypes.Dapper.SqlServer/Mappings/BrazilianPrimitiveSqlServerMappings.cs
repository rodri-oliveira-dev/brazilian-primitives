using System.Globalization;
using Brazilian.PrimitivesTypes;

namespace Brazilian.PrimitivesTypes.Dapper.SqlServer;

internal static class BrazilianPrimitiveSqlServerMappings
{
    internal static readonly BrazilianPrimitiveSqlServerMapping<Cpf> CpfMapping =
        CreateInvariant<Cpf>(11, static value => value.Value);

    internal static readonly BrazilianPrimitiveSqlServerMapping<Cnpj> CnpjMapping =
        CreateInvariant<Cnpj>(14, static value => value.Value);

    internal static readonly BrazilianPrimitiveSqlServerMapping<CpfCnpj> CpfCnpjMapping =
        CreateInvariant<CpfCnpj>(14, static value => value.Value);

    internal static readonly BrazilianPrimitiveSqlServerMapping<Cep> CepMapping =
        CreateInvariant<Cep>(8, static value => value.Value);

    internal static readonly BrazilianPrimitiveSqlServerMapping<Email> EmailMapping =
        CreateInvariant<Email>(254, static value => value.Value);

    internal static readonly BrazilianPrimitiveSqlServerMapping<MobilePhone> MobilePhoneMapping =
        CreateInvariant<MobilePhone>(11, static value => value.Value);

    internal static readonly BrazilianPrimitiveSqlServerMapping<LandlinePhone> LandlinePhoneMapping =
        CreateInvariant<LandlinePhone>(10, static value => value.Value);

    internal static readonly BrazilianPrimitiveSqlServerMapping<TelefoneBrasileiro> TelefoneBrasileiroMapping =
        CreateInvariant<TelefoneBrasileiro>(11, static value => value.Value);

    internal static readonly BrazilianPrimitiveSqlServerMapping<ChavePix> ChavePixMapping = new(
        77,
        static value => value.Value,
        ChavePixCanonicalValueParser.Parse);

    internal static readonly BrazilianPrimitiveSqlServerMapping<Cnh> CnhMapping =
        CreateInvariant<Cnh>(11, static value => value.Value);

    internal static readonly BrazilianPrimitiveSqlServerMapping<Cns> CnsMapping =
        CreateInvariant<Cns>(15, static value => value.Value);

    internal static readonly BrazilianPrimitiveSqlServerMapping<TituloEleitoral> TituloEleitoralMapping =
        CreateInvariant<TituloEleitoral>(12, static value => value.Value);

    internal static readonly BrazilianPrimitiveSqlServerMapping<Nit> NitMapping =
        CreateInvariant<Nit>(11, static value => value.Value);

    internal static readonly BrazilianPrimitiveSqlServerMapping<PisPasep> PisPasepMapping =
        CreateInvariant<PisPasep>(11, static value => value.Value);

    internal static readonly BrazilianPrimitiveSqlServerMapping<PlacaVeiculo> PlacaVeiculoMapping =
        CreateInvariant<PlacaVeiculo>(7, static value => value.Value);

    internal static readonly BrazilianPrimitiveSqlServerMapping<Renavam> RenavamMapping =
        CreateInvariant<Renavam>(11, static value => value.Value);

    internal static readonly BrazilianPrimitiveSqlServerMapping<Ispb> IspbMapping =
        CreateInvariant<Ispb>(8, static value => value.Value);

    internal static readonly BrazilianPrimitiveSqlServerMapping<CodigoCompe> CodigoCompeMapping =
        CreateInvariant<CodigoCompe>(3, static value => value.Value);

    internal static readonly BrazilianPrimitiveSqlServerMapping<Rg> RgMapping = new(
        10,
        static value => value.Value,
        Rg.Parse);

    internal static readonly BrazilianPrimitiveSqlServerMapping<InscricaoEstadual> InscricaoEstadualMapping = new(
        14,
        static value => value.Value,
        InscricaoEstadual.Parse);

    private static BrazilianPrimitiveSqlServerMapping<T> CreateInvariant<T>(
        int size,
        Func<T, string> serialize)
        where T : IParsable<T>
    {
        return new BrazilianPrimitiveSqlServerMapping<T>(
            size,
            serialize,
            static value => T.Parse(value, CultureInfo.InvariantCulture));
    }
}
