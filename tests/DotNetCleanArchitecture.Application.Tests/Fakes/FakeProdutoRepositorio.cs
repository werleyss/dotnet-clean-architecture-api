using DotNetCleanArchitecture.Domain.Entidades;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Tests.Fakes;

public class FakeProdutoRepositorio : IProdutoRepositorio
{
    private readonly Dictionary<Guid, Produto> _produtos = new();

    public Task AdicionarAsync(Produto produto)
    {
        _produtos[produto.Id] = produto;
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Produto produto)
    {
        _produtos[produto.Id] = produto;
        return Task.CompletedTask;
    }

    public Task<Produto?> ObterPorIdAsync(Guid id)
        => Task.FromResult(_produtos.GetValueOrDefault(id));

    public Task<bool> ExisteComCodigoAsync(string codigo)
        => Task.FromResult(_produtos.Values.Any(p =>
            p.Codigo.Equals(codigo, StringComparison.OrdinalIgnoreCase)));
}
