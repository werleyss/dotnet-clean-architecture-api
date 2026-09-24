using DotNetCleanArchitecture.Application.Nfces.DTOs;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Nfces
{
    public class AutorizarNfceUseCase
    {
        private readonly INfceRepositorio _nfceRepositorio;

        public AutorizarNfceUseCase(INfceRepositorio nfceRepositorio)
        {
            _nfceRepositorio = nfceRepositorio;
        }

        public async Task<NfceResponse> ExecutarAsync(AutorizarNfceRequest request)
        {
            var nfce = await _nfceRepositorio.ObterPorIdAsync(request.NfceId)
                ?? throw new ExcecaoDeDominio("NFC-e não encontrada.");

            nfce.Autorizar(request.ChaveAcesso, request.ProtocoloAutorizacao, request.QrCode);

            await _nfceRepositorio.AtualizarAsync(nfce);

            return NfceMapper.ParaResponse(nfce);
        }
    }
}
