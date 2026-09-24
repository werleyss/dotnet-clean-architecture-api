using DotNetCleanArchitecture.Domain.Core.ObjetosValor;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Domain.Interfaces
{
    public interface IFornecedorRepositorio
    {
        Task AdicionarAsync(Fornecedor fornecedor);

        Task AtualizarAsync(Fornecedor fornecedor);

        Task<Fornecedor?> ObterPorIdAsync(Guid id);

        Task<bool> ExisteComDocumentoAsync(Documento documento);
    }
}
