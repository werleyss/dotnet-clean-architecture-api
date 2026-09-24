using DotNetCleanArchitecture.Application.Nfces.DTOs;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Nfces
{
    public class EmitirNfceUseCase
    {
        private readonly IVendaRepositorio _vendaRepositorio;
        private readonly INfceRepositorio _nfceRepositorio;

        public EmitirNfceUseCase(IVendaRepositorio vendaRepositorio, INfceRepositorio nfceRepositorio)
        {
            _vendaRepositorio = vendaRepositorio;
            _nfceRepositorio = nfceRepositorio;
        }

        public async Task<NfceResponse> ExecutarAsync(EmitirNfceRequest request)
        {
            var venda = await _vendaRepositorio.ObterPorIdAsync(request.VendaId)
                ?? throw new ExcecaoDeDominio("Venda não encontrada.");

            if (venda.Status != StatusVenda.Faturada)
                throw new ExcecaoDeDominio("Somente uma venda faturada pode gerar NFC-e.");

            var nfce = new Nfce(venda.Id, request.Numero, request.Serie, request.Ambiente, request.TipoEmissao);

            await _nfceRepositorio.AdicionarAsync(nfce);

            return NfceMapper.ParaResponse(nfce);
        }
    }
}
