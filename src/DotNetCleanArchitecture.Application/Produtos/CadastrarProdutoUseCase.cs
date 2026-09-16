using DotNetCleanArchitecture.Application.Produtos.DTOs;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Produtos
{
    public class CadastrarProdutoUseCase
    {
        private readonly IProdutoRepositorio _produtoRepositorio;

        public CadastrarProdutoUseCase(IProdutoRepositorio produtoRepositorio)
        {
            _produtoRepositorio = produtoRepositorio;
        }

        public async Task<ProdutoResponse> ExecutarAsync(CadastrarProdutoRequest request)
        {
            if (await _produtoRepositorio.ExisteComCodigoAsync(request.Codigo))
                throw new ExcecaoDeDominio(
                    "Já existe um produto cadastrado com esse código.");

            var produto = new Produto(
                request.Codigo,
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
                request.EstoqueAtual,
                request.EstoqueMinimo,
                request.PesoLiquido,
                request.PesoBruto,
                request.Altura,
                request.Largura,
                request.Profundidade,
                request.InfoAdicional,
                request.Ativo);

            await _produtoRepositorio.AdicionarAsync(produto);

            return ProdutoMapper.ParaResponse(produto);
        }
    }
}
