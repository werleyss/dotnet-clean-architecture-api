namespace DotNetCleanArchitecture.Application.Vendas.DTOs
{
    public record CriarVendaRequest(
        int Numero,
        Guid EmpresaId,
        Guid ClienteId,
        DateTime DataEmissao,
        string NaturezaOperacao,
        string CFOP,
        string? Observacoes);
}
