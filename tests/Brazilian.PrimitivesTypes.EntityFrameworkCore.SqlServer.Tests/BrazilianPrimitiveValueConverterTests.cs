using Brazilian.PrimitivesTypes;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Xunit;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer.Tests;

public sealed class BrazilianPrimitiveValueConverterTests
{
    [Fact]
    public void ConvertersRoundTripCanonicalValuesAndExposeAsciiMappingHints()
    {
        foreach (ConverterCase testCase in CreateCases())
        {
            object? providerValue = testCase.Converter.ConvertToProvider(testCase.ModelValue);
            object? modelValue = testCase.Converter.ConvertFromProvider(testCase.ProviderValue);
            ConverterMappingHints mappingHints = Assert.IsType<ConverterMappingHints>(testCase.Converter.MappingHints);

            Assert.Equal(testCase.ProviderValue, providerValue);
            Assert.Equal(testCase.ModelValue, modelValue);
            Assert.Equal(testCase.MaxLength, mappingHints.Size);
            Assert.False(mappingHints.IsUnicode ?? true);
        }
    }

    [Fact]
    public void DiscriminatedPrimitivesPreserveSemanticTypeAfterRoundTrip()
    {
        CpfCnpj cpfCnpj = Assert.IsType<CpfCnpj>(
            new CpfCnpjValueConverter().ConvertFromProvider("00000000E08G12"));
        TelefoneBrasileiro telefone = Assert.IsType<TelefoneBrasileiro>(
            new TelefoneBrasileiroValueConverter().ConvertFromProvider("11987654321"));
        ChavePix chavePix = Assert.IsType<ChavePix>(
            new ChavePixValueConverter().ConvertFromProvider("11900000083"));
        PlacaVeiculo placa = Assert.IsType<PlacaVeiculo>(
            new PlacaVeiculoValueConverter().ConvertFromProvider("ABC1D23"));

        Assert.Equal(TipoCpfCnpj.Cnpj, cpfCnpj.Tipo);
        Assert.Equal(TipoTelefoneBrasileiro.Celular, telefone.Tipo);
        Assert.Equal(TipoChavePix.Cpf, chavePix.Tipo);
        Assert.Equal(PadraoPlacaVeiculo.Mercosul, placa.Padrao);
    }

    [Fact]
    public void ChavePixConverterUsesCanonicalPersistenceRepresentationToResolveAmbiguousCpf()
    {
        Cpf ambiguousCpf = Cpf.Parse("11900000083");
        Assert.True(MobilePhone.IsValid(ambiguousCpf.Value));

        ChavePix original = ChavePix.From(ambiguousCpf);
        ChavePixValueConverter converter = new();

        string providerValue = Assert.IsType<string>(converter.ConvertToProvider(original));
        ChavePix rehydrated = Assert.IsType<ChavePix>(converter.ConvertFromProvider(providerValue));

        Assert.Equal("11900000083", providerValue);
        Assert.Equal(original, rehydrated);
        Assert.Equal(TipoChavePix.Cpf, rehydrated.Tipo);
    }

    [Fact]
    public void InvalidPersistedProviderValueFailsThroughPublicParsingInvariant()
    {
        EmailValueConverter converter = new();

        Assert.Throws<FormatException>(() => converter.ConvertFromProvider("not-an-email"));
    }

    private static IEnumerable<ConverterCase> CreateCases()
    {
        yield return new ConverterCase(new CpfValueConverter(), Cpf.Parse("529.982.247-25"), "52998224725", 11);
        yield return new ConverterCase(new CnpjValueConverter(), Cnpj.Parse("00000000e08g12"), "00000000E08G12", 14);
        yield return new ConverterCase(new CpfCnpjValueConverter(), CpfCnpj.Parse("00.000.000/e08g-12"), "00000000E08G12", 14);
        yield return new ConverterCase(new CepValueConverter(), Cep.Parse("01311-000"), "01311000", 8);
        yield return new ConverterCase(new EmailValueConverter(), Email.Parse("usuario@domínio.com"), "usuario@xn--domnio-5va.com", 254);
        yield return new ConverterCase(new MobilePhoneValueConverter(), MobilePhone.Parse("(11) 98765-4321"), "11987654321", 11);
        yield return new ConverterCase(new LandlinePhoneValueConverter(), LandlinePhone.Parse("(11) 3234-5678"), "1132345678", 10);
        yield return new ConverterCase(new TelefoneBrasileiroValueConverter(), TelefoneBrasileiro.Parse("(11) 98765-4321"), "11987654321", 11);
        yield return new ConverterCase(new ChavePixValueConverter(), ChavePix.From(Cpf.Parse("11900000083")), "11900000083", 77);
        yield return new ConverterCase(new CnhValueConverter(), Cnh.Parse("02650306461"), "02650306461", 11);
        yield return new ConverterCase(new CnsValueConverter(), Cns.Parse("123456789010000"), "123456789010000", 15);
        yield return new ConverterCase(new TituloEleitoralValueConverter(), TituloEleitoral.Parse("000123450159"), "000123450159", 12);
        yield return new ConverterCase(new NitValueConverter(), Nit.Parse("00000000001"), "00000000001", 11);
        yield return new ConverterCase(new PisPasepValueConverter(), PisPasep.Parse("01234567897"), "01234567897", 11);
        yield return new ConverterCase(new PlacaVeiculoValueConverter(), PlacaVeiculo.Parse("abc1d23"), "ABC1D23", 7);
        yield return new ConverterCase(new RenavamValueConverter(), Renavam.Parse("00123456789"), "00123456789", 11);
        yield return new ConverterCase(new IspbValueConverter(), Ispb.Parse("00000001"), "00000001", 8);
        yield return new ConverterCase(new CodigoCompeValueConverter(), CodigoCompe.Parse("001"), "001", 3);
    }

    private sealed record ConverterCase(
        ValueConverter Converter,
        object ModelValue,
        string ProviderValue,
        int MaxLength);
}
