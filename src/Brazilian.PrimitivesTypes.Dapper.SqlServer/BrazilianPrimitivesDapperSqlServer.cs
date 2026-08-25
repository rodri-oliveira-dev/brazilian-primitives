using Dapper;

namespace Brazilian.PrimitivesTypes.Dapper.SqlServer;

/// <summary>
/// Registers Dapper type handlers for Brazilian primitive value objects persisted in SQL Server.
/// </summary>
public static class BrazilianPrimitivesDapperSqlServer
{
    /// <summary>
    /// Registers all supported Brazilian primitive type handlers in Dapper's global type-handler registry.
    /// </summary>
    /// <remarks>
    /// Calling this method more than once is safe. Dapper also registers the corresponding nullable value types.
    /// RG and Inscricao Estadual use Value-only persistence and do not preserve federative-unit context.
    /// </remarks>
    public static void Register()
    {
        SqlMapper.AddTypeHandler(BrazilianPrimitiveSqlServerMappings.CpfMapping.Handler);
        SqlMapper.AddTypeHandler(BrazilianPrimitiveSqlServerMappings.CnpjMapping.Handler);
        SqlMapper.AddTypeHandler(BrazilianPrimitiveSqlServerMappings.CpfCnpjMapping.Handler);
        SqlMapper.AddTypeHandler(BrazilianPrimitiveSqlServerMappings.CepMapping.Handler);
        SqlMapper.AddTypeHandler(BrazilianPrimitiveSqlServerMappings.EmailMapping.Handler);
        SqlMapper.AddTypeHandler(BrazilianPrimitiveSqlServerMappings.MobilePhoneMapping.Handler);
        SqlMapper.AddTypeHandler(BrazilianPrimitiveSqlServerMappings.LandlinePhoneMapping.Handler);
        SqlMapper.AddTypeHandler(BrazilianPrimitiveSqlServerMappings.TelefoneBrasileiroMapping.Handler);
        SqlMapper.AddTypeHandler(BrazilianPrimitiveSqlServerMappings.ChavePixMapping.Handler);
        SqlMapper.AddTypeHandler(BrazilianPrimitiveSqlServerMappings.CnhMapping.Handler);
        SqlMapper.AddTypeHandler(BrazilianPrimitiveSqlServerMappings.CnsMapping.Handler);
        SqlMapper.AddTypeHandler(BrazilianPrimitiveSqlServerMappings.TituloEleitoralMapping.Handler);
        SqlMapper.AddTypeHandler(BrazilianPrimitiveSqlServerMappings.NitMapping.Handler);
        SqlMapper.AddTypeHandler(BrazilianPrimitiveSqlServerMappings.PisPasepMapping.Handler);
        SqlMapper.AddTypeHandler(BrazilianPrimitiveSqlServerMappings.PlacaVeiculoMapping.Handler);
        SqlMapper.AddTypeHandler(BrazilianPrimitiveSqlServerMappings.RenavamMapping.Handler);
        SqlMapper.AddTypeHandler(BrazilianPrimitiveSqlServerMappings.IspbMapping.Handler);
        SqlMapper.AddTypeHandler(BrazilianPrimitiveSqlServerMappings.CodigoCompeMapping.Handler);
        SqlMapper.AddTypeHandler(BrazilianPrimitiveSqlServerMappings.RgMapping.Handler);
        SqlMapper.AddTypeHandler(BrazilianPrimitiveSqlServerMappings.InscricaoEstadualMapping.Handler);
    }
}
