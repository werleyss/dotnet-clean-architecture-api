using DotNetCleanArchitecture.Domain.Entidades;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Tests.Fakes;

public class FakeNfceRepositorio : INfceRepositorio
{
    private readonly Dictionary<Guid, Nfce> _nfces = new();

    public Task AdicionarAsync(Nfce nfce)
    {
        _nfces[nfce.Id] = nfce;
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Nfce nfce)
    {
        _nfces[nfce.Id] = nfce;
        return Task.CompletedTask;
    }

    public Task<Nfce?> ObterPorIdAsync(Guid id)
        => Task.FromResult(_nfces.GetValueOrDefault(id));
}
