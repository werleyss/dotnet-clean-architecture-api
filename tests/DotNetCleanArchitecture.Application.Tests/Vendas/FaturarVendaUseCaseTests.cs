using DotNetCleanArchitecture.Application.Tests.Fakes;
using DotNetCleanArchitecture.Application.Vendas;
using DotNetCleanArchitecture.Application.Vendas.DTOs;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Application.Tests.Vendas;

public class FaturarVendaUseCaseTests
{
    private static Produto CriarProduto(decimal vlrVenda = 100m, decimal estoqueAtual = 10m)
        => new(
            "P-001",
            "Produto Teste",
            TipoProduto.MercadoriaParaRevenda,
            "94013000",
            null,
            OrigemMercadoria.Nacional,
            null,
            null,
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
            null);

    private static async Task<(FakeVendaRepositorio vendas, FakeProdutoRepositorio produtos,
            FakeMovimentoEstoqueRepositorio movimentos, Guid vendaId, Produto produto)>
        CriarVendaComItemEPagamentoAsync(decimal quantidade = 2m, decimal vlrVenda = 100m, decimal estoqueAtual = 10m)
    {
        var vendas = new FakeVendaRepositorio();
        var produtos = new FakeProdutoRepositorio();
        var movimentos = new FakeMovimentoEstoqueRepositorio();

        var produto = CriarProduto(vlrVenda, estoqueAtual);
        await produtos.AdicionarAsync(produto);

        var venda = new Venda(1, Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, "Venda de mercadoria", "5102", null);
        venda.AdicionarItem(produto.Id, quantidade, vlrVenda);
        venda.AdicionarPagamento(TipoPagamento.Dinheiro, quantidade * vlrVenda);
        await vendas.AdicionarAsync(venda);

        return (vendas, produtos, movimentos, venda.Id, produto);
    }

    [Fact]
    public async Task Deve_faturar_venda_baixar_estoque_e_registrar_movimento()
    {
        var (vendas, produtos, movimentos, vendaId, produto) =
            await CriarVendaComItemEPagamentoAsync(quantidade: 2m, vlrVenda: 100m, estoqueAtual: 10m);

        var useCase = new FaturarVendaUseCase(vendas, produtos, movimentos);

        var response = await useCase.ExecutarAsync(new FaturarVendaRequest(vendaId));

        Assert.Equal("Faturada", response.Status);

        var produtoAtualizado = await produtos.ObterPorIdAsync(produto.Id);
        Assert.Equal(8m, produtoAtualizado!.EstoqueAtual);

        var movimento = Assert.Single(movimentos.Movimentos);
        Assert.Equal(produto.Id, movimento.ProdutoId);
        Assert.Equal(TipoMovimentoEstoque.Saida, movimento.Tipo);
        Assert.Equal(OrigemMovimentoEstoque.Venda, movimento.Origem);
        Assert.Equal(2m, movimento.Quantidade);
        Assert.Equal(10m, movimento.SaldoAnterior);
        Assert.Equal(8m, movimento.SaldoAtual);
        Assert.Equal(vendaId, movimento.DocumentoOrigemId);
    }

    [Fact]
    public async Task Deve_impedir_faturar_venda_inexistente()
    {
        var vendas = new FakeVendaRepositorio();
        var produtos = new FakeProdutoRepositorio();
        var movimentos = new FakeMovimentoEstoqueRepositorio();
        var useCase = new FaturarVendaUseCase(vendas, produtos, movimentos);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new FaturarVendaRequest(Guid.NewGuid())));

        Assert.Equal("Venda não encontrada.", excecao.Message);
    }

    [Fact]
    public async Task Deve_propagar_validacao_de_dominio_ao_faturar_sem_pagamentos()
    {
        var vendas = new FakeVendaRepositorio();
        var produtos = new FakeProdutoRepositorio();
        var movimentos = new FakeMovimentoEstoqueRepositorio();

        var produto = CriarProduto();
        await produtos.AdicionarAsync(produto);

        var venda = new Venda(1, Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, "Venda de mercadoria", "5102", null);
        venda.AdicionarItem(produto.Id, 1m, 100m);
        await vendas.AdicionarAsync(venda);

        var useCase = new FaturarVendaUseCase(vendas, produtos, movimentos);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new FaturarVendaRequest(venda.Id)));

        Assert.Equal(
            "A venda deve ter ao menos um pagamento para ser faturada.",
            excecao.Message);
    }

    [Fact]
    public async Task Deve_permitir_estoque_negativo_apos_faturar_alem_do_saldo()
    {
        var (vendas, produtos, movimentos, vendaId, produto) =
            await CriarVendaComItemEPagamentoAsync(quantidade: 5m, vlrVenda: 100m, estoqueAtual: 2m);

        var useCase = new FaturarVendaUseCase(vendas, produtos, movimentos);

        await useCase.ExecutarAsync(new FaturarVendaRequest(vendaId));

        var produtoAtualizado = await produtos.ObterPorIdAsync(produto.Id);
        Assert.Equal(-3m, produtoAtualizado!.EstoqueAtual);
    }
}
