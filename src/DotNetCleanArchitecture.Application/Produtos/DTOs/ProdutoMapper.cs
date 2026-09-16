using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Application.Produtos.DTOs
{
    internal static class ProdutoMapper
    {
        public static ProdutoResponse ParaResponse(Produto produto)
            => new(
                produto.Id,
                produto.Codigo,
                produto.Descricao,
                produto.Tipo.ToString(),
                produto.Ativo,
                produto.NCM,
                produto.CEST,
                produto.OrigemMercadoria.ToString(),
                produto.CST?.ToString(),
                produto.CSOSN?.ToString(),
                produto.EAN,
                produto.EANTrib,
                produto.UN,
                produto.VlrCusto,
                produto.VlrVenda,
                produto.EstoqueAtual,
                produto.EstoqueMinimo,
                produto.PesoLiquido,
                produto.PesoBruto,
                produto.Altura,
                produto.Largura,
                produto.Profundidade,
                produto.InfoAdicional);
    }
}
