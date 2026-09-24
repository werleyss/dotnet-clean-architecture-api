using DotNetCleanArchitecture.Application.Fornecedores.DTOs;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Core.ObjetosValor;
using DotNetCleanArchitecture.Domain.Entidades;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Fornecedores
{
    public class CadastrarFornecedorUseCase
    {
        private readonly IFornecedorRepositorio _fornecedorRepositorio;

        public CadastrarFornecedorUseCase(IFornecedorRepositorio fornecedorRepositorio)
        {
            _fornecedorRepositorio = fornecedorRepositorio;
        }

        public async Task<FornecedorResponse> ExecutarAsync(CadastrarFornecedorRequest request)
        {
            var documento = CriarDocumento(request.TipoDocumento, request.NumeroDocumento);

            if (await _fornecedorRepositorio.ExisteComDocumentoAsync(documento))
                throw new ExcecaoDeDominio(
                    "Já existe um fornecedor cadastrado com esse documento.");

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

            var fornecedor = new Fornecedor(
                request.Nome,
                request.Fantasia,
                documento,
                request.IndicadorIE,
                request.IE,
                request.IM,
                request.Celular,
                request.Fone,
                request.Email,
                endereco);

            await _fornecedorRepositorio.AdicionarAsync(fornecedor);

            return FornecedorMapper.ParaResponse(fornecedor);
        }

        private static Documento CriarDocumento(TipoDocumento tipo, string numero)
            => tipo switch
            {
                TipoDocumento.CPF => Cpf.Criar(numero),
                TipoDocumento.CNPJ => Cnpj.Criar(numero),
                _ => throw new ExcecaoDeDominio("O tipo de documento informado é inválido.")
            };
    }
}
