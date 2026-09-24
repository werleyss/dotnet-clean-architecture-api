namespace DotNetCleanArchitecture.Application.Fornecedores.DTOs
{
    public record FornecedorResponse(
        Guid Id,
        string Nome,
        string? Fantasia,
        string TipoDocumento,
        string NumeroDocumento,
        string IndicadorIE,
        string? IE,
        string? IM,
        string? Celular,
        string? Fone,
        string? Email,
        string Cidade,
        string Uf);
}
