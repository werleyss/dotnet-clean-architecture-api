using DotNetCleanArchitecture.Application.Nfes.DTOs;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Nfes
{
    public class DenegarNfeUseCase
    {
        private readonly INfeRepositorio _nfeRepositorio;

        public DenegarNfeUseCase(INfeRepositorio nfeRepositorio)
        {
            _nfeRepositorio = nfeRepositorio;
        }

        public async Task<NfeResponse> ExecutarAsync(DenegarNfeRequest request)
        {
            var nfe = await _nfeRepositorio.ObterPorIdAsync(request.NfeId)
                ?? throw new ExcecaoDeDominio("NF-e não encontrada.");

            nfe.Denegar(request.Motivo);

            await _nfeRepositorio.AtualizarAsync(nfe);

            return NfeMapper.ParaResponse(nfe);
        }
    }
}
