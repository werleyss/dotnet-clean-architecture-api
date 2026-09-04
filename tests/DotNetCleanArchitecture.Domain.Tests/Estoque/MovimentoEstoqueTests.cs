using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Domain.Tests.Estoque;

public class MovimentoEstoqueTests
{
    private static MovimentoEstoque CriarMovimento(
        Guid? produtoId = null,
        TipoMovimentoEstoque tipo = TipoMovimentoEstoque.Entrada,
        OrigemMovimentoEstoque origem = OrigemMovimentoEstoque.Compra,
        decimal quantidade = 5m,
        decimal saldoAnterior = 10m,
        Guid? documentoOrigemId = null,
        string? observacao = null)
        => new MovimentoEstoque(
            produtoId ?? Guid.NewGuid(),
            tipo,
            origem,
            quantidade,
            saldoAnterior,
            documentoOrigemId,
            observacao);

    [Fact]
    public void Deve_criar_movimento_de_entrada_e_somar_ao_saldo()
    {
        var produtoId = Guid.NewGuid();
        var documentoOrigemId = Guid.NewGuid();

        var movimento = CriarMovimento(
            produtoId: produtoId,
            tipo: TipoMovimentoEstoque.Entrada,
            origem: OrigemMovimentoEstoque.Compra,
            quantidade: 5m,
            saldoAnterior: 10m,
            documentoOrigemId: documentoOrigemId,
            observacao: "  Compra do fornecedor X  ");

        Assert.NotNull(movimento);
        Assert.Equal(produtoId, movimento.ProdutoId);
        Assert.Equal(TipoMovimentoEstoque.Entrada, movimento.Tipo);
        Assert.Equal(OrigemMovimentoEstoque.Compra, movimento.Origem);
        Assert.Equal(5m, movimento.Quantidade);
        Assert.Equal(10m, movimento.SaldoAnterior);
        Assert.Equal(15m, movimento.SaldoAtual);
        Assert.Equal(documentoOrigemId, movimento.DocumentoOrigemId);
        Assert.Equal("Compra do fornecedor X", movimento.Observacao);
    }

    [Fact]
    public void Deve_criar_movimento_de_saida_e_subtrair_do_saldo()
    {
        var movimento = CriarMovimento(
            tipo: TipoMovimentoEstoque.Saida,
            origem: OrigemMovimentoEstoque.Venda,
            quantidade: 4m,
            saldoAnterior: 10m);

        Assert.Equal(TipoMovimentoEstoque.Saida, movimento.Tipo);
        Assert.Equal(10m, movimento.SaldoAnterior);
        Assert.Equal(6m, movimento.SaldoAtual);
    }

    [Fact]
    public void Deve_permitir_saida_deixar_saldo_atual_negativo()
    {
        var movimento = CriarMovimento(
            tipo: TipoMovimentoEstoque.Saida,
            quantidade: 8m,
            saldoAnterior: 5m);

        Assert.Equal(-3m, movimento.SaldoAtual);
    }

    [Fact]
    public void Deve_arredondar_quantidade_do_movimento()
    {
        var movimento = CriarMovimento(quantidade: 2.123456789m, saldoAnterior: 0m);

        Assert.Equal(2.1235m, movimento.Quantidade);
    }

    [Fact]
    public void Documento_origem_e_observacao_devem_ser_opcionais()
    {
        var movimento = CriarMovimento(documentoOrigemId: null, observacao: null);

        Assert.Null(movimento.DocumentoOrigemId);
        Assert.Null(movimento.Observacao);
    }

    [Fact]
    public void Deve_impedir_movimento_sem_produto()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarMovimento(produtoId: Guid.Empty));

        Assert.Equal(
            "O produto do movimento de estoque deve ser informado.",
            excecao.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_impedir_quantidade_invalida(decimal quantidade)
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarMovimento(quantidade: quantidade));

        Assert.Equal(
            "A quantidade do movimento de estoque deve ser maior que zero.",
            excecao.Message);
    }

    [Fact]
    public void Deve_impedir_tipo_invalido()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            CriarMovimento(tipo: (TipoMovimentoEstoque)99));

        Assert.Equal(
            "O tipo do movimento de estoque informado é inválido.",
            excecao.Message);
    }

    [Fact]
    public void Deve_impedir_origem_invalida()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            CriarMovimento(origem: (OrigemMovimentoEstoque)99));

        Assert.Equal(
            "A origem do movimento de estoque informada é inválida.",
            excecao.Message);
    }
}
