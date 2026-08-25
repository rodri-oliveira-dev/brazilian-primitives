using System.Data;
using System.Globalization;
using Brazilian.PrimitivesTypes;
using Brazilian.PrimitivesTypes.Dapper.SqlServer.Tests.Infrastructure;
using Dapper;
using Xunit;

namespace Brazilian.PrimitivesTypes.Dapper.SqlServer.Tests;

public sealed class DapperParameterIntegrationTests
{
    [Fact]
    public void AnonymousPrimitiveParameterWorksForInsertUpdateAndWhere()
    {
        BrazilianPrimitivesDapperSqlServer.Register();
        Cpf cpf = Cpf.Parse("529.982.247-25", CultureInfo.InvariantCulture);
        TestDbConnection connection = new();

        connection.Execute("INSERT INTO People (Cpf) VALUES (@Cpf);", new { Cpf = cpf });
        AssertCpfParameter(connection, "52998224725");

        connection.Execute("UPDATE People SET Cpf = @Cpf WHERE Id = 1;", new { Cpf = cpf });
        AssertCpfParameter(connection, "52998224725");

        connection.Execute("SELECT 1 FROM People WHERE Cpf = @Cpf;", new { Cpf = cpf });
        AssertCpfParameter(connection, "52998224725");
    }

    [Fact]
    public void DynamicParametersUseRegisteredHandlersWithoutManualStringConversion()
    {
        BrazilianPrimitivesDapperSqlServer.Register();
        Cpf cpf = Cpf.Parse("529.982.247-25", CultureInfo.InvariantCulture);
        Email email = Email.Parse("usuario@domínio.com", CultureInfo.InvariantCulture);
        DynamicParameters parameters = new();
        parameters.Add("Cpf", cpf);
        parameters.Add("Email", email);
        TestDbConnection connection = new();

        connection.Execute("UPDATE People SET Email = @Email WHERE Cpf = @Cpf;", parameters);

        AssertParameter(connection, "Cpf", "52998224725", DbType.AnsiString, 11);
        AssertParameter(connection, "Email", "usuario@xn--domnio-5va.com", DbType.AnsiString, 254);
    }

    [Fact]
    public void NullableAnonymousParameterWithValueUsesRegisteredHandler()
    {
        BrazilianPrimitivesDapperSqlServer.Register();
        Cpf? cpf = Cpf.Parse("529.982.247-25", CultureInfo.InvariantCulture);
        TestDbConnection connection = new();

        connection.Execute("SELECT 1 FROM People WHERE Cpf = @Cpf;", new { Cpf = cpf });

        AssertParameter(connection, "Cpf", "52998224725", DbType.AnsiString, 11);
    }

    [Fact]
    public void NullableAnonymousNullProducesDbNull()
    {
        BrazilianPrimitivesDapperSqlServer.Register();
        TestDbConnection connection = new();

        connection.Execute("SELECT 1 WHERE @Email IS NULL;", new { Email = (Email?)null });

        Assert.Equal(DBNull.Value, GetParameter(connection, "Email").Value);
    }

    [Fact]
    public void DynamicParametersWithUntypedNullUseNativeDbNullBehavior()
    {
        SqlMapper.ResetTypeHandlers();

        try
        {
            DynamicParameters parameters = new();
            parameters.Add("Cpf", (Cpf?)null);
            TestDbConnection connection = new();

            connection.Execute("SELECT 1 WHERE @Cpf IS NULL;", parameters);

            TestDbParameter parameter = GetParameter(connection, "Cpf");
            Assert.Equal(DBNull.Value, parameter.Value);
            Assert.Equal(DbType.Object, parameter.DbType);
            Assert.Equal(0, parameter.Size);
        }
        finally
        {
            BrazilianPrimitivesDapperSqlServer.Register();
        }
    }

    [Fact]
    public void MultiplePrimitiveTypesShareTheSameAnonymousParameterObject()
    {
        BrazilianPrimitivesDapperSqlServer.Register();
        Cpf cpf = Cpf.Parse("529.982.247-25", CultureInfo.InvariantCulture);
        Email email = Email.Parse("user@example.com", CultureInfo.InvariantCulture);
        Cep cep = Cep.Parse("01311-000", CultureInfo.InvariantCulture);
        TestDbConnection connection = new();

        connection.Execute(
            "INSERT INTO People (Cpf, Email, Cep) VALUES (@Cpf, @Email, @Cep);",
            new { Cpf = cpf, Email = email, Cep = cep });

        AssertParameter(connection, "Cpf", "52998224725", DbType.AnsiString, 11);
        AssertParameter(connection, "Email", "user@example.com", DbType.AnsiString, 254);
        AssertParameter(connection, "Cep", "01311000", DbType.AnsiString, 8);
    }

