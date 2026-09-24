using DotNetCleanArchitecture.Domain.Core.Enum;

namespace DotNetCleanArchitecture.Application.Empresas.DTOs
{
    public record CadastrarEmpresaRequest(
        string Nome,
        string Fantasia,
        CRT CRT,
        TipoDocumento TipoDocumento,
        string NumeroDocumento,
        string? IE,
        string? IEST,
        string? IM,
        string? CNAE,
        string? Fone,
        EnderecoRequest Endereco);
}
