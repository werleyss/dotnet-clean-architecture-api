using DotNetCleanArchitecture.Domain.Core.ObjetosValor;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Domain.Interfaces
{
    public interface IEmpresaRepositorio
    {
        Task AdicionarAsync(Empresa empresa);

        Task AtualizarAsync(Empresa empresa);

        Task<Empresa?> ObterPorIdAsync(Guid id);

        Task<bool> ExisteComDocumentoAsync(Documento documento);
    }
}
