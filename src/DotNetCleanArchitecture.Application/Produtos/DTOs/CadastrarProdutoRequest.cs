using DotNetCleanArchitecture.Domain.Core.Enum;

namespace DotNetCleanArchitecture.Application.Produtos.DTOs
{
    public record CadastrarProdutoRequest(
        string Codigo,
        string Descricao,
        TipoProduto Tipo,
        string NCM,
        string? CEST,
        OrigemMercadoria OrigemMercadoria,
        CstIcms? CST,
        Csosn? CSOSN,
        string? EAN,
        string? EANTrib,
        string UN,
        decimal VlrCusto,
        decimal VlrVenda,
        decimal EstoqueAtual,
        decimal EstoqueMinimo,
        decimal PesoLiquido,
        decimal PesoBruto,
        decimal Altura,
        decimal Largura,
        decimal Profundidade,
        string? InfoAdicional,
        bool Ativo = true);
}
