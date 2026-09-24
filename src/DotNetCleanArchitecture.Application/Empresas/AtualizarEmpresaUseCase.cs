using DotNetCleanArchitecture.Application.Empresas.DTOs;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Core.ObjetosValor;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Empresas
{
    public class AtualizarEmpresaUseCase
    {
        private readonly IEmpresaRepositorio _empresaRepositorio;

        public AtualizarEmpresaUseCase(IEmpresaRepositorio empresaRepositorio)
        {
            _empresaRepositorio = empresaRepositorio;
        }

        public async Task<EmpresaResponse> ExecutarAsync(AtualizarEmpresaRequest request)
        {
            var empresa = await _empresaRepositorio.ObterPorIdAsync(request.Id)
                ?? throw new ExcecaoDeDominio("Empresa não encontrada.");

            var endereco = Endereco.Criar(
                request.Endereco.Logradouro,
                request.Endereco.Numero,
                request.Endereco.Complemento,
                request.Endereco.Bairro,
                request.Endereco.CodigoIBGE,
                request.Endereco.Cidade,
                request.Endereco.CodigoUf,
                request.Endereco.Uf,
                request.Endereco.Cep,
                request.Endereco.CodigoPais,
                request.Endereco.Pais);

            empresa.Atualizar(
                request.Nome,
                request.Fantasia,
                request.CRT,
                request.IE,
                request.IEST,
                request.IM,
                request.CNAE,
                request.Fone,
                endereco);

            await _empresaRepositorio.AtualizarAsync(empresa);

            return EmpresaMapper.ParaResponse(empresa);
        }
    }
}
