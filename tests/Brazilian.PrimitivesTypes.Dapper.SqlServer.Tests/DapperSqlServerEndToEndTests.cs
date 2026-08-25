using System.Globalization;
using Brazilian.PrimitivesTypes;
using Brazilian.PrimitivesTypes.Dapper.SqlServer.Tests.Infrastructure;
using Dapper;
using Microsoft.Data.SqlClient;
using Xunit;

namespace Brazilian.PrimitivesTypes.Dapper.SqlServer.Tests;

[Collection(SqlServerEndToEndCollection.Name)]
public sealed class DapperSqlServerEndToEndTests
{
    private readonly SqlServerContainerFixture _fixture;

    public DapperSqlServerEndToEndTests(SqlServerContainerFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task AllSupportedPrimitivesRoundTripWithCanonicalVarcharStorage()
    {
        BrazilianPrimitivesDapperSqlServer.Register();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        await using SqlServerDatabase database = await _fixture.CreateDatabaseAsync(cancellationToken);
        await using SqlConnection connection = new(database.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        await connection.ExecuteAsync(CreatePrimitiveRoundTripTableSql);

        Cpf cpf = Cpf.Parse("529.982.247-25", CultureInfo.InvariantCulture);
        Cnpj cnpj = Cnpj.Parse("00000000e08g12", CultureInfo.InvariantCulture);
        CpfCnpj cpfCnpj = CpfCnpj.Parse("00.000.000/e08g-12", CultureInfo.InvariantCulture);
        Cep cep = Cep.Parse("01311-000", CultureInfo.InvariantCulture);
        Email email = Email.Parse("usuario@domínio.com", CultureInfo.InvariantCulture);
        MobilePhone mobilePhone = MobilePhone.Parse("(11) 98765-4321", CultureInfo.InvariantCulture);
        LandlinePhone landlinePhone = LandlinePhone.Parse("(11) 3234-5678", CultureInfo.InvariantCulture);
        TelefoneBrasileiro telefone = TelefoneBrasileiro.Parse("(11) 98765-4321", CultureInfo.InvariantCulture);
        ChavePix chavePix = ChavePix.From(Cpf.Parse("11900000083", CultureInfo.InvariantCulture));
        Cnh cnh = Cnh.Parse("02650306461", CultureInfo.InvariantCulture);
        Cns cns = Cns.Parse("123456789010000", CultureInfo.InvariantCulture);
        TituloEleitoral titulo = TituloEleitoral.Parse("000123450159", CultureInfo.InvariantCulture);
        Nit nit = Nit.Parse("00000000001", CultureInfo.InvariantCulture);
        PisPasep pisPasep = PisPasep.Parse("01234567897", CultureInfo.InvariantCulture);
        PlacaVeiculo placa = PlacaVeiculo.Parse("abc1d23", CultureInfo.InvariantCulture);
        Renavam renavam = Renavam.Parse("00123456789", CultureInfo.InvariantCulture);
        Ispb ispb = Ispb.Parse("00000001", CultureInfo.InvariantCulture);
        CodigoCompe codigoCompe = CodigoCompe.Parse("001", CultureInfo.InvariantCulture);
        Rg rg = Rg.Parse("123456789", BrazilianState.Amazonas);
        InscricaoEstadual inscricaoEstadual = InscricaoEstadual.Parse("110042490114", BrazilianState.SaoPaulo);

        await connection.ExecuteAsync(
            InsertPrimitiveRoundTripSql,
            new
            {
                Id = 1,
                Cpf = cpf,
                Cnpj = cnpj,
                CpfCnpj = cpfCnpj,
                Cep = cep,
                Email = email,
                MobilePhone = mobilePhone,
                LandlinePhone = landlinePhone,
                TelefoneBrasileiro = telefone,
                ChavePix = chavePix,
                Cnh = cnh,
                Cns = cns,
                TituloEleitoral = titulo,
                Nit = nit,
                PisPasep = pisPasep,
                PlacaVeiculo = placa,
                Renavam = renavam,
                Ispb = ispb,
                CodigoCompe = codigoCompe,
                Rg = rg,
                InscricaoEstadual = inscricaoEstadual,
            });

        StoredPrimitiveRow stored = await connection.QuerySingleAsync<StoredPrimitiveRow>(
            SelectPrimitiveRoundTripSql,
            new
            {
                Id = 1
            });

        Assert.Equal("52998224725", stored.Cpf);
        Assert.Equal("00000000E08G12", stored.Cnpj);
        Assert.Equal("00000000E08G12", stored.CpfCnpj);
        Assert.Equal("01311000", stored.Cep);
        Assert.Equal("usuario@xn--domnio-5va.com", stored.Email);
        Assert.Equal("11987654321", stored.MobilePhone);
        Assert.Equal("1132345678", stored.LandlinePhone);
        Assert.Equal("11987654321", stored.TelefoneBrasileiro);
        Assert.Equal("11900000083", stored.ChavePix);
        Assert.Equal("02650306461", stored.Cnh);
        Assert.Equal("123456789010000", stored.Cns);
        Assert.Equal("000123450159", stored.TituloEleitoral);
        Assert.Equal("00000000001", stored.Nit);
        Assert.Equal("01234567897", stored.PisPasep);
        Assert.Equal("ABC1D23", stored.PlacaVeiculo);
        Assert.Equal("00123456789", stored.Renavam);
        Assert.Equal("00000001", stored.Ispb);
        Assert.Equal("001", stored.CodigoCompe);
        Assert.Equal(rg.Value, stored.Rg);
        Assert.Equal(inscricaoEstadual.Value, stored.InscricaoEstadual);

        MaterializedPrimitiveRow materialized = await connection.QuerySingleAsync<MaterializedPrimitiveRow>(
            SelectPrimitiveRoundTripSql,
            new
            {
                Id = 1
            });

        Assert.Equal(cpf, materialized.Cpf);
        Assert.Equal(cnpj, materialized.Cnpj);
        Assert.Equal(cpfCnpj, materialized.CpfCnpj);
        Assert.Equal(cep, materialized.Cep);
        Assert.Equal(email, materialized.Email);
        Assert.Equal(mobilePhone, materialized.MobilePhone);
        Assert.Equal(landlinePhone, materialized.LandlinePhone);
        Assert.Equal(telefone, materialized.TelefoneBrasileiro);
        Assert.Equal(chavePix, materialized.ChavePix);
        Assert.Equal(TipoChavePix.Cpf, materialized.ChavePix.Tipo);
        Assert.Equal(cnh, materialized.Cnh);
        Assert.Equal(cns, materialized.Cns);
        Assert.Equal(titulo, materialized.TituloEleitoral);
        Assert.Equal(nit, materialized.Nit);
        Assert.Equal(pisPasep, materialized.PisPasep);
        Assert.Equal(placa, materialized.PlacaVeiculo);
        Assert.Equal(renavam, materialized.Renavam);
        Assert.Equal(ispb, materialized.Ispb);
        Assert.Equal(codigoCompe, materialized.CodigoCompe);
        Assert.Equal(rg.Value, materialized.Rg.Value);
        Assert.False(materialized.Rg.HasState);
        Assert.Equal(inscricaoEstadual.Value, materialized.InscricaoEstadual.Value);
        Assert.False(materialized.InscricaoEstadual.HasState);

        await AssertPhysicalSchemaAsync(connection);
    }

    [Fact]
    public async Task UpdateWhereAndDynamicParametersUsePrimitiveHandlersAgainstSqlServer()
    {
        BrazilianPrimitivesDapperSqlServer.Register();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        await using SqlServerDatabase database = await _fixture.CreateDatabaseAsync(cancellationToken);
        await using SqlConnection connection = new(database.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        await connection.ExecuteAsync(
            "CREATE TABLE [CpfFlow] ([Id] int NOT NULL PRIMARY KEY, [Cpf] varchar(11) NOT NULL, [Email] varchar(254) NULL);");

        Cpf originalCpf = Cpf.Parse("529.982.247-25", CultureInfo.InvariantCulture);
        Cpf updatedCpf = Cpf.Parse("111.444.777-35", CultureInfo.InvariantCulture);
        Email updatedEmail = Email.Parse("USER@Example.COM", CultureInfo.InvariantCulture);

        await connection.ExecuteAsync(
            "INSERT INTO [CpfFlow] ([Id], [Cpf], [Email]) VALUES (@Id, @Cpf, NULL);",
            new
            {
                Id = 1,
                Cpf = originalCpf
            });

        DynamicParameters parameters = new();
        parameters.Add("Id", 1);
        parameters.Add("Cpf", updatedCpf);
        parameters.Add("Email", updatedEmail);

        await connection.ExecuteAsync(
            "UPDATE [CpfFlow] SET [Cpf] = @Cpf, [Email] = @Email WHERE [Id] = @Id;",
            parameters);

        Cpf queried = await connection.QuerySingleAsync<Cpf>(
            "SELECT [Cpf] FROM [CpfFlow] WHERE [Cpf] = @Cpf;",
            new
            {
                Cpf = updatedCpf
            });
        string storedCpf = await connection.QuerySingleAsync<string>(
            "SELECT [Cpf] FROM [CpfFlow] WHERE [Id] = @Id;",
            new
            {
                Id = 1
            });
        string storedEmail = await connection.QuerySingleAsync<string>(
            "SELECT [Email] FROM [CpfFlow] WHERE [Id] = @Id;",
            new
            {
                Id = 1
            });

        Assert.Equal(updatedCpf, queried);
        Assert.Equal("11144477735", storedCpf);
        Assert.Equal("user@example.com", storedEmail);
    }

    [Fact]
    public async Task NullablePrimitivesRoundTripWithValueAndSqlNull()
    {
        BrazilianPrimitivesDapperSqlServer.Register();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        await using SqlServerDatabase database = await _fixture.CreateDatabaseAsync(cancellationToken);
        await using SqlConnection connection = new(database.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        await connection.ExecuteAsync(
            "CREATE TABLE [NullableFlow] ([Id] int NOT NULL PRIMARY KEY, [Cpf] varchar(11) NULL, [Email] varchar(254) NULL);");

        Cpf? cpf = Cpf.Parse("529.982.247-25", CultureInfo.InvariantCulture);
        Email? email = Email.Parse("user@example.com", CultureInfo.InvariantCulture);

        await connection.ExecuteAsync(
            "INSERT INTO [NullableFlow] ([Id], [Cpf], [Email]) VALUES (@Id, @Cpf, @Email);",
            new
            {
                Id = 1,
                Cpf = cpf,
                Email = email
            });
        await connection.ExecuteAsync(
            "INSERT INTO [NullableFlow] ([Id], [Cpf], [Email]) VALUES (@Id, @Cpf, @Email);",
            new
            {
                Id = 2,
                Cpf = (Cpf?)null,
                Email = (Email?)null
            });

        NullablePrimitiveRow withValues = await connection.QuerySingleAsync<NullablePrimitiveRow>(
            "SELECT [Cpf], [Email] FROM [NullableFlow] WHERE [Id] = @Id;",
            new
            {
                Id = 1
            });
        NullablePrimitiveRow withNulls = await connection.QuerySingleAsync<NullablePrimitiveRow>(
            "SELECT [Cpf], [Email] FROM [NullableFlow] WHERE [Id] = @Id;",
            new
            {
                Id = 2
            });

        Assert.Equal(cpf, withValues.Cpf);
        Assert.Equal(email, withValues.Email);
        Assert.Null(withNulls.Cpf);
        Assert.Null(withNulls.Email);
    }

    [Fact]
    public async Task InvalidPersistedContentFailsDuringRealSqlServerMaterialization()
    {
        BrazilianPrimitivesDapperSqlServer.Register();
        CancellationToken cancellationToken = TestContext.Current.CancellationToken;
        await using SqlServerDatabase database = await _fixture.CreateDatabaseAsync(cancellationToken);
        await using SqlConnection connection = new(database.ConnectionString);
        await connection.OpenAsync(cancellationToken);

        await connection.ExecuteAsync("CREATE TABLE [InvalidEmail] ([Value] varchar(254) NOT NULL);");
        await connection.ExecuteAsync(
            "INSERT INTO [InvalidEmail] ([Value]) VALUES (@Value);",
            new
            {
                Value = "not-an-email"
            });

        await Assert.ThrowsAsync<FormatException>(
            () => connection.QuerySingleAsync<Email>("SELECT [Value] FROM [InvalidEmail];"));
    }

    private static async Task AssertPhysicalSchemaAsync(SqlConnection connection)
    {
        IReadOnlyDictionary<string, int> expectedSizes = new Dictionary<string, int>(StringComparer.Ordinal)
        {
            [nameof(MaterializedPrimitiveRow.Cpf)] = 11,
            [nameof(MaterializedPrimitiveRow.Cnpj)] = 14,
            [nameof(MaterializedPrimitiveRow.CpfCnpj)] = 14,
            [nameof(MaterializedPrimitiveRow.Cep)] = 8,
            [nameof(MaterializedPrimitiveRow.Email)] = 254,
            [nameof(MaterializedPrimitiveRow.MobilePhone)] = 11,
            [nameof(MaterializedPrimitiveRow.LandlinePhone)] = 10,
            [nameof(MaterializedPrimitiveRow.TelefoneBrasileiro)] = 11,
            [nameof(MaterializedPrimitiveRow.ChavePix)] = 77,
            [nameof(MaterializedPrimitiveRow.Cnh)] = 11,
            [nameof(MaterializedPrimitiveRow.Cns)] = 15,
            [nameof(MaterializedPrimitiveRow.TituloEleitoral)] = 12,
            [nameof(MaterializedPrimitiveRow.Nit)] = 11,
            [nameof(MaterializedPrimitiveRow.PisPasep)] = 11,
            [nameof(MaterializedPrimitiveRow.PlacaVeiculo)] = 7,
            [nameof(MaterializedPrimitiveRow.Renavam)] = 11,
            [nameof(MaterializedPrimitiveRow.Ispb)] = 8,
            [nameof(MaterializedPrimitiveRow.CodigoCompe)] = 3,
            [nameof(MaterializedPrimitiveRow.Rg)] = 10,
            [nameof(MaterializedPrimitiveRow.InscricaoEstadual)] = 14,
        };

        IEnumerable<ColumnMetadata> columns = await connection.QueryAsync<ColumnMetadata>(
            """
            SELECT [COLUMN_NAME] AS [ColumnName], [DATA_TYPE] AS [DataType],
                   [CHARACTER_MAXIMUM_LENGTH] AS [MaxLength]
            FROM [INFORMATION_SCHEMA].[COLUMNS]
            WHERE [TABLE_NAME] = 'PrimitiveRoundTrip';
            """);
        ColumnMetadata[] materializedColumns = columns.ToArray();

        foreach ((string columnName, int expectedSize) in expectedSizes)
        {
            ColumnMetadata column = Assert.Single(materializedColumns, candidate => candidate.ColumnName == columnName);
            Assert.Equal("varchar", column.DataType);
            Assert.Equal(expectedSize, column.MaxLength);
        }

        Assert.DoesNotContain(
            materializedColumns,
            column => column.ColumnName.Contains("State", StringComparison.OrdinalIgnoreCase)
                || column.ColumnName.Contains("Uf", StringComparison.OrdinalIgnoreCase));
    }

    private const string CreatePrimitiveRoundTripTableSql = """
        CREATE TABLE [PrimitiveRoundTrip]
        (
            [Id] int NOT NULL PRIMARY KEY,
            [Cpf] varchar(11) NOT NULL,
            [Cnpj] varchar(14) NOT NULL,
            [CpfCnpj] varchar(14) NOT NULL,
            [Cep] varchar(8) NOT NULL,
            [Email] varchar(254) NOT NULL,
            [MobilePhone] varchar(11) NOT NULL,
            [LandlinePhone] varchar(10) NOT NULL,
            [TelefoneBrasileiro] varchar(11) NOT NULL,
            [ChavePix] varchar(77) NOT NULL,
            [Cnh] varchar(11) NOT NULL,
            [Cns] varchar(15) NOT NULL,
            [TituloEleitoral] varchar(12) NOT NULL,
            [Nit] varchar(11) NOT NULL,
            [PisPasep] varchar(11) NOT NULL,
            [PlacaVeiculo] varchar(7) NOT NULL,
            [Renavam] varchar(11) NOT NULL,
            [Ispb] varchar(8) NOT NULL,
            [CodigoCompe] varchar(3) NOT NULL,
            [Rg] varchar(10) NOT NULL,
            [InscricaoEstadual] varchar(14) NOT NULL
        );
        """;

    private const string InsertPrimitiveRoundTripSql = """
        INSERT INTO [PrimitiveRoundTrip]
        (
            [Id], [Cpf], [Cnpj], [CpfCnpj], [Cep], [Email], [MobilePhone], [LandlinePhone],
            [TelefoneBrasileiro], [ChavePix], [Cnh], [Cns], [TituloEleitoral], [Nit], [PisPasep],
            [PlacaVeiculo], [Renavam], [Ispb], [CodigoCompe], [Rg], [InscricaoEstadual]
        )
        VALUES
        (
            @Id, @Cpf, @Cnpj, @CpfCnpj, @Cep, @Email, @MobilePhone, @LandlinePhone,
            @TelefoneBrasileiro, @ChavePix, @Cnh, @Cns, @TituloEleitoral, @Nit, @PisPasep,
            @PlacaVeiculo, @Renavam, @Ispb, @CodigoCompe, @Rg, @InscricaoEstadual
        );
        """;

    private const string SelectPrimitiveRoundTripSql = """
        SELECT [Cpf], [Cnpj], [CpfCnpj], [Cep], [Email], [MobilePhone], [LandlinePhone],
               [TelefoneBrasileiro], [ChavePix], [Cnh], [Cns], [TituloEleitoral], [Nit], [PisPasep],
               [PlacaVeiculo], [Renavam], [Ispb], [CodigoCompe], [Rg], [InscricaoEstadual]
        FROM [PrimitiveRoundTrip]
        WHERE [Id] = @Id;
        """;

    private sealed class StoredPrimitiveRow
    {
        public string Cpf { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public string CpfCnpj { get; set; } = string.Empty;
        public string Cep { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string MobilePhone { get; set; } = string.Empty;
        public string LandlinePhone { get; set; } = string.Empty;
        public string TelefoneBrasileiro { get; set; } = string.Empty;
        public string ChavePix { get; set; } = string.Empty;
        public string Cnh { get; set; } = string.Empty;
        public string Cns { get; set; } = string.Empty;
        public string TituloEleitoral { get; set; } = string.Empty;
        public string Nit { get; set; } = string.Empty;
        public string PisPasep { get; set; } = string.Empty;
        public string PlacaVeiculo { get; set; } = string.Empty;
        public string Renavam { get; set; } = string.Empty;
        public string Ispb { get; set; } = string.Empty;
        public string CodigoCompe { get; set; } = string.Empty;
        public string Rg { get; set; } = string.Empty;
        public string InscricaoEstadual { get; set; } = string.Empty;
    }

    private sealed class MaterializedPrimitiveRow
    {
        public Cpf Cpf
        {
            get; set;
        }
        public Cnpj Cnpj
        {
            get; set;
        }
        public CpfCnpj CpfCnpj
        {
            get; set;
        }
        public Cep Cep
        {
            get; set;
        }
        public Email Email
        {
            get; set;
        }
        public MobilePhone MobilePhone
        {
            get; set;
        }
        public LandlinePhone LandlinePhone
        {
            get; set;
        }
        public TelefoneBrasileiro TelefoneBrasileiro
        {
            get; set;
        }
        public ChavePix ChavePix
        {
            get; set;
        }
        public Cnh Cnh
        {
            get; set;
        }
        public Cns Cns
        {
            get; set;
        }
        public TituloEleitoral TituloEleitoral
        {
            get; set;
        }
        public Nit Nit
        {
            get; set;
        }
        public PisPasep PisPasep
        {
            get; set;
        }
        public PlacaVeiculo PlacaVeiculo
        {
            get; set;
        }
        public Renavam Renavam
        {
            get; set;
        }
        public Ispb Ispb
        {
            get; set;
        }
        public CodigoCompe CodigoCompe
        {
            get; set;
        }
        public Rg Rg
        {
            get; set;
        }
        public InscricaoEstadual InscricaoEstadual
        {
            get; set;
        }
    }

    private sealed class NullablePrimitiveRow
    {
        public Cpf? Cpf
        {
            get; set;
        }
        public Email? Email
        {
            get; set;
        }
    }

    private sealed class ColumnMetadata
    {
        public string ColumnName { get; set; } = string.Empty;
        public string DataType { get; set; } = string.Empty;
        public int? MaxLength
        {
            get; set;
        }
    }
}
