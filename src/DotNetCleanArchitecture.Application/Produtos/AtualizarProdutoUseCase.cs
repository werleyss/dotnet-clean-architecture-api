using DotNetCleanArchitecture.Application.Produtos.DTOs;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Produtos
{
    public class AtualizarProdutoUseCase
    {
        private readonly IProdutoRepositorio _produtoRepositorio;

        public AtualizarProdutoUseCase(IProdutoRepositorio produtoRepositorio)
        {
            _produtoRepositorio = produtoRepositorio;
        }

        public async Task<ProdutoResponse> ExecutarAsync(AtualizarProdutoRequest request)
        {
            var produto = await _produtoRepositorio.ObterPorIdAsync(request.Id)
                ?? throw new ExcecaoDeDominio("Produto não encontrado.");

            produto.Atualizar(
                request.Descricao,
                request.Tipo,
                request.NCM,
                request.CEST,
                request.OrigemMercadoria,
                request.CST,
                request.CSOSN,
                request.EAN,
                request.EANTrib,
                request.UN,
                request.VlrCusto,
                request.VlrVenda,
                request.EstoqueMinimo,
                request.PesoLiquido,
                request.PesoBruto,
                request.Altura,
                request.Largura,
                request.Profundidade,
                request.InfoAdicional);

            await _produtoRepositorio.AtualizarAsync(produto);

            return ProdutoMapper.ParaResponse(produto);
        }
    }
}
