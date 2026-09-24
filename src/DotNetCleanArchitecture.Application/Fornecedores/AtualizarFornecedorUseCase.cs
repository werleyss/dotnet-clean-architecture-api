using DotNetCleanArchitecture.Application.Fornecedores.DTOs;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Core.ObjetosValor;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Fornecedores
{
    public class AtualizarFornecedorUseCase
    {
        private readonly IFornecedorRepositorio _fornecedorRepositorio;

        public AtualizarFornecedorUseCase(IFornecedorRepositorio fornecedorRepositorio)
        {
            _fornecedorRepositorio = fornecedorRepositorio;
        }

        public async Task<FornecedorResponse> ExecutarAsync(AtualizarFornecedorRequest request)
        {
            var fornecedor = await _fornecedorRepositorio.ObterPorIdAsync(request.Id)
                ?? throw new ExcecaoDeDominio("Fornecedor não encontrado.");

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

            fornecedor.Atualizar(
                request.Nome,
                request.Fantasia,
                request.IndicadorIE,
                request.IE,
                request.IM,
                request.Celular,
                request.Fone,
                request.Email,
                endereco);

            await _fornecedorRepositorio.AtualizarAsync(fornecedor);

            return FornecedorMapper.ParaResponse(fornecedor);
        }
    }
}
