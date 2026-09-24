using DotNetCleanArchitecture.Domain.Entidades;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Tests.Fakes;

public class FakeMovimentoEstoqueRepositorio : IMovimentoEstoqueRepositorio
{
    private readonly List<MovimentoEstoque> _movimentos = new();

    public IReadOnlyCollection<MovimentoEstoque> Movimentos => _movimentos.AsReadOnly();

    public Task AdicionarAsync(MovimentoEstoque movimento)
    {
        _movimentos.Add(movimento);
        return Task.CompletedTask;
    }
}
