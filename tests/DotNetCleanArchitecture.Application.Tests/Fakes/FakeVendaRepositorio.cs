using DotNetCleanArchitecture.Domain.Entidades;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Tests.Fakes;

public class FakeVendaRepositorio : IVendaRepositorio
{
    private readonly Dictionary<Guid, Venda> _vendas = new();

    public Task AdicionarAsync(Venda venda)
    {
        _vendas[venda.Id] = venda;
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Venda venda)
    {
        _vendas[venda.Id] = venda;
        return Task.CompletedTask;
    }

    public Task<Venda?> ObterPorIdAsync(Guid id)
        => Task.FromResult(_vendas.GetValueOrDefault(id));
}
