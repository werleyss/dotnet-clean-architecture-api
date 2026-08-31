namespace DotNetCleanArchitecture.Domain.Core.Enum
{
    public enum OrigemMercadoria
    {
        Nacional = 0,
        EstrangeiraImportacaoDireta = 1,
        EstrangeiraAdquiridaMercadoInterno = 2,
        NacionalConteudoImportacaoSuperior40AteInferiorOuIgual70 = 3,
        NacionalProcessoProdutivoBasico = 4,
        NacionalConteudoImportacaoInferiorOuIgual40 = 5,
        EstrangeiraImportacaoDiretaSemSimilarNacional = 6,
        EstrangeiraAdquiridaMercadoInternoSemSimilarNacional = 7,
        NacionalConteudoImportacaoSuperior70 = 8
    }
}
