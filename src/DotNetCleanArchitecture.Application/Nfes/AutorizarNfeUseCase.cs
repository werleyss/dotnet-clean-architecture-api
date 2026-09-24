using DotNetCleanArchitecture.Application.Nfes.DTOs;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Nfes
{
    public class AutorizarNfeUseCase
    {
        private readonly INfeRepositorio _nfeRepositorio;

        public AutorizarNfeUseCase(INfeRepositorio nfeRepositorio)
        {
            _nfeRepositorio = nfeRepositorio;
        }

        public async Task<NfeResponse> ExecutarAsync(AutorizarNfeRequest request)
        {
            var nfe = await _nfeRepositorio.ObterPorIdAsync(request.NfeId)
                ?? throw new ExcecaoDeDominio("NF-e não encontrada.");

            nfe.Autorizar(request.ChaveAcesso, request.ProtocoloAutorizacao);

            await _nfeRepositorio.AtualizarAsync(nfe);

            return NfeMapper.ParaResponse(nfe);
        }
    }
}
