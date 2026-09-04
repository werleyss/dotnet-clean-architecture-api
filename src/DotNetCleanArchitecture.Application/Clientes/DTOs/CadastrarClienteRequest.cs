using DotNetCleanArchitecture.Domain.Core.Enum;

namespace DotNetCleanArchitecture.Application.Clientes.DTOs
{
    public record CadastrarClienteRequest(
        string Nome,
        string? Fantasia,
        TipoDocumento TipoDocumento,
        string NumeroDocumento,
        IndicadorIE IndicadorIE,
        string? IE,
        string? IM,
        string? Celular,
        string? Fone,
        string? Email,
        EnderecoRequest Endereco);
}
