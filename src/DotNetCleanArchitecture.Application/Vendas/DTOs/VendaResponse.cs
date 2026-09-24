namespace DotNetCleanArchitecture.Application.Vendas.DTOs
{
    public record VendaResponse(
        Guid Id,
        int Numero,
        string Status,
        decimal ValorProdutos,
        decimal ValorDesconto,
        decimal ValorFrete,
        decimal ValorOutrasDespesas,
        decimal ValorTotal,
        int QuantidadeItens,
        int QuantidadePagamentos);
}
