using DotNetCleanArchitecture.Application.Vendas.DTOs;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Vendas
{
    public class FaturarVendaUseCase
    {
        private readonly IVendaRepositorio _vendaRepositorio;
        private readonly IProdutoRepositorio _produtoRepositorio;
        private readonly IMovimentoEstoqueRepositorio _movimentoEstoqueRepositorio;

        public FaturarVendaUseCase(IVendaRepositorio vendaRepositorio,
                                   IProdutoRepositorio produtoRepositorio,
                                   IMovimentoEstoqueRepositorio movimentoEstoqueRepositorio)
        {
            _vendaRepositorio = vendaRepositorio;
            _produtoRepositorio = produtoRepositorio;
            _movimentoEstoqueRepositorio = movimentoEstoqueRepositorio;
        }

        public async Task<VendaResponse> ExecutarAsync(FaturarVendaRequest request)
        {
            var venda = await _vendaRepositorio.ObterPorIdAsync(request.VendaId)
                ?? throw new ExcecaoDeDominio("Venda não encontrada.");

            // Dispara as invariantes da própria Venda (itens, pagamentos, soma dos pagamentos).
            venda.Faturar();

            foreach (var item in venda.Itens)
            {
                var produto = await _produtoRepositorio.ObterPorIdAsync(item.ProdutoId)
                    ?? throw new ExcecaoDeDominio(
                        $"O produto do item da venda não foi encontrado (id: {item.ProdutoId}).");

                var saldoAnterior = produto.EstoqueAtual;
                produto.RegistrarSaidaEstoque(item.Quantidade);

                var movimento = new MovimentoEstoque(
                    produto.Id,
                    TipoMovimentoEstoque.Saida,
                    OrigemMovimentoEstoque.Venda,
                    item.Quantidade,
                    saldoAnterior,
                    documentoOrigemId: venda.Id);

                await _produtoRepositorio.AtualizarAsync(produto);
                await _movimentoEstoqueRepositorio.AdicionarAsync(movimento);
            }

            await _vendaRepositorio.AtualizarAsync(venda);

            return VendaMapper.ParaResponse(venda);
        }
    }
}
