using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Domain.Interfaces
{
    public interface INfceRepositorio
    {
        Task AdicionarAsync(Nfce nfce);

        Task AtualizarAsync(Nfce nfce);

        Task<Nfce?> ObterPorIdAsync(Guid id);
    }
}
