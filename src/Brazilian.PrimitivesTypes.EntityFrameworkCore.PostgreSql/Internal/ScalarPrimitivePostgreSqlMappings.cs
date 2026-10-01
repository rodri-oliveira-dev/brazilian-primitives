using Brazilian.PrimitivesTypes;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.PostgreSql;

internal static class ScalarPrimitivePostgreSqlMappings
{
    public static readonly ScalarPrimitivePostgreSqlMapping<Cpf, CpfValueConverter> Cpf = new(11);
    public static readonly ScalarPrimitivePostgreSqlMapping<Cnpj, CnpjValueConverter> Cnpj = new(14);
    public static readonly ScalarPrimitivePostgreSqlMapping<CpfCnpj, CpfCnpjValueConverter> CpfCnpj = new(14);
    public static readonly ScalarPrimitivePostgreSqlMapping<Cep, CepValueConverter> Cep = new(8);
    public static readonly ScalarPrimitivePostgreSqlMapping<Email, EmailValueConverter> Email = new(254);
    public static readonly ScalarPrimitivePostgreSqlMapping<MobilePhone, MobilePhoneValueConverter> MobilePhone = new(11);
    public static readonly ScalarPrimitivePostgreSqlMapping<LandlinePhone, LandlinePhoneValueConverter> LandlinePhone = new(10);
    public static readonly ScalarPrimitivePostgreSqlMapping<TelefoneBrasileiro, TelefoneBrasileiroValueConverter> TelefoneBrasileiro = new(11);
    public static readonly ScalarPrimitivePostgreSqlMapping<ChavePix, ChavePixValueConverter> ChavePix = new(77);
    public static readonly ScalarPrimitivePostgreSqlMapping<Cnh, CnhValueConverter> Cnh = new(11);
    public static readonly ScalarPrimitivePostgreSqlMapping<Cns, CnsValueConverter> Cns = new(15);
    public static readonly ScalarPrimitivePostgreSqlMapping<TituloEleitoral, TituloEleitoralValueConverter> TituloEleitoral = new(12);
    public static readonly ScalarPrimitivePostgreSqlMapping<Nit, NitValueConverter> Nit = new(11);
    public static readonly ScalarPrimitivePostgreSqlMapping<PisPasep, PisPasepValueConverter> PisPasep = new(11);
    public static readonly ScalarPrimitivePostgreSqlMapping<PlacaVeiculo, PlacaVeiculoValueConverter> PlacaVeiculo = new(7);
    public static readonly ScalarPrimitivePostgreSqlMapping<Renavam, RenavamValueConverter> Renavam = new(11);
    public static readonly ScalarPrimitivePostgreSqlMapping<Ispb, IspbValueConverter> Ispb = new(8);
    public static readonly ScalarPrimitivePostgreSqlMapping<CodigoCompe, CodigoCompeValueConverter> CodigoCompe = new(3);
}
