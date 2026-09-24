namespace DotNetCleanArchitecture.Application.Vendas.DTOs
{
    public record AdicionarItemVendaRequest(
        Guid VendaId,
        Guid ProdutoId,
        decimal Quantidade,
        decimal ValorDesconto = 0);
}
