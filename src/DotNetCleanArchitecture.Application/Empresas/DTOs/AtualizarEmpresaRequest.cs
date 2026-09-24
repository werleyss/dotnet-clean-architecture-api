using DotNetCleanArchitecture.Domain.Core.Enum;

namespace DotNetCleanArchitecture.Application.Empresas.DTOs
{
    public record AtualizarEmpresaRequest(
        Guid Id,
        string Nome,
        string Fantasia,
        CRT CRT,
        string? IE,
        string? IEST,
        string? IM,
        string? CNAE,
        string? Fone,
        EnderecoRequest Endereco);
}
