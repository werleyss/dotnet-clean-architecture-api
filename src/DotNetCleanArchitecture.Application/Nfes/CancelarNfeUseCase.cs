using DotNetCleanArchitecture.Application.Nfes.DTOs;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Nfes
{
    public class CancelarNfeUseCase
    {
        private readonly INfeRepositorio _nfeRepositorio;

        public CancelarNfeUseCase(INfeRepositorio nfeRepositorio)
        {
            _nfeRepositorio = nfeRepositorio;
        }

        public async Task<NfeResponse> ExecutarAsync(CancelarNfeRequest request)
        {
            var nfe = await _nfeRepositorio.ObterPorIdAsync(request.NfeId)
                ?? throw new ExcecaoDeDominio("NF-e não encontrada.");

            nfe.Cancelar(request.ProtocoloCancelamento, request.Justificativa);

            await _nfeRepositorio.AtualizarAsync(nfe);

            return NfeMapper.ParaResponse(nfe);
        }
    }
}
