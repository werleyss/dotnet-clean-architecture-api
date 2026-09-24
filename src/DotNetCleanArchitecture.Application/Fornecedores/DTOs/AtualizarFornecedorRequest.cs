using DotNetCleanArchitecture.Domain.Core.Enum;

namespace DotNetCleanArchitecture.Application.Fornecedores.DTOs
{
    public record AtualizarFornecedorRequest(
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
