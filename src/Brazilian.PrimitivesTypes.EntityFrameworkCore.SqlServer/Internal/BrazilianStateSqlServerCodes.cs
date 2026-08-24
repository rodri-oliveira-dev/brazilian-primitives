using Brazilian.PrimitivesTypes;

namespace Brazilian.PrimitivesTypes.EntityFrameworkCore.SqlServer;

internal static class BrazilianStateSqlServerCodes
{
    public static string ToCode(BrazilianState state)
    {
        return state switch
        {
            BrazilianState.Acre => "AC",
            BrazilianState.Alagoas => "AL",
            BrazilianState.Amapa => "AP",
            BrazilianState.Amazonas => "AM",
            BrazilianState.Bahia => "BA",
            BrazilianState.Ceara => "CE",
            BrazilianState.DistritoFederal => "DF",
            BrazilianState.EspiritoSanto => "ES",
            BrazilianState.Goias => "GO",
            BrazilianState.Maranhao => "MA",
            BrazilianState.MatoGrosso => "MT",
            BrazilianState.MatoGrossoDoSul => "MS",
            BrazilianState.MinasGerais => "MG",
            BrazilianState.Para => "PA",
            BrazilianState.Paraiba => "PB",
            BrazilianState.Parana => "PR",
            BrazilianState.Pernambuco => "PE",
            BrazilianState.Piaui => "PI",
            BrazilianState.RioDeJaneiro => "RJ",
            BrazilianState.RioGrandeDoNorte => "RN",
            BrazilianState.RioGrandeDoSul => "RS",
            BrazilianState.Rondonia => "RO",
            BrazilianState.Roraima => "RR",
            BrazilianState.SantaCatarina => "SC",
            BrazilianState.SaoPaulo => "SP",
            BrazilianState.Sergipe => "SE",
            BrazilianState.Tocantins => "TO",
            _ => throw new InvalidOperationException("Only a known Brazilian federative unit can be persisted in a state-aware mapping."),
        };
    }

    public static BrazilianState Parse(string code)
    {
        return code switch
        {
            "AC" => BrazilianState.Acre,
            "AL" => BrazilianState.Alagoas,
            "AP" => BrazilianState.Amapa,
            "AM" => BrazilianState.Amazonas,
            "BA" => BrazilianState.Bahia,
            "CE" => BrazilianState.Ceara,
            "DF" => BrazilianState.DistritoFederal,
            "ES" => BrazilianState.EspiritoSanto,
            "GO" => BrazilianState.Goias,
            "MA" => BrazilianState.Maranhao,
            "MT" => BrazilianState.MatoGrosso,
            "MS" => BrazilianState.MatoGrossoDoSul,
            "MG" => BrazilianState.MinasGerais,
            "PA" => BrazilianState.Para,
            "PB" => BrazilianState.Paraiba,
            "PR" => BrazilianState.Parana,
            "PE" => BrazilianState.Pernambuco,
            "PI" => BrazilianState.Piaui,
            "RJ" => BrazilianState.RioDeJaneiro,
            "RN" => BrazilianState.RioGrandeDoNorte,
            "RS" => BrazilianState.RioGrandeDoSul,
            "RO" => BrazilianState.Rondonia,
            "RR" => BrazilianState.Roraima,
            "SC" => BrazilianState.SantaCatarina,
            "SP" => BrazilianState.SaoPaulo,
            "SE" => BrazilianState.Sergipe,
            "TO" => BrazilianState.Tocantins,
            _ => throw new FormatException("Persisted Brazilian state code must be a supported two-letter UF code."),
        };
    }
}
