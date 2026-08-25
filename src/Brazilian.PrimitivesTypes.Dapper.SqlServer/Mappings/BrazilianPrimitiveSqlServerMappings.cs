using System.Globalization;
using Brazilian.PrimitivesTypes;

namespace Brazilian.PrimitivesTypes.Dapper.SqlServer;

internal static class BrazilianPrimitiveSqlServerMappings
{
    internal static readonly BrazilianPrimitiveSqlServerMapping<Cpf> CpfMapping = new(
        11,
        static value => value.Value,
        static value => Cpf.Parse(value, CultureInfo.InvariantCulture));

    internal static readonly BrazilianPrimitiveSqlServerMapping<Cnpj> CnpjMapping = new(
        14,
        static value => value.Value,
        static value => Cnpj.Parse(value, CultureInfo.InvariantCulture));

    internal static readonly BrazilianPrimitiveSqlServerMapping<CpfCnpj> CpfCnpjMapping = new(
        14,
        static value => value.Value,
        static value => CpfCnpj.Parse(value, CultureInfo.InvariantCulture));

    internal static readonly BrazilianPrimitiveSqlServerMapping<Cep> CepMapping = new(
        8,
        static value => value.Value,
        static value => Cep.Parse(value, CultureInfo.InvariantCulture));

    internal static readonly BrazilianPrimitiveSqlServerMapping<Email> EmailMapping = new(
        254,
        static value => value.Value,
        static value => Email.Parse(value, CultureInfo.InvariantCulture));

    internal static readonly BrazilianPrimitiveSqlServerMapping<MobilePhone> MobilePhoneMapping = new(
        11,
        static value => value.Value,
        static value => MobilePhone.Parse(value, CultureInfo.InvariantCulture));

    internal static readonly BrazilianPrimitiveSqlServerMapping<LandlinePhone> LandlinePhoneMapping = new(
        10,
        static value => value.Value,
        static value => LandlinePhone.Parse(value, CultureInfo.InvariantCulture));

    internal static readonly BrazilianPrimitiveSqlServerMapping<TelefoneBrasileiro> TelefoneBrasileiroMapping = new(
        11,
        static value => value.Value,
        static value => TelefoneBrasileiro.Parse(value, CultureInfo.InvariantCulture));

    internal static readonly BrazilianPrimitiveSqlServerMapping<ChavePix> ChavePixMapping = new(
        77,
        static value => value.Value,
        ChavePixCanonicalValueParser.Parse);

    internal static readonly BrazilianPrimitiveSqlServerMapping<Cnh> CnhMapping = new(
        11,
        static value => value.Value,
        static value => Cnh.Parse(value, CultureInfo.InvariantCulture));

    internal static readonly BrazilianPrimitiveSqlServerMapping<Cns> CnsMapping = new(
        15,
        static value => value.Value,
        static value => Cns.Parse(value, CultureInfo.InvariantCulture));

    internal static readonly BrazilianPrimitiveSqlServerMapping<TituloEleitoral> TituloEleitoralMapping = new(
        12,
        static value => value.Value,
        static value => TituloEleitoral.Parse(value, CultureInfo.InvariantCulture));

    internal static readonly BrazilianPrimitiveSqlServerMapping<Nit> NitMapping = new(
        11,
        static value => value.Value,
        static value => Nit.Parse(value, CultureInfo.InvariantCulture));

    internal static readonly BrazilianPrimitiveSqlServerMapping<PisPasep> PisPasepMapping = new(
        11,
        static value => value.Value,
        static value => PisPasep.Parse(value, CultureInfo.InvariantCulture));

    internal static readonly BrazilianPrimitiveSqlServerMapping<PlacaVeiculo> PlacaVeiculoMapping = new(
        7,
        static value => value.Value,
        static value => PlacaVeiculo.Parse(value, CultureInfo.InvariantCulture));

    internal static readonly BrazilianPrimitiveSqlServerMapping<Renavam> RenavamMapping = new(
        11,
        static value => value.Value,
        static value => Renavam.Parse(value, CultureInfo.InvariantCulture));

    internal static readonly BrazilianPrimitiveSqlServerMapping<Ispb> IspbMapping = new(
        8,
        static value => value.Value,
        static value => Ispb.Parse(value, CultureInfo.InvariantCulture));

    internal static readonly BrazilianPrimitiveSqlServerMapping<CodigoCompe> CodigoCompeMapping = new(
        3,
        static value => value.Value,
        static value => CodigoCompe.Parse(value, CultureInfo.InvariantCulture));

    internal static readonly BrazilianPrimitiveSqlServerMapping<Rg> RgMapping = new(
        10,
        static value => value.Value,
        Rg.Parse);

    internal static readonly BrazilianPrimitiveSqlServerMapping<InscricaoEstadual> InscricaoEstadualMapping = new(
        14,
        static value => value.Value,
        InscricaoEstadual.Parse);
}
