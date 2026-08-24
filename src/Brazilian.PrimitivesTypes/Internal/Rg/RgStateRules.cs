namespace Brazilian.PrimitivesTypes;

internal static class RgStateRules
{
    internal static bool TryGet(BrazilianState state, out RgStateRule rule)
    {
        rule = state switch
        {
            BrazilianState.Acre => new RgStateRule(6, RgMaskKind.None, false),
            BrazilianState.Alagoas => new RgStateRule(7, RgMaskKind.None, false),
            BrazilianState.Amapa => new RgStateRule(9, RgMaskKind.None, false),
            BrazilianState.Amazonas => new RgStateRule(9, RgMaskKind.None, false),
            BrazilianState.Bahia => new RgStateRule(10, RgMaskKind.None, false),
            BrazilianState.Ceara => new RgStateRule(9, RgMaskKind.None, false),
            BrazilianState.DistritoFederal => new RgStateRule(7, RgMaskKind.None, false),
            BrazilianState.EspiritoSanto => new RgStateRule(9, RgMaskKind.None, false),
            BrazilianState.Goias => new RgStateRule(9, RgMaskKind.None, false),
            BrazilianState.Maranhao => new RgStateRule(9, RgMaskKind.None, false),
            BrazilianState.MatoGrosso => new RgStateRule(9, RgMaskKind.None, false),
            BrazilianState.MatoGrossoDoSul => new RgStateRule(9, RgMaskKind.None, false),
            BrazilianState.MinasGerais => new RgStateRule(8, RgMaskKind.MinasGerais, false),
            BrazilianState.Para => new RgStateRule(9, RgMaskKind.None, false),
            BrazilianState.Paraiba => new RgStateRule(9, RgMaskKind.None, false),
            BrazilianState.Parana => new RgStateRule(8, RgMaskKind.None, false),
            BrazilianState.Pernambuco => new RgStateRule(9, RgMaskKind.None, false),
            BrazilianState.Piaui => new RgStateRule(9, RgMaskKind.None, false),
            BrazilianState.RioDeJaneiro => new RgStateRule(9, RgMaskKind.RioDeJaneiro, false),
            BrazilianState.RioGrandeDoNorte => new RgStateRule(9, RgMaskKind.None, false),
            BrazilianState.RioGrandeDoSul => new RgStateRule(10, RgMaskKind.None, false),
            BrazilianState.Rondonia => new RgStateRule(9, RgMaskKind.None, false),
            BrazilianState.Roraima => new RgStateRule(9, RgMaskKind.None, false),
            BrazilianState.SantaCatarina => new RgStateRule(9, RgMaskKind.SantaCatarina, false),
            BrazilianState.SaoPaulo => new RgStateRule(9, RgMaskKind.SaoPaulo, true),
            BrazilianState.Sergipe => new RgStateRule(9, RgMaskKind.None, false),
            BrazilianState.Tocantins => new RgStateRule(9, RgMaskKind.None, false),
            _ => default,
        };

        return rule.CanonicalLength != 0;
    }
}
