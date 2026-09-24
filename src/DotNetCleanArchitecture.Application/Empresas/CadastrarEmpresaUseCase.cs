using DotNetCleanArchitecture.Application.Empresas.DTOs;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Core.ObjetosValor;
using DotNetCleanArchitecture.Domain.Entidades;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Empresas
{
    public class CadastrarEmpresaUseCase
    {
        private readonly IEmpresaRepositorio _empresaRepositorio;

        public CadastrarEmpresaUseCase(IEmpresaRepositorio empresaRepositorio)
        {
            _empresaRepositorio = empresaRepositorio;
        }

        public async Task<EmpresaResponse> ExecutarAsync(CadastrarEmpresaRequest request)
        {
            var documento = CriarDocumento(request.TipoDocumento, request.NumeroDocumento);

            if (await _empresaRepositorio.ExisteComDocumentoAsync(documento))
                throw new ExcecaoDeDominio(
                    "Já existe uma empresa cadastrada com esse documento.");

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

            var empresa = new Empresa(
                request.Nome,
                request.Fantasia,
                request.CRT,
                documento,
                request.IE,
                request.IEST,
                request.IM,
                request.CNAE,
                request.Fone,
                endereco);

            await _empresaRepositorio.AdicionarAsync(empresa);

            return EmpresaMapper.ParaResponse(empresa);
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
