namespace DotNetCleanArchitecture.Application.Empresas.DTOs
{
    public record EmpresaResponse(
        Guid Id,
        string Nome,
        string Fantasia,
        string CRT,
        string TipoDocumento,
        string NumeroDocumento,
        string? IE,
        string? IEST,
        string? IM,
        string? CNAE,
        string? Fone,
        string Cidade,
        string Uf);
}
