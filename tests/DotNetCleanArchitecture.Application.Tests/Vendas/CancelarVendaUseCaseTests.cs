using DotNetCleanArchitecture.Application.Tests.Fakes;
using DotNetCleanArchitecture.Application.Vendas;
using DotNetCleanArchitecture.Application.Vendas.DTOs;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Application.Tests.Vendas;

public class CancelarVendaUseCaseTests
{
    [Fact]
    public async Task Deve_cancelar_venda_em_orcamento()
    {
        var vendas = new FakeVendaRepositorio();
        var venda = new Venda(1, Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, "Venda de mercadoria", "5102", null);
        await vendas.AdicionarAsync(venda);

        var useCase = new CancelarVendaUseCase(vendas);

        var response = await useCase.ExecutarAsync(new CancelarVendaRequest(venda.Id));

        Assert.Equal("Cancelada", response.Status);
    }

    [Fact]
    public async Task Deve_impedir_cancelar_venda_inexistente()
    {
        var vendas = new FakeVendaRepositorio();
        var useCase = new CancelarVendaUseCase(vendas);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new CancelarVendaRequest(Guid.NewGuid())));

        Assert.Equal("Venda não encontrada.", excecao.Message);
    }

    [Fact]
    public async Task Deve_impedir_cancelar_venda_ja_cancelada()
    {
        var vendas = new FakeVendaRepositorio();
        var venda = new Venda(1, Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, "Venda de mercadoria", "5102", null);
        venda.Cancelar();
        await vendas.AdicionarAsync(venda);

        var useCase = new CancelarVendaUseCase(vendas);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new CancelarVendaRequest(venda.Id)));

        Assert.Equal("A venda já está cancelada.", excecao.Message);
    }
}
