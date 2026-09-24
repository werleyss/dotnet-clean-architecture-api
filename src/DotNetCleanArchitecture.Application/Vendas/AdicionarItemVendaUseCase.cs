using DotNetCleanArchitecture.Application.Vendas.DTOs;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Vendas
{
    public class AdicionarItemVendaUseCase
    {
        private readonly IVendaRepositorio _vendaRepositorio;
        private readonly IProdutoRepositorio _produtoRepositorio;

        public AdicionarItemVendaUseCase(IVendaRepositorio vendaRepositorio,
                                         IProdutoRepositorio produtoRepositorio)
        {
            _vendaRepositorio = vendaRepositorio;
            _produtoRepositorio = produtoRepositorio;
        }

        public async Task<VendaResponse> ExecutarAsync(AdicionarItemVendaRequest request)
        {
            var venda = await _vendaRepositorio.ObterPorIdAsync(request.VendaId)
                ?? throw new ExcecaoDeDominio("Venda não encontrada.");

            var produto = await _produtoRepositorio.ObterPorIdAsync(request.ProdutoId)
                ?? throw new ExcecaoDeDominio("Produto não encontrado.");

            if (!produto.Ativo)
                throw new ExcecaoDeDominio("Não é possível vender um produto inativo.");

            // Preço e situação tributária vêm do cadastro do produto (snapshot), não do
            // cliente da requisição — evita que o preço da venda seja manipulado.
            venda.AdicionarItem(
                produto.Id,
                request.Quantidade,
                produto.VlrVenda,
                request.ValorDesconto,
                produto.CST,
                produto.CSOSN);

            await _vendaRepositorio.AtualizarAsync(venda);

            return VendaMapper.ParaResponse(venda);
        }
    }
}
