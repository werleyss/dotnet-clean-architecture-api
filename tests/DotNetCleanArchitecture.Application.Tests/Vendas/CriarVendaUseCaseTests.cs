using DotNetCleanArchitecture.Application.Tests.Fakes;
using DotNetCleanArchitecture.Application.Vendas;
using DotNetCleanArchitecture.Application.Vendas.DTOs;
using DotNetCleanArchitecture.Domain.Core.Excecoes;

namespace DotNetCleanArchitecture.Application.Tests.Vendas;

public class CriarVendaUseCaseTests
{
    private static CriarVendaRequest RequestValido(
        int numero = 1,
        Guid? empresaId = null,
        Guid? clienteId = null,
        string cfop = "5102")
        => new(
            numero,
            empresaId ?? Guid.NewGuid(),
            clienteId ?? Guid.NewGuid(),
            DateTime.Now,
            "Venda de mercadoria",
            cfop,
            null);

    [Fact]
    public async Task Deve_criar_venda_em_orcamento()
    {
        var repositorio = new FakeVendaRepositorio();
        var useCase = new CriarVendaUseCase(repositorio);

        var response = await useCase.ExecutarAsync(RequestValido());

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("Orcamento", response.Status);
        Assert.Equal(0, response.QuantidadeItens);
        Assert.Equal(0, response.QuantidadePagamentos);

        var salva = await repositorio.ObterPorIdAsync(response.Id);
        Assert.NotNull(salva);
    }

    [Fact]
    public async Task Deve_propagar_validacao_de_dominio_do_cfop_invalido()
    {
        var repositorio = new FakeVendaRepositorio();
        var useCase = new CriarVendaUseCase(repositorio);

        var request = RequestValido(cfop: "1102");

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() => useCase.ExecutarAsync(request));

        Assert.Equal(
            "O CFOP de uma venda deve iniciar com 5, 6 ou 7 (operação de saída).",
            excecao.Message);
    }
}
