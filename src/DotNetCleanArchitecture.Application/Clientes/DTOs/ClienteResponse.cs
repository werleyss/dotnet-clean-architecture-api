namespace DotNetCleanArchitecture.Application.Clientes.DTOs
{
    public record ClienteResponse(
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
