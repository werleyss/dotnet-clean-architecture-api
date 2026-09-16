namespace DotNetCleanArchitecture.Application.Produtos.DTOs
{
    public record ProdutoResponse(
        Guid Id,
        string Codigo,
        string Descricao,
        string Tipo,
        bool Ativo,
        string NCM,
        string? CEST,
        string OrigemMercadoria,
        string? CST,
        string? CSOSN,
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
        string? InfoAdicional);
}
