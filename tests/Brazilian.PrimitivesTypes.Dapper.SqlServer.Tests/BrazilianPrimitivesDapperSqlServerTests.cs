using Brazilian.PrimitivesTypes;
using Dapper;
using Xunit;

namespace Brazilian.PrimitivesTypes.Dapper.SqlServer.Tests;

public sealed class BrazilianPrimitivesDapperSqlServerTests
{
    [Fact]
    public void RegisterInstallsAllHandlersAndCanBeCalledRepeatedly()
    {
        Type[] primitiveTypes =
        [
            typeof(Cpf),
            typeof(Cnpj),
            typeof(CpfCnpj),
            typeof(Cep),
            typeof(Email),
            typeof(MobilePhone),
            typeof(LandlinePhone),
            typeof(TelefoneBrasileiro),
            typeof(ChavePix),
            typeof(Cnh),
            typeof(Cns),
            typeof(TituloEleitoral),
            typeof(Nit),
            typeof(PisPasep),
            typeof(PlacaVeiculo),
            typeof(Renavam),
            typeof(Ispb),
            typeof(CodigoCompe),
            typeof(Rg),
            typeof(InscricaoEstadual),
        ];

        SqlMapper.ResetTypeHandlers();

        try
        {
            BrazilianPrimitivesDapperSqlServer.Register();
            AssertHandlersRegistered(primitiveTypes);

            BrazilianPrimitivesDapperSqlServer.Register();
            AssertHandlersRegistered(primitiveTypes);
        }
        finally
        {
            SqlMapper.ResetTypeHandlers();
        }
    }

    [Fact]
    public void RegisterWorksAgainAfterDapperTypeHandlerReset()
    {
        SqlMapper.ResetTypeHandlers();

        try
        {
            BrazilianPrimitivesDapperSqlServer.Register();
            Assert.True(SqlMapper.HasTypeHandler(typeof(Cpf)));

            SqlMapper.ResetTypeHandlers();
            Assert.False(SqlMapper.HasTypeHandler(typeof(Cpf)));

            BrazilianPrimitivesDapperSqlServer.Register();
            Assert.True(SqlMapper.HasTypeHandler(typeof(Cpf)));
        }
        finally
        {
            SqlMapper.ResetTypeHandlers();
        }
    }

    private static void AssertHandlersRegistered(IEnumerable<Type> primitiveTypes)
    {
        foreach (Type primitiveType in primitiveTypes)
        {
            Type nullableType = typeof(Nullable<>).MakeGenericType(primitiveType);

            Assert.True(SqlMapper.HasTypeHandler(primitiveType), $"Missing handler for {primitiveType.Name}.");
            Assert.True(SqlMapper.HasTypeHandler(nullableType), $"Missing handler for nullable {primitiveType.Name}.");
        }
    }
}
