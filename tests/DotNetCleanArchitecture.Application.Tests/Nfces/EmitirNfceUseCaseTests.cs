using DotNetCleanArchitecture.Application.Nfces;
using DotNetCleanArchitecture.Application.Nfces.DTOs;
using DotNetCleanArchitecture.Application.Tests.Fakes;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Application.Tests.Nfces;

public class EmitirNfceUseCaseTests
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
    public async Task Deve_emitir_nfce_para_venda_faturada()
    {
        var (vendas, vendaId) = await CriarVendaFaturadaAsync();
        var nfces = new FakeNfceRepositorio();
        var useCase = new EmitirNfceUseCase(vendas, nfces);

        var response = await useCase.ExecutarAsync(
            new EmitirNfceRequest(vendaId, 1001, 1, AmbienteEmissao.Homologacao));

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal(vendaId, response.VendaId);
        Assert.Equal("Pendente", response.Status);
        Assert.Equal("Normal", response.TipoEmissao);

        var salva = await nfces.ObterPorIdAsync(response.Id);
        Assert.NotNull(salva);
    }

    [Fact]
    public async Task Deve_impedir_emitir_nfce_para_venda_inexistente()
    {
        var vendas = new FakeVendaRepositorio();
        var nfces = new FakeNfceRepositorio();
        var useCase = new EmitirNfceUseCase(vendas, nfces);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new EmitirNfceRequest(Guid.NewGuid(), 1001, 1, AmbienteEmissao.Homologacao)));

        Assert.Equal("Venda não encontrada.", excecao.Message);
    }

    [Fact]
    public async Task Deve_impedir_emitir_nfce_para_venda_nao_faturada()
    {
        var vendas = new FakeVendaRepositorio();
        var venda = new Venda(1, Guid.NewGuid(), Guid.NewGuid(), DateTime.Now, "Venda de mercadoria", "5102", null);
        await vendas.AdicionarAsync(venda);
        var nfces = new FakeNfceRepositorio();
        var useCase = new EmitirNfceUseCase(vendas, nfces);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new EmitirNfceRequest(venda.Id, 1001, 1, AmbienteEmissao.Homologacao)));

        Assert.Equal("Somente uma venda faturada pode gerar NFC-e.", excecao.Message);
    }

    [Fact]
    public async Task Deve_propagar_validacao_de_dominio_do_numero_invalido()
    {
        var (vendas, vendaId) = await CriarVendaFaturadaAsync();
        var nfces = new FakeNfceRepositorio();
        var useCase = new EmitirNfceUseCase(vendas, nfces);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new EmitirNfceRequest(vendaId, 0, 1, AmbienteEmissao.Homologacao)));

        Assert.Equal("O número da NFC-e deve ser informado.", excecao.Message);
    }
}
