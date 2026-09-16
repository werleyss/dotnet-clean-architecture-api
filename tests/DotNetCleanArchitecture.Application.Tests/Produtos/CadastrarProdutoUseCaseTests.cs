using DotNetCleanArchitecture.Application.Produtos;
using DotNetCleanArchitecture.Application.Produtos.DTOs;
using DotNetCleanArchitecture.Application.Tests.Fakes;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;

namespace DotNetCleanArchitecture.Application.Tests.Produtos;

public class CadastrarProdutoUseCaseTests
{
    private static CadastrarProdutoRequest RequestValido(
        string codigo = "P-001",
        string descricao = "Cadeira de Escritório",
        string ncm = "94013000",
        decimal vlrCusto = 120.00m,
        decimal vlrVenda = 249.90m,
        bool ativo = true)
        => new(
            codigo,
            descricao,
            TipoProduto.MercadoriaParaRevenda,
            ncm,
            null,
            OrigemMercadoria.Nacional,
            null,
            null,
            null,
            null,
            "UN",
            vlrCusto,
            vlrVenda,
            EstoqueAtual: 10m,
            EstoqueMinimo: 2m,
            PesoLiquido: 1m,
            PesoBruto: 1m,
            Altura: 1m,
            Largura: 1m,
            Profundidade: 1m,
            InfoAdicional: null,
            Ativo: ativo);

    [Fact]
    public async Task Deve_cadastrar_produto_com_dados_validos()
    {
        var repositorio = new FakeProdutoRepositorio();
        var useCase = new CadastrarProdutoUseCase(repositorio);

        var response = await useCase.ExecutarAsync(RequestValido());

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("P-001", response.Codigo);
        Assert.Equal("Cadeira de Escritório", response.Descricao);
        Assert.Equal("94013000", response.NCM);
        Assert.True(response.Ativo);
        Assert.Equal(10m, response.EstoqueAtual);

        var salvo = await repositorio.ObterPorIdAsync(response.Id);
        Assert.NotNull(salvo);
    }

    [Fact]
    public async Task Deve_impedir_cadastro_com_codigo_ja_existente()
    {
        var repositorio = new FakeProdutoRepositorio();
        var useCase = new CadastrarProdutoUseCase(repositorio);
        await useCase.ExecutarAsync(RequestValido());

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(RequestValido(descricao: "Outra descrição")));

        Assert.Equal("Já existe um produto cadastrado com esse código.", excecao.Message);
    }

    [Fact]
    public async Task Deve_propagar_validacao_de_dominio_do_ncm_invalido()
    {
        var repositorio = new FakeProdutoRepositorio();
        var useCase = new CadastrarProdutoUseCase(repositorio);

        var request = RequestValido(ncm: "123");

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() => useCase.ExecutarAsync(request));

        Assert.Equal("O NCM deve conter 8 dígitos.", excecao.Message);
    }

    [Fact]
    public async Task Deve_propagar_validacao_de_dominio_do_valor_negativo()
    {
        var repositorio = new FakeProdutoRepositorio();
        var useCase = new CadastrarProdutoUseCase(repositorio);

        var request = RequestValido(vlrCusto: -1m);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() => useCase.ExecutarAsync(request));

        Assert.Equal("O valor de custo do produto não pode ser negativo.", excecao.Message);
    }
}
