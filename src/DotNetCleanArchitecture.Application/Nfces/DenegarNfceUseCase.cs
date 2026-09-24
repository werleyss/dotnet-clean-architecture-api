using DotNetCleanArchitecture.Application.Nfces.DTOs;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Nfces
{
    public class DenegarNfceUseCase
    {
        private readonly INfceRepositorio _nfceRepositorio;

        public DenegarNfceUseCase(INfceRepositorio nfceRepositorio)
        {
            _nfceRepositorio = nfceRepositorio;
        }

        public async Task<NfceResponse> ExecutarAsync(DenegarNfceRequest request)
        {
            var nfce = await _nfceRepositorio.ObterPorIdAsync(request.NfceId)
                ?? throw new ExcecaoDeDominio("NFC-e não encontrada.");

            nfce.Denegar(request.Motivo);

            await _nfceRepositorio.AtualizarAsync(nfce);

            return NfceMapper.ParaResponse(nfce);
        }
    }
}
