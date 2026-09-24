using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Domain.Interfaces
{
    public interface INfeRepositorio
    {
        Task AdicionarAsync(Nfe nfe);

        Task AtualizarAsync(Nfe nfe);

        Task<Nfe?> ObterPorIdAsync(Guid id);
    }
}
