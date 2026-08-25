using System.Data;
using System.Globalization;
using Brazilian.PrimitivesTypes;
using Brazilian.PrimitivesTypes.Dapper.SqlServer.Tests.Infrastructure;
using Xunit;

namespace Brazilian.PrimitivesTypes.Dapper.SqlServer.Tests;

public sealed class BrazilianPrimitiveTypeHandlerTests
{
    [Fact]
    public void ScalarHandlersWriteAnsiSizedCanonicalValuesAndRoundTrip()
    {
        foreach (IHandlerCase testCase in CreateCases())
        {
            testCase.AssertRoundTrip();
        }
    }

    [Fact]
    public void InvalidPersistedValueFailsThroughDomainParser()
    {
        BrazilianPrimitiveTypeHandler<Email> handler = BrazilianPrimitiveSqlServerMappings.EmailMapping.Handler;

        Assert.Throws<FormatException>(() => handler.Parse("not-an-email"));
    }

    [Fact]
    public void DefaultPrimitiveCannotBePersistedSilently()
    {
        BrazilianPrimitiveTypeHandler<Cpf> handler = BrazilianPrimitiveSqlServerMappings.CpfMapping.Handler;
        TestDbParameter parameter = new();

        Assert.Throws<InvalidOperationException>(() => handler.SetValue(parameter, default));
    }

    [Fact]
    public void RgAndInscricaoEstadualPersistOnlyValueAndRehydrateWithoutState()
    {
        Rg stateAwareRg = Rg.Parse("123456789", BrazilianState.Amazonas);
        InscricaoEstadual stateAwareInscricao = InscricaoEstadual.Parse("110042490114", BrazilianState.SaoPaulo);

        TestDbParameter rgParameter = new();
        BrazilianPrimitiveSqlServerMappings.RgMapping.Handler.SetValue(rgParameter, stateAwareRg);
        Rg rehydratedRg = BrazilianPrimitiveSqlServerMappings.RgMapping.Handler.Parse(stateAwareRg.Value);

        TestDbParameter inscricaoParameter = new();
        BrazilianPrimitiveSqlServerMappings.InscricaoEstadualMapping.Handler.SetValue(inscricaoParameter, stateAwareInscricao);
        InscricaoEstadual rehydratedInscricao = BrazilianPrimitiveSqlServerMappings.InscricaoEstadualMapping.Handler.Parse(stateAwareInscricao.Value);

        Assert.Equal(stateAwareRg.Value, rgParameter.Value);
        Assert.Equal(DbType.AnsiString, rgParameter.DbType);
        Assert.Equal(10, rgParameter.Size);
        Assert.Equal(stateAwareRg.Value, rehydratedRg.Value);
        Assert.False(rehydratedRg.HasState);

        Assert.Equal(stateAwareInscricao.Value, inscricaoParameter.Value);
        Assert.Equal(DbType.AnsiString, inscricaoParameter.DbType);
        Assert.Equal(14, inscricaoParameter.Size);
        Assert.Equal(stateAwareInscricao.Value, rehydratedInscricao.Value);
        Assert.False(rehydratedInscricao.HasState);
    }