    [Fact]
    public void ListExpansionDocumentsNativeDapperTypeHandlerLimitation()
    {
        BrazilianPrimitivesDapperSqlServer.Register();
        Cpf[] cpfs =
        [
            Cpf.Parse("529.982.247-25", CultureInfo.InvariantCulture),
            Cpf.Parse("111.444.777-35", CultureInfo.InvariantCulture),
        ];
        TestDbConnection connection = new();

        connection.Execute("SELECT 1 FROM People WHERE Cpf IN @Cpfs;", new { Cpfs = cpfs });

        Assert.Contains("@Cpfs1", connection.LastCommandText, StringComparison.Ordinal);
        Assert.Contains("@Cpfs2", connection.LastCommandText, StringComparison.Ordinal);

        // Dapper 2.1.x expands IEnumerable parameters natively, but PackListParameters does not invoke
        // ITypeHandler.SetValue for each item. The package deliberately exposes this upstream limitation
        // instead of introducing mandatory collection wrappers or connection abstractions.
        TestDbParameter first = GetParameter(connection, "Cpfs1");
        TestDbParameter second = GetParameter(connection, "Cpfs2");
        Assert.Equal(DbType.Object, first.DbType);
        Assert.Equal(DbType.Object, second.DbType);
        Assert.Equal(cpfs[0], Assert.IsType<Cpf>(first.Value));
        Assert.Equal(cpfs[1], Assert.IsType<Cpf>(second.Value));
    }

    [Fact]
    public void InvalidPersistedContentFailsDuringDapperMaterialization()
    {
        BrazilianPrimitivesDapperSqlServer.Register();
        TestDbConnection connection = new()
        {
            ReaderFactory = () => CreateSingleValueTable("Value", "not-an-email").CreateDataReader(),
        };

        Assert.Throws<FormatException>(() => connection.QuerySingle<Email>("SELECT Value FROM PrimitiveValues;"));
    }

    [Fact]
    public void DefaultPrimitiveCannotBePersistedThroughDapperSilently()
    {
        BrazilianPrimitivesDapperSqlServer.Register();
        TestDbConnection connection = new();

        Assert.Throws<InvalidOperationException>(
            () => connection.Execute("INSERT INTO People (Cpf) VALUES (@Cpf);", new { Cpf = default(Cpf) }));
    }

    [Fact]
    public void RgAndInscricaoEstadualRemainValueOnlyThroughDapper()
    {
        BrazilianPrimitivesDapperSqlServer.Register();
        Rg rg = Rg.Parse("123456789", BrazilianState.Amazonas);
        InscricaoEstadual inscricao = InscricaoEstadual.Parse("110042490114", BrazilianState.SaoPaulo);
        TestDbConnection connection = new();

        connection.Execute(
            "INSERT INTO Documents (Rg, InscricaoEstadual) VALUES (@Rg, @InscricaoEstadual);",
            new { Rg = rg, InscricaoEstadual = inscricao });

        Assert.Equal(2, connection.LastParameters.Count);
        AssertParameter(connection, "Rg", rg.Value, DbType.AnsiString, 10);
        AssertParameter(connection, "InscricaoEstadual", inscricao.Value, DbType.AnsiString, 14);
        Assert.DoesNotContain(
            connection.LastParameters,
            parameter => parameter.ParameterName.Contains("State", StringComparison.OrdinalIgnoreCase)
                || parameter.ParameterName.Contains("Uf", StringComparison.OrdinalIgnoreCase));

        connection.ReaderFactory = () => CreateSingleValueTable("Rg", rg.Value).CreateDataReader();
        Rg materializedRg = connection.QuerySingle<Rg>("SELECT Rg FROM Documents;");
        Assert.Equal(rg.Value, materializedRg.Value);
        Assert.False(materializedRg.HasState);

        connection.ReaderFactory = () => CreateSingleValueTable("InscricaoEstadual", inscricao.Value).CreateDataReader();
        InscricaoEstadual materializedInscricao = connection.QuerySingle<InscricaoEstadual>(
            "SELECT InscricaoEstadual FROM Documents;");
        Assert.Equal(inscricao.Value, materializedInscricao.Value);
        Assert.False(materializedInscricao.HasState);
    }

    private static void AssertCpfParameter(TestDbConnection connection, string expectedValue) =>
        AssertParameter(connection, "Cpf", expectedValue, DbType.AnsiString, 11);

    private static void AssertParameter(
        TestDbConnection connection,
        string name,
        object expectedValue,
        DbType expectedDbType,
        int expectedSize)
    {
        TestDbParameter parameter = GetParameter(connection, name);
        Assert.Equal(expectedValue, parameter.Value);
        Assert.Equal(expectedDbType, parameter.DbType);
        Assert.Equal(expectedSize, parameter.Size);
    }

    private static TestDbParameter GetParameter(TestDbConnection connection, string name) =>
        connection.LastParameters.Single(
            parameter => string.Equals(parameter.ParameterName, name, StringComparison.OrdinalIgnoreCase));

    private static DataTable CreateSingleValueTable(string columnName, string value)
    {
        DataTable table = new();
        table.Columns.Add(columnName, typeof(string));
        table.Rows.Add(value);
        return table;
    }
}
