namespace Brazilian.PrimitivesTypes;

internal static class InscricaoEstadualStateRules
{
    internal static bool TryGet(BrazilianState state, out InscricaoEstadualRule rule)
    {
        rule = state switch
        {
            BrazilianState.Acre => new InscricaoEstadualRule(13),
            BrazilianState.Alagoas => new InscricaoEstadualRule(9),
            BrazilianState.Amapa => new InscricaoEstadualRule(9),
            BrazilianState.Amazonas => new InscricaoEstadualRule(9),
            BrazilianState.Bahia => new InscricaoEstadualRule(8, 9),
            BrazilianState.Ceara => new InscricaoEstadualRule(9),
            BrazilianState.DistritoFederal => new InscricaoEstadualRule(13),
            BrazilianState.EspiritoSanto => new InscricaoEstadualRule(9),
            BrazilianState.Goias => new InscricaoEstadualRule(9),
            BrazilianState.Maranhao => new InscricaoEstadualRule(9),
            BrazilianState.MatoGrosso => new InscricaoEstadualRule(11),
            BrazilianState.MatoGrossoDoSul => new InscricaoEstadualRule(9),
            BrazilianState.MinasGerais => new InscricaoEstadualRule(13),
            BrazilianState.Para => new InscricaoEstadualRule(9),
            BrazilianState.Paraiba => new InscricaoEstadualRule(9),
            BrazilianState.Parana => new InscricaoEstadualRule(10),
            BrazilianState.Pernambuco => new InscricaoEstadualRule(9, 14),
            BrazilianState.Piaui => new InscricaoEstadualRule(9),
            BrazilianState.RioDeJaneiro => new InscricaoEstadualRule(8),
            BrazilianState.RioGrandeDoNorte => new InscricaoEstadualRule(9, 10),
            BrazilianState.RioGrandeDoSul => new InscricaoEstadualRule(10),
            BrazilianState.Rondonia => new InscricaoEstadualRule(14),
            BrazilianState.Roraima => new InscricaoEstadualRule(9),
            BrazilianState.SantaCatarina => new InscricaoEstadualRule(9),
            BrazilianState.SaoPaulo => new InscricaoEstadualRule(12),
            BrazilianState.Sergipe => new InscricaoEstadualRule(9),
            BrazilianState.Tocantins => new InscricaoEstadualRule(11),
            _ => default,
        };

        return rule.FirstLength != 0;
    }
}
