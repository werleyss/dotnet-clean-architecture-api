using DotNetCleanArchitecture.Application.Tests.Fakes;
using DotNetCleanArchitecture.Application.Vendas;
using DotNetCleanArchitecture.Application.Vendas.DTOs;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Application.Tests.Vendas;

public class AdicionarPagamentoVendaUseCaseTests
{
    private static async Task<(FakeVendaRepositorio vendas, Guid vendaId)> CriarVendaAsync()
    {
        var repositorio = new FakeVendaRepositorio();
        var venda = new Venda(1, Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, "Venda de mercadoria", "5102", null);
        await repositorio.AdicionarAsync(venda);
        return (repositorio, venda.Id);
    }

    [Fact]
    public async Task Deve_adicionar_pagamento()
    {
        var (vendas, vendaId) = await CriarVendaAsync();
        var useCase = new AdicionarPagamentoVendaUseCase(vendas);

        var response = await useCase.ExecutarAsync(
            new AdicionarPagamentoVendaRequest(vendaId, TipoPagamento.Pix, 50m));

        Assert.Equal(1, response.QuantidadePagamentos);

        var venda = await vendas.ObterPorIdAsync(vendaId);
        var pagamento = venda!.Pagamentos.Single();
        Assert.Equal(TipoPagamento.Pix, pagamento.FormaPagamento);
        Assert.Equal(50m, pagamento.Valor);
    }

    [Fact]
    public async Task Deve_impedir_pagamento_de_venda_inexistente()
    {
        var vendas = new FakeVendaRepositorio();
        var useCase = new AdicionarPagamentoVendaUseCase(vendas);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new AdicionarPagamentoVendaRequest(Guid.NewGuid(), TipoPagamento.Dinheiro, 10m)));

        Assert.Equal("Venda não encontrada.", excecao.Message);
    }

    [Fact]
    public async Task Deve_propagar_validacao_de_dominio_do_pagamento()
    {
        var (vendas, vendaId) = await CriarVendaAsync();
        var useCase = new AdicionarPagamentoVendaUseCase(vendas);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new AdicionarPagamentoVendaRequest(vendaId, TipoPagamento.Dinheiro, 0m)));

        Assert.Equal("O valor do pagamento deve ser maior que zero.", excecao.Message);
    }
}
