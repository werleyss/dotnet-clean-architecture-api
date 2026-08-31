namespace DotNetCleanArchitecture.Domain.Core.Enum
{
    public enum Csosn
    {
        TributadaComPermissaoDeCredito = 101,
        TributadaSemPermissaoDeCredito = 102,
        IsencaoIcmsFaixaReceitaBruta = 103,
        TributadaComPermissaoDeCreditoECobrancaIcmsPorSt = 201,
        TributadaSemPermissaoDeCreditoECobrancaIcmsPorSt = 202,
        IsencaoIcmsFaixaReceitaBrutaECobrancaIcmsPorSt = 203,
        Imune = 300,
        NaoTributadaPeloSimplesNacional = 400,
        IcmsCobradoAnteriormentePorStOuAntecipacao = 500,
        Outros = 900
    }
}
