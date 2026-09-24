using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Domain.Interfaces
{
    public interface IMovimentoEstoqueRepositorio
    {
        Task AdicionarAsync(MovimentoEstoque movimento);
    }
}
