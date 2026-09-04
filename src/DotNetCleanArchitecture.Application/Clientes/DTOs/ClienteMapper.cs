using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Application.Clientes.DTOs
{
    internal static class ClienteMapper
    {
        public static ClienteResponse ParaResponse(Cliente cliente)
            => new(
                cliente.Id,
                cliente.Nome,
                cliente.Fantasia,
                cliente.Documento.Tipo.ToString(),
                cliente.Documento.Numero,
                cliente.IndicadorIE.ToString(),
                cliente.IE,
                cliente.IM,
                cliente.Celular,
                cliente.Fone,
                cliente.Email,
                cliente.Endereco.Cidade,
                cliente.Endereco.Uf);
    }
}
