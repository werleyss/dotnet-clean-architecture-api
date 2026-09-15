using DotNetCleanArchitecture.Domain.Core.ObjetosValor;
using DotNetCleanArchitecture.Domain.Entidades;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Tests.Fakes;

public class FakeClienteRepositorio : IClienteRepositorio
{
    private readonly Dictionary<Guid, Cliente> _clientes = new();

    public Task AdicionarAsync(Cliente cliente)
    {
        _clientes[cliente.Id] = cliente;
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Cliente cliente)
    {
        _clientes[cliente.Id] = cliente;
        return Task.CompletedTask;
    }

    public Task<Cliente?> ObterPorIdAsync(Guid id)
        => Task.FromResult(_clientes.GetValueOrDefault(id));

    public Task<bool> ExisteComDocumentoAsync(Documento documento)
        => Task.FromResult(_clientes.Values.Any(c =>
            c.Documento.Tipo == documento.Tipo &&
            c.Documento.Numero == documento.Numero));
}
