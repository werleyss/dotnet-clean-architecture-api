using DotNetCleanArchitecture.Application.Nfes.DTOs;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Nfes
{
    public class EmitirNfeUseCase
    {
        private readonly IVendaRepositorio _vendaRepositorio;
        private readonly INfeRepositorio _nfeRepositorio;

        public EmitirNfeUseCase(IVendaRepositorio vendaRepositorio, INfeRepositorio nfeRepositorio)
        {
            _vendaRepositorio = vendaRepositorio;
            _nfeRepositorio = nfeRepositorio;
        }

        public async Task<NfeResponse> ExecutarAsync(EmitirNfeRequest request)
        {
            var venda = await _vendaRepositorio.ObterPorIdAsync(request.VendaId)
                ?? throw new ExcecaoDeDominio("Venda não encontrada.");

            if (venda.Status != StatusVenda.Faturada)
                throw new ExcecaoDeDominio("Somente uma venda faturada pode gerar NF-e.");

            var nfe = new Nfe(
                venda.Id,
                request.Numero,
                request.Serie,
                request.Ambiente,
                request.ModalidadeFrete,
                request.InformacoesComplementares,
                request.TipoEmissao);

            await _nfeRepositorio.AdicionarAsync(nfe);

            return NfeMapper.ParaResponse(nfe);
        }
    }
}
