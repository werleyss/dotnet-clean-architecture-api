using DotNetCleanArchitecture.Application.Nfes;
using DotNetCleanArchitecture.Application.Nfes.DTOs;
using DotNetCleanArchitecture.Application.Tests.Fakes;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Application.Tests.Nfes;

public class EmitirNfeUseCaseTests
{
    private static async Task<(FakeVendaRepositorio vendas, Guid vendaId)> CriarVendaFaturadaAsync()
    {
        var vendas = new FakeVendaRepositorio();
        var venda = new Venda(1, Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, "Venda de mercadoria", "5102", null);
        venda.AdicionarItem(Guid.NewGuid(), 1m, 100m);
        venda.AdicionarPagamento(TipoPagamento.Dinheiro, 100m);
        venda.Faturar();
        await vendas.AdicionarAsync(venda);
        return (vendas, venda.Id);
    }

    [Fact]
    public async Task Deve_emitir_nfe_para_venda_faturada()
    {
        var (vendas, vendaId) = await CriarVendaFaturadaAsync();
        var nfes = new FakeNfeRepositorio();
        var useCase = new EmitirNfeUseCase(vendas, nfes);

        var response = await useCase.ExecutarAsync(new EmitirNfeRequest(
            vendaId, 501, 1, AmbienteEmissao.Homologacao, ModalidadeFrete.CifRemetente, "Pedido 123"));

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(vendaId, response.VendaId);
        Assert.Equal("Pendente", response.Status);
        Assert.Equal("Normal", response.TipoEmissao);
        Assert.Equal("CifRemetente", response.ModalidadeFrete);
        Assert.Equal("Pedido 123", response.InformacoesComplementares);

        var salva = await nfes.ObterPorIdAsync(response.Id);
        Assert.NotNull(salva);
    }

    [Fact]
    public async Task Deve_impedir_emitir_nfe_para_venda_inexistente()
    {
        var vendas = new FakeVendaRepositorio();
        var nfes = new FakeNfeRepositorio();
        var useCase = new EmitirNfeUseCase(vendas, nfes);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new EmitirNfeRequest(
                Guid.NewGuid(), 501, 1, AmbienteEmissao.Homologacao, ModalidadeFrete.CifRemetente)));

        Assert.Equal("Venda não encontrada.", excecao.Message);
    }

    [Fact]
    public async Task Deve_impedir_emitir_nfe_para_venda_nao_faturada()
    {
        var vendas = new FakeVendaRepositorio();
        var venda = new Venda(1, Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, "Venda de mercadoria", "5102", null);
        await vendas.AdicionarAsync(venda);
        var nfes = new FakeNfeRepositorio();
        var useCase = new EmitirNfeUseCase(vendas, nfes);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new EmitirNfeRequest(
                venda.Id, 501, 1, AmbienteEmissao.Homologacao, ModalidadeFrete.CifRemetente)));

        Assert.Equal("Somente uma venda faturada pode gerar NF-e.", excecao.Message);
    }

    [Fact]
    public async Task Deve_propagar_validacao_de_dominio_do_numero_invalido()
    {
        var (vendas, vendaId) = await CriarVendaFaturadaAsync();
        var nfes = new FakeNfeRepositorio();
        var useCase = new EmitirNfeUseCase(vendas, nfes);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new EmitirNfeRequest(
                vendaId, 0, 1, AmbienteEmissao.Homologacao, ModalidadeFrete.CifRemetente)));

        Assert.Equal("O número da NF-e deve ser informado.", excecao.Message);
    }
}
