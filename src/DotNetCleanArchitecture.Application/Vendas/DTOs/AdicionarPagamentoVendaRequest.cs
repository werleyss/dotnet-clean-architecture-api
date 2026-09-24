using DotNetCleanArchitecture.Domain.Core.Enum;

namespace DotNetCleanArchitecture.Application.Vendas.DTOs
{
    public record AdicionarPagamentoVendaRequest(
        Guid VendaId,
        TipoPagamento FormaPagamento,
        decimal Valor,
        int QtdParcelas = 1);
}
