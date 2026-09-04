using DotNetCleanArchitecture.Application.Clientes.DTOs;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Core.ObjetosValor;
using DotNetCleanArchitecture.Domain.Entidades;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Clientes
{
    public class CadastrarClienteUseCase
    {
        private readonly IClienteRepositorio _clienteRepositorio;

        public CadastrarClienteUseCase(IClienteRepositorio clienteRepositorio)
        {
            _clienteRepositorio = clienteRepositorio;
        }

        public async Task<ClienteResponse> ExecutarAsync(CadastrarClienteRequest request)
        {
            var documento = CriarDocumento(request.TipoDocumento, request.NumeroDocumento);

            if (await _clienteRepositorio.ExisteComDocumentoAsync(documento))
                throw new ExcecaoDeDominio(
                    "Já existe um cliente cadastrado com esse documento.");

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

            var cliente = new Cliente(
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

            await _clienteRepositorio.AdicionarAsync(cliente);

            return ClienteMapper.ParaResponse(cliente);
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
