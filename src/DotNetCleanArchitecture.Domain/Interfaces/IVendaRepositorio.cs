using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Domain.Interfaces
{
    public interface IVendaRepositorio
    {
        Task AdicionarAsync(Venda venda);

        Task AtualizarAsync(Venda venda);

        Task<Venda?> ObterPorIdAsync(Guid id);
    }
}
