using DotNetCleanArchitecture.Application.Vendas.DTOs;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Vendas
{
    public class CancelarVendaUseCase
    {
        private readonly IVendaRepositorio _vendaRepositorio;

        public CancelarVendaUseCase(IVendaRepositorio vendaRepositorio)
        {
            _vendaRepositorio = vendaRepositorio;
        }

        public async Task<VendaResponse> ExecutarAsync(CancelarVendaRequest request)
        {
            var venda = await _vendaRepositorio.ObterPorIdAsync(request.VendaId)
                ?? throw new ExcecaoDeDominio("Venda não encontrada.");

            venda.Cancelar();

            await _vendaRepositorio.AtualizarAsync(venda);

            return VendaMapper.ParaResponse(venda);
        }
    }
}
