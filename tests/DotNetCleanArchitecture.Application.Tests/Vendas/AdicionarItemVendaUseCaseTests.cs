using DotNetCleanArchitecture.Application.Tests.Fakes;
using DotNetCleanArchitecture.Application.Vendas;
using DotNetCleanArchitecture.Application.Vendas.DTOs;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Application.Tests.Vendas;

public class AdicionarItemVendaUseCaseTests
{
    private static Produto CriarProduto(
        decimal vlrVenda = 100m,
        decimal estoqueAtual = 10m,
        bool ativo = true,
        CstIcms? cst = null,
        Csosn? csosn = null)
        => new(
            "P-001",
            "Produto Teste",
            TipoProduto.MercadoriaParaRevenda,
            "94013000",
            null,
            OrigemMercadoria.Nacional,
            cst,
            csosn,
            null,
            null,
            "UN",
            50m,
            vlrVenda,
            estoqueAtual,
            2m,
            1m,
            1m,
            1m,
            1m,
            1m,
            null,
            ativo);

    private static async Task<(FakeVendaRepositorio vendas, Guid vendaId)> CriarVendaAsync()
    {
        var repositorio = new FakeVendaRepositorio();
        var venda = new Venda(1, Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, "Venda de mercadoria", "5102", null);
        await repositorio.AdicionarAsync(venda);
        return (repositorio, venda.Id);
    }

    [Fact]
    public async Task Deve_adicionar_item_usando_preco_e_tributacao_do_produto()
    {
        var (vendas, vendaId) = await CriarVendaAsync();
        var produtos = new FakeProdutoRepositorio();
        var produto = CriarProduto(vlrVenda: 150m, cst: CstIcms.TributadaIntegralmente);
        await produtos.AdicionarAsync(produto);

        var useCase = new AdicionarItemVendaUseCase(vendas, produtos);

        var response = await useCase.ExecutarAsync(new AdicionarItemVendaRequest(vendaId, produto.Id, 2m));

        Assert.Equal(1, response.QuantidadeItens);
        Assert.Equal(300m, response.ValorProdutos);

        var venda = await vendas.ObterPorIdAsync(vendaId);
        var item = venda!.Itens.Single();
        Assert.Equal(150m, item.ValorUnitario);
        Assert.Equal(CstIcms.TributadaIntegralmente, item.CST);
    }

    [Fact]
    public async Task Deve_impedir_item_de_venda_inexistente()
    {
        var vendas = new FakeVendaRepositorio();
        var produtos = new FakeProdutoRepositorio();
        var produto = CriarProduto();
        await produtos.AdicionarAsync(produto);

        var useCase = new AdicionarItemVendaUseCase(vendas, produtos);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new AdicionarItemVendaRequest(Guid.NewGuid(), produto.Id, 1m)));

        Assert.Equal("Venda não encontrada.", excecao.Message);
    }

    [Fact]
    public async Task Deve_impedir_item_de_produto_inexistente()
    {
        var (vendas, vendaId) = await CriarVendaAsync();
        var produtos = new FakeProdutoRepositorio();

        var useCase = new AdicionarItemVendaUseCase(vendas, produtos);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new AdicionarItemVendaRequest(vendaId, Guid.NewGuid(), 1m)));

        Assert.Equal("Produto não encontrado.", excecao.Message);
    }

    [Fact]
    public async Task Deve_impedir_item_de_produto_inativo()
    {
        var (vendas, vendaId) = await CriarVendaAsync();
        var produtos = new FakeProdutoRepositorio();
        var produto = CriarProduto(ativo: false);
        await produtos.AdicionarAsync(produto);

        var useCase = new AdicionarItemVendaUseCase(vendas, produtos);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new AdicionarItemVendaRequest(vendaId, produto.Id, 1m)));

        Assert.Equal("Não é possível vender um produto inativo.", excecao.Message);
    }

    [Fact]
    public async Task Deve_propagar_validacao_de_dominio_do_item()
    {
        var (vendas, vendaId) = await CriarVendaAsync();
        var produtos = new FakeProdutoRepositorio();
        var produto = CriarProduto();
        await produtos.AdicionarAsync(produto);

        var useCase = new AdicionarItemVendaUseCase(vendas, produtos);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new AdicionarItemVendaRequest(vendaId, produto.Id, 0m)));

        Assert.Equal("A quantidade do item deve ser maior que zero.", excecao.Message);
    }
}
