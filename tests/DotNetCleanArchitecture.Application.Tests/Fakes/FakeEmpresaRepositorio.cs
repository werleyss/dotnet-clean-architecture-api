using DotNetCleanArchitecture.Domain.Core.ObjetosValor;
using DotNetCleanArchitecture.Domain.Entidades;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Tests.Fakes;

public class FakeEmpresaRepositorio : IEmpresaRepositorio
{
    private readonly Dictionary<Guid, Empresa> _empresas = new();

    public Task AdicionarAsync(Empresa empresa)
    {
        _empresas[empresa.Id] = empresa;
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Empresa empresa)
    {
        _empresas[empresa.Id] = empresa;
        return Task.CompletedTask;
    }

    public Task<Empresa?> ObterPorIdAsync(Guid id)
        => Task.FromResult(_empresas.GetValueOrDefault(id));

    public Task<bool> ExisteComDocumentoAsync(Documento documento)
        => Task.FromResult(_empresas.Values.Any(e =>
            e.Documento.Tipo == documento.Tipo &&
            e.Documento.Numero == documento.Numero));
}
