using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Application.Vendas.DTOs
{
    internal static class VendaMapper
    {
        public static VendaResponse ParaResponse(Venda venda)
            => new(
                venda.Id,
                venda.Numero,
                venda.Status.ToString(),
                venda.ValorProdutos,
                venda.ValorDesconto,
                venda.ValorFrete,
                venda.ValorOutrasDespesas,
                venda.ValorTotal,
                venda.Itens.Count,
                venda.Pagamentos.Count);
    }
}
