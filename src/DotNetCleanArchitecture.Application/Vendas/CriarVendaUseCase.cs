using DotNetCleanArchitecture.Application.Vendas.DTOs;
using DotNetCleanArchitecture.Domain.Entidades;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Vendas
{
    public class CriarVendaUseCase
    {
        private readonly IVendaRepositorio _vendaRepositorio;

        public CriarVendaUseCase(IVendaRepositorio vendaRepositorio)
        {
            _vendaRepositorio = vendaRepositorio;
        }

        public async Task<VendaResponse> ExecutarAsync(CriarVendaRequest request)
        {
            var venda = new Venda(
                request.Numero,
                request.EmpresaId,
                request.ClienteId,
                request.DataEmissao,
                request.NaturezaOperacao,
                request.CFOP,
                request.Observacoes);

            await _vendaRepositorio.AdicionarAsync(venda);

            return VendaMapper.ParaResponse(venda);
        }
    }
}