    private static IEnumerable<IHandlerCase> CreateCases()
    {
        yield return new HandlerCase<Cpf>(
            BrazilianPrimitiveSqlServerMappings.CpfMapping,
            Cpf.Parse("529.982.247-25", CultureInfo.InvariantCulture),
            "52998224725");
        yield return new HandlerCase<Cnpj>(
            BrazilianPrimitiveSqlServerMappings.CnpjMapping,
            Cnpj.Parse("00000000e08g12", CultureInfo.InvariantCulture),
            "00000000E08G12");
        yield return new HandlerCase<CpfCnpj>(
            BrazilianPrimitiveSqlServerMappings.CpfCnpjMapping,
            CpfCnpj.Parse("00.000.000/e08g-12", CultureInfo.InvariantCulture),
            "00000000E08G12");
        yield return new HandlerCase<Cep>(
            BrazilianPrimitiveSqlServerMappings.CepMapping,
            Cep.Parse("01311-000", CultureInfo.InvariantCulture),
            "01311000");
        yield return new HandlerCase<Email>(
            BrazilianPrimitiveSqlServerMappings.EmailMapping,
            Email.Parse("usuario@domínio.com", CultureInfo.InvariantCulture),
            "usuario@xn--domnio-5va.com");
        yield return new HandlerCase<MobilePhone>(
            BrazilianPrimitiveSqlServerMappings.MobilePhoneMapping,
            MobilePhone.Parse("(11) 98765-4321", CultureInfo.InvariantCulture),
            "11987654321");
        yield return new HandlerCase<LandlinePhone>(
            BrazilianPrimitiveSqlServerMappings.LandlinePhoneMapping,
            LandlinePhone.Parse("(11) 3234-5678", CultureInfo.InvariantCulture),
            "1132345678");
        yield return new HandlerCase<TelefoneBrasileiro>(
            BrazilianPrimitiveSqlServerMappings.TelefoneBrasileiroMapping,
            TelefoneBrasileiro.Parse("(11) 98765-4321", CultureInfo.InvariantCulture),
            "11987654321");
        yield return new HandlerCase<Cnh>(
            BrazilianPrimitiveSqlServerMappings.CnhMapping,
            Cnh.Parse("02650306461", CultureInfo.InvariantCulture),
            "02650306461");
        yield return new HandlerCase<Cns>(
            BrazilianPrimitiveSqlServerMappings.CnsMapping,
            Cns.Parse("123456789010000", CultureInfo.InvariantCulture),
            "123456789010000");
        yield return new HandlerCase<TituloEleitoral>(
            BrazilianPrimitiveSqlServerMappings.TituloEleitoralMapping,
            TituloEleitoral.Parse("000123450159", CultureInfo.InvariantCulture),
            "000123450159");
        yield return new HandlerCase<Nit>(
            BrazilianPrimitiveSqlServerMappings.NitMapping,
            Nit.Parse("00000000001", CultureInfo.InvariantCulture),
            "00000000001");
        yield return new HandlerCase<PisPasep>(
            BrazilianPrimitiveSqlServerMappings.PisPasepMapping,
            PisPasep.Parse("01234567897", CultureInfo.InvariantCulture),
            "01234567897");
        yield return new HandlerCase<PlacaVeiculo>(
            BrazilianPrimitiveSqlServerMappings.PlacaVeiculoMapping,
            PlacaVeiculo.Parse("abc1d23", CultureInfo.InvariantCulture),
            "ABC1D23");
        yield return new HandlerCase<Renavam>(
            BrazilianPrimitiveSqlServerMappings.RenavamMapping,
            Renavam.Parse("00123456789", CultureInfo.InvariantCulture),
            "00123456789");
        yield return new HandlerCase<Ispb>(
            BrazilianPrimitiveSqlServerMappings.IspbMapping,
            Ispb.Parse("00000001", CultureInfo.InvariantCulture),
            "00000001");
        yield return new HandlerCase<CodigoCompe>(
            BrazilianPrimitiveSqlServerMappings.CodigoCompeMapping,
            CodigoCompe.Parse("001", CultureInfo.InvariantCulture),
            "001");
        yield return new HandlerCase<Rg>(
            BrazilianPrimitiveSqlServerMappings.RgMapping,
            Rg.Parse("00000005x"),
            "00000005X");
        yield return new HandlerCase<InscricaoEstadual>(
            BrazilianPrimitiveSqlServerMappings.InscricaoEstadualMapping,
            InscricaoEstadual.Parse("0012345678"),
            "0012345678");
    }

    private interface IHandlerCase
    {
        void AssertRoundTrip();
    }

    private sealed record HandlerCase<T>(
        BrazilianPrimitiveSqlServerMapping<T> Mapping,
        T ModelValue,
        string ProviderValue) : IHandlerCase
    {
        public void AssertRoundTrip()
        {
            TestDbParameter parameter = new();

            Mapping.Handler.SetValue(parameter, ModelValue);
            T? rehydrated = Mapping.Handler.Parse(ProviderValue);

            Assert.Equal(DbType.AnsiString, parameter.DbType);
            Assert.Equal(Mapping.Size, parameter.Size);
            Assert.Equal(ProviderValue, parameter.Value);
            Assert.Equal(ModelValue, rehydrated);
        }
    }
}
