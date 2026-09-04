using DotNetCleanArchitecture.Domain.Core.Enum;

namespace DotNetCleanArchitecture.Application.Clientes.DTOs
{
    public record AtualizarClienteRequest(
        Guid Id,
        string Nome,
        string? Fantasia,
        IndicadorIE IndicadorIE,
        string? IE,
        string? IM,
        string? Celular,
        string? Fone,
        string? Email,
        EnderecoRequest Endereco);
}
