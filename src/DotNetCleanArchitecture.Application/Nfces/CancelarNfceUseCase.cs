using DotNetCleanArchitecture.Application.Nfces.DTOs;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Nfces
{
    public class CancelarNfceUseCase
    {
        private readonly INfceRepositorio _nfceRepositorio;

        public CancelarNfceUseCase(INfceRepositorio nfceRepositorio)
        {
            _nfceRepositorio = nfceRepositorio;
        }

        public async Task<NfceResponse> ExecutarAsync(CancelarNfceRequest request)
        {
            var nfce = await _nfceRepositorio.ObterPorIdAsync(request.NfceId)
                ?? throw new ExcecaoDeDominio("NFC-e não encontrada.");

            nfce.Cancelar(request.ProtocoloCancelamento, request.Justificativa);

            await _nfceRepositorio.AtualizarAsync(nfce);

            return NfceMapper.ParaResponse(nfce);
        }
    }
}
