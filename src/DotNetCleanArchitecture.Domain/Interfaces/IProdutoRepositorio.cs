using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Domain.Interfaces
{
    public interface IProdutoRepositorio
    {
        Task AdicionarAsync(Produto produto);

        Task AtualizarAsync(Produto produto);

        Task<Produto?> ObterPorIdAsync(Guid id);

        Task<bool> ExisteComCodigoAsync(string codigo);
    }
}
