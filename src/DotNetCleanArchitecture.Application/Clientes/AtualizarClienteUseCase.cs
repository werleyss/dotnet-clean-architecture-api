using DotNetCleanArchitecture.Application.Clientes.DTOs;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Core.ObjetosValor;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Clientes
{
    public class AtualizarClienteUseCase
    {
        private readonly IClienteRepositorio _clienteRepositorio;

        public AtualizarClienteUseCase(IClienteRepositorio clienteRepositorio)
        {
            _clienteRepositorio = clienteRepositorio;
        }

        public async Task<ClienteResponse> ExecutarAsync(AtualizarClienteRequest request)
        {
            var cliente = await _clienteRepositorio.ObterPorIdAsync(request.Id)
                ?? throw new ExcecaoDeDominio("Cliente não encontrado.");

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

            cliente.Atualizar(
                request.Nome,
                request.Fantasia,
                request.IndicadorIE,
                request.IE,
                request.IM,
                request.Celular,
                request.Fone,
                request.Email,
                endereco);

            await _clienteRepositorio.AtualizarAsync(cliente);

            return ClienteMapper.ParaResponse(cliente);
        }
    }
}
