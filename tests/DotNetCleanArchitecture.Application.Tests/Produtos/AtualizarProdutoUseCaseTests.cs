using DotNetCleanArchitecture.Application.Produtos;
using DotNetCleanArchitecture.Application.Produtos.DTOs;
using DotNetCleanArchitecture.Application.Tests.Fakes;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;

namespace DotNetCleanArchitecture.Application.Tests.Produtos;

public class AtualizarProdutoUseCaseTests
{
    private static async Task<(FakeProdutoRepositorio repositorio, Guid produtoId)> CadastrarProdutoAsync()
    {
        var repositorio = new FakeProdutoRepositorio();
        var cadastrar = new CadastrarProdutoUseCase(repositorio);

        var response = await cadastrar.ExecutarAsync(new CadastrarProdutoRequest(
            "P-001",
            "Cadeira de Escritório",
            TipoProduto.MercadoriaParaRevenda,
            "94013000",
            null,
            OrigemMercadoria.Nacional,
            null,
            null,
            null,
            null,
            "UN",
            120.00m,
            249.90m,
            EstoqueAtual: 10m,
            EstoqueMinimo: 2m,
            PesoLiquido: 1m,
            PesoBruto: 1m,
            Altura: 1m,
            Largura: 1m,
            Profundidade: 1m,
            InfoAdicional: null));

        return (repositorio, response.Id);
    }

    private static AtualizarProdutoRequest RequestValido(Guid id, decimal vlrVenda = 279.90m)
        => new(
            id,
            "Cadeira de Escritório Ergonômica",
            TipoProduto.MercadoriaParaRevenda,
            "94013000",
            null,
            OrigemMercadoria.Nacional,
            null,
            null,
            null,
            null,
            "UN",
            120.00m,
            vlrVenda,
            EstoqueMinimo: 3m,
            PesoLiquido: 1m,
            PesoBruto: 1m,
            Altura: 1m,
            Largura: 1m,
            Profundidade: 1m,
            InfoAdicional: "Com apoio de braço");

    [Fact]
    public async Task Deve_atualizar_dados_do_produto()
    {
        var (repositorio, produtoId) = await CadastrarProdutoAsync();
        var useCase = new AtualizarProdutoUseCase(repositorio);

        var response = await useCase.ExecutarAsync(RequestValido(produtoId));

        Assert.Equal("Cadeira de Escritório Ergonômica", response.Descricao);
        Assert.Equal(279.90m, response.VlrVenda);
        Assert.Equal(3m, response.EstoqueMinimo);
        Assert.Equal("Com apoio de braço", response.InfoAdicional);

        var salvo = await repositorio.ObterPorIdAsync(produtoId);
        Assert.Equal("Cadeira de Escritório Ergonômica", salvo!.Descricao);
    }

    [Fact]
    public async Task Nao_deve_alterar_o_codigo_nem_o_estoque_atual_do_produto()
    {
        var (repositorio, produtoId) = await CadastrarProdutoAsync();
        var useCase = new AtualizarProdutoUseCase(repositorio);

        var response = await useCase.ExecutarAsync(RequestValido(produtoId));

        Assert.Equal("P-001", response.Codigo);
        Assert.Equal(10m, response.EstoqueAtual);
    }

    [Fact]
    public async Task Deve_impedir_atualizar_produto_inexistente()
    {
        var repositorio = new FakeProdutoRepositorio();
        var useCase = new AtualizarProdutoUseCase(repositorio);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(RequestValido(Guid.NewGuid())));

        Assert.Equal("Produto não encontrado.", excecao.Message);
    }

    [Fact]
    public async Task Deve_propagar_validacao_de_dominio_ao_atualizar()
    {
        var (repositorio, produtoId) = await CadastrarProdutoAsync();
        var useCase = new AtualizarProdutoUseCase(repositorio);

        var request = RequestValido(produtoId) with { VlrVenda = -1m };

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() => useCase.ExecutarAsync(request));

        Assert.Equal("O valor de venda do produto não pode ser negativo.", excecao.Message);
    }
}
