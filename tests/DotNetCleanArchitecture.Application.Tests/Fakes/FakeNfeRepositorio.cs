using DotNetCleanArchitecture.Domain.Entidades;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Tests.Fakes;

public class FakeNfeRepositorio : INfeRepositorio
{
    private readonly Dictionary<Guid, Nfe> _nfes = new();

    public Task AdicionarAsync(Nfe nfe)
    {
        _nfes[nfe.Id] = nfe;
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Nfe nfe)
    {
        _nfes[nfe.Id] = nfe;
        return Task.CompletedTask;
    }

    public Task<Nfe?> ObterPorIdAsync(Guid id)
        => Task.FromResult(_nfes.GetValueOrDefault(id));
}
