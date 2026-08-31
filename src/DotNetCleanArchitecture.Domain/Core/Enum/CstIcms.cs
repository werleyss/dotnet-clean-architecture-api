namespace DotNetCleanArchitecture.Domain.Core.Enum
{
    public enum CstIcms
    {
        TributadaIntegralmente = 0,
        TributadaComCobrancaIcmsPorSt = 10,
        ComReducaoBaseCalculo = 20,
        IsentaOuNaoTributadaComCobrancaIcmsPorSt = 30,
        Isenta = 40,
        NaoTributada = 41,
        Suspensao = 50,
        Diferimento = 51,
        IcmsCobradoAnteriormentePorSt = 60,
        ComReducaoBaseCalculoECobrancaIcmsPorSt = 70,
        Outras = 90
    }
}
