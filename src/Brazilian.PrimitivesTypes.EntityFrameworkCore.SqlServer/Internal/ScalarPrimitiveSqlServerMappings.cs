using Brazilian.PrimitivesTypes;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

internal static class ScalarPrimitiveSqlServerMappings
{
    public static readonly ScalarPrimitiveSqlServerMapping<Cpf, CpfValueConverter> Cpf = new(11);
    public static readonly ScalarPrimitiveSqlServerMapping<Cnpj, CnpjValueConverter> Cnpj = new(14);
    public static readonly ScalarPrimitiveSqlServerMapping<CpfCnpj, CpfCnpjValueConverter> CpfCnpj = new(14);
    public static readonly ScalarPrimitiveSqlServerMapping<Cep, CepValueConverter> Cep = new(8);
    public static readonly ScalarPrimitiveSqlServerMapping<Email, EmailValueConverter> Email = new(254);
    public static readonly ScalarPrimitiveSqlServerMapping<MobilePhone, MobilePhoneValueConverter> MobilePhone = new(11);
    public static readonly ScalarPrimitiveSqlServerMapping<LandlinePhone, LandlinePhoneValueConverter> LandlinePhone = new(10);
    public static readonly ScalarPrimitiveSqlServerMapping<TelefoneBrasileiro, TelefoneBrasileiroValueConverter> TelefoneBrasileiro = new(11);
    public static readonly ScalarPrimitiveSqlServerMapping<ChavePix, ChavePixValueConverter> ChavePix = new(77);
    public static readonly ScalarPrimitiveSqlServerMapping<Cnh, CnhValueConverter> Cnh = new(11);
    public static readonly ScalarPrimitiveSqlServerMapping<Cns, CnsValueConverter> Cns = new(15);
    public static readonly ScalarPrimitiveSqlServerMapping<TituloEleitoral, TituloEleitoralValueConverter> TituloEleitoral = new(12);
    public static readonly ScalarPrimitiveSqlServerMapping<Nit, NitValueConverter> Nit = new(11);
    public static readonly ScalarPrimitiveSqlServerMapping<PisPasep, PisPasepValueConverter> PisPasep = new(11);
    public static readonly ScalarPrimitiveSqlServerMapping<PlacaVeiculo, PlacaVeiculoValueConverter> PlacaVeiculo = new(7);
    public static readonly ScalarPrimitiveSqlServerMapping<Renavam, RenavamValueConverter> Renavam = new(11);
    public static readonly ScalarPrimitiveSqlServerMapping<Ispb, IspbValueConverter> Ispb = new(8);
    public static readonly ScalarPrimitiveSqlServerMapping<CodigoCompe, CodigoCompeValueConverter> CodigoCompe = new(3);
}
