using DotNetCleanArchitecture.Domain.Core.ObjetosValor;
using DotNetCleanArchitecture.Domain.Entidades;
using DotNetCleanArchitecture.Domain.Interfaces;

namespace DotNetCleanArchitecture.Application.Tests.Fakes;

public class FakeFornecedorRepositorio : IFornecedorRepositorio
{
    private readonly Dictionary<Guid, Fornecedor> _fornecedores = new();

    public Task AdicionarAsync(Fornecedor fornecedor)
    {
        _fornecedores[fornecedor.Id] = fornecedor;
        return Task.CompletedTask;
    }

    public Task AtualizarAsync(Fornecedor fornecedor)
    {
        _fornecedores[fornecedor.Id] = fornecedor;
        return Task.CompletedTask;
    }

    public Task<Fornecedor?> ObterPorIdAsync(Guid id)
        => Task.FromResult(_fornecedores.GetValueOrDefault(id));

    public Task<bool> ExisteComDocumentoAsync(Documento documento)
        => Task.FromResult(_fornecedores.Values.Any(f =>
            f.Documento.Tipo == documento.Tipo &&
            f.Documento.Numero == documento.Numero));
}
