using DotNetCleanArchitecture.Domain.Core.ObjetosValor;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Domain.Interfaces
{
    public interface IClienteRepositorio
    {
        Task AdicionarAsync(Cliente cliente);

        Task AtualizarAsync(Cliente cliente);

        Task<Cliente?> ObterPorIdAsync(Guid id);

        Task<bool> ExisteComDocumentoAsync(Documento documento);
    }
}
