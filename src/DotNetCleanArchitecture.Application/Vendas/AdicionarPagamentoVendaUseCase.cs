using DotNetCleanArchitecture.Application.Vendas.DTOs;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Vendas
{
    public class AdicionarPagamentoVendaUseCase
    {
        private readonly IVendaRepositorio _vendaRepositorio;

        public AdicionarPagamentoVendaUseCase(IVendaRepositorio vendaRepositorio)
        {
            _vendaRepositorio = vendaRepositorio;
        }

        public async Task<VendaResponse> ExecutarAsync(AdicionarPagamentoVendaRequest request)
        {
            var venda = await _vendaRepositorio.ObterPorIdAsync(request.VendaId)
                ?? throw new ExcecaoDeDominio("Venda não encontrada.");

            venda.AdicionarPagamento(request.FormaPagamento, request.Valor, request.QtdParcelas);

            await _vendaRepositorio.AtualizarAsync(venda);

            return VendaMapper.ParaResponse(venda);
        }
    }
}
