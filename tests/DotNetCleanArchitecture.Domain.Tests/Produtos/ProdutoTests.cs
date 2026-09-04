using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Domain.Tests.Produtos;

public class ProdutoTests
{
    private static Produto CriarProduto(
        string? codigo = "P-001",
        string? descricao = "Cadeira de Escritório",
        TipoProduto tipo = TipoProduto.MercadoriaParaRevenda,
        string? ncm = "94013000",
        string? cest = null,
        OrigemMercadoria origemMercadoria = OrigemMercadoria.Nacional,
        CstIcms? cst = null,
        Csosn? csosn = null,
        string? ean = null,
        string? eanTrib = null,
        string? un = "UN",
        decimal vlrCusto = 120.00m,
        decimal vlrVenda = 249.90m,
        decimal estoqueAtual = 10m,
        decimal estoqueMinimo = 2m,
        decimal pesoLiquido = 8.5m,
        decimal pesoBruto = 9.2m,
        decimal altura = 1.1m,
        decimal largura = 0.6m,
        decimal profundidade = 0.6m,
        string? infoAdicional = null,
        bool ativo = true)
        => new Produto(
            codigo!,
            descricao!,
            tipo,
            ncm!,
            cest,
            origemMercadoria,
            cst,
            csosn,
            ean,
            eanTrib,
            un!,
            vlrCusto,
            vlrVenda,
            estoqueAtual,
            estoqueMinimo,
            pesoLiquido,
            pesoBruto,
            altura,
            largura,
            profundidade,
            infoAdicional,
            ativo);

    [Fact]
    public void Deve_criar_produto_com_dados_validos()
    {
        // Act
        var produto = new Produto(
            codigo: "P-001",
            descricao: "Cadeira de Escritório",
            tipo: TipoProduto.MercadoriaParaRevenda,
            ncm: "94013000",
            cest: "2803800",
            origemMercadoria: OrigemMercadoria.Nacional,
            cst: CstIcms.TributadaIntegralmente,
            csosn: null,
            ean: "7891234567890",
            eanTrib: "7891234567890",
            un: "UN",
            vlrCusto: 120.00m,
            vlrVenda: 249.90m,
            estoqueAtual: 10m,
            estoqueMinimo: 2m,
            pesoLiquido: 8.5m,
            pesoBruto: 9.2m,
            altura: 1.1m,
            largura: 0.6m,
            profundidade: 0.6m,
            infoAdicional: "Montagem inclusa",
            ativo: true);

        // Assert
        Assert.NotNull(produto);
        Assert.Equal("P-001", produto.Codigo);
        Assert.Equal("Cadeira de Escritório", produto.Descricao);
        Assert.Equal(TipoProduto.MercadoriaParaRevenda, produto.Tipo);
        Assert.True(produto.Ativo);
        Assert.Equal("94013000", produto.NCM);
        Assert.Equal("2803800", produto.CEST);
        Assert.Equal(OrigemMercadoria.Nacional, produto.OrigemMercadoria);
        Assert.Equal(CstIcms.TributadaIntegralmente, produto.CST);
        Assert.Null(produto.CSOSN);
        Assert.Equal("7891234567890", produto.EAN);
        Assert.Equal("7891234567890", produto.EANTrib);
        Assert.Equal("UN", produto.UN);
        Assert.Equal(120.00m, produto.VlrCusto);
        Assert.Equal(249.90m, produto.VlrVenda);
        Assert.Equal(10m, produto.EstoqueAtual);
        Assert.Equal(2m, produto.EstoqueMinimo);
        Assert.Equal(8.5m, produto.PesoLiquido);
        Assert.Equal(9.2m, produto.PesoBruto);
        Assert.Equal(1.1m, produto.Altura);
        Assert.Equal(0.6m, produto.Largura);
        Assert.Equal(0.6m, produto.Profundidade);
        Assert.Equal("Montagem inclusa", produto.InfoAdicional);
    }

    [Fact]
    public void Deve_criar_produto_ativo_por_padrao()
    {
        // Act
        var produto = CriarProduto();

        // Assert
        Assert.True(produto.Ativo);
    }

    [Fact]
    public void Deve_normalizar_ncm_cest_e_gtin()
    {
        // Act
        var produto = CriarProduto(
            ncm: "9401.30.00",
            cest: "28.038.00",
            ean: "789-1234-567890");

        // Assert
        Assert.Equal("94013000", produto.NCM);
        Assert.Equal("2803800", produto.CEST);
        Assert.Equal("7891234567890", produto.EAN);
    }

    [Fact]
    public void Deve_remover_espacos_e_padronizar_unidade()
    {
        // Act
        var produto = CriarProduto(
            codigo: "  P-001  ",
            descricao: "  Cadeira de Escritório  ",
            un: "  cx  ",
            infoAdicional: "  Montagem inclusa  ");

        // Assert
        Assert.Equal("P-001", produto.Codigo);
        Assert.Equal("Cadeira de Escritório", produto.Descricao);
        Assert.Equal("CX", produto.UN);
        Assert.Equal("Montagem inclusa", produto.InfoAdicional);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_impedir_produto_sem_codigo(string? codigo)
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarProduto(codigo: codigo));

        Assert.Equal("O código do produto deve ser informado.", excecao.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_impedir_produto_sem_descricao(string? descricao)
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarProduto(descricao: descricao));

        Assert.Equal("A descrição do produto deve ser informada.", excecao.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_impedir_produto_sem_ncm(string? ncm)
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarProduto(ncm: ncm));

        Assert.Equal("O NCM do produto deve ser informado.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_ncm_com_tamanho_invalido()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarProduto(ncm: "123"));

        Assert.Equal("O NCM deve conter 8 dígitos.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_cest_com_tamanho_invalido()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarProduto(cest: "123"));

        Assert.Equal("O CEST deve conter 7 dígitos.", excecao.Message);
    }

    [Fact]
    public void Deve_ignorar_cest_em_branco()
    {
        var produto = CriarProduto(cest: "   ");

        Assert.Null(produto.CEST);
    }

    [Theory]
    [InlineData("12345")]
    [InlineData("123456789012345")]
    public void Deve_impedir_ean_com_tamanho_invalido(string ean)
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarProduto(ean: ean));

        Assert.Equal("O EAN deve conter 8, 12, 13 ou 14 dígitos.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_ean_tributavel_com_tamanho_invalido()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarProduto(eanTrib: "123"));

        Assert.Equal("O EAN tributável deve conter 8, 12, 13 ou 14 dígitos.", excecao.Message);
    }

    [Fact]
    public void Deve_aceitar_somente_cst()
    {
        var produto = CriarProduto(cst: CstIcms.Isenta, csosn: null);

        Assert.Equal(CstIcms.Isenta, produto.CST);
        Assert.Null(produto.CSOSN);
    }

    [Fact]
    public void Deve_aceitar_somente_csosn()
    {
        var produto = CriarProduto(cst: null, csosn: Csosn.TributadaComPermissaoDeCredito);

        Assert.Null(produto.CST);
        Assert.Equal(Csosn.TributadaComPermissaoDeCredito, produto.CSOSN);
    }

    [Fact]
    public void Deve_impedir_cst_e_csosn_informados_juntos()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            CriarProduto(cst: CstIcms.TributadaIntegralmente, csosn: Csosn.TributadaComPermissaoDeCredito));

        Assert.Equal("Informe o CST ou o CSOSN, não ambos.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_cst_invalido()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarProduto(cst: (CstIcms)99));

        Assert.Equal("O CST informado é inválido.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_csosn_invalido()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarProduto(csosn: (Csosn)999));

        Assert.Equal("O CSOSN informado é inválido.", excecao.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_impedir_unidade_vazia(string? un)
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarProduto(un: un));

        Assert.Equal("A unidade do produto deve ser informada.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_valor_de_custo_negativo()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarProduto(vlrCusto: -1m));

        Assert.Equal("O valor de custo do produto não pode ser negativo.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_valor_de_venda_negativo()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarProduto(vlrVenda: -1m));

        Assert.Equal("O valor de venda do produto não pode ser negativo.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_estoque_minimo_negativo()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarProduto(estoqueMinimo: -1m));

        Assert.Equal("O estoque mínimo do produto não pode ser negativo.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_peso_liquido_negativo()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            CriarProduto(pesoLiquido: -1m, pesoBruto: 0m));

        Assert.Equal("O peso líquido do produto não pode ser negativo.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_peso_bruto_negativo()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            CriarProduto(pesoLiquido: 0m, pesoBruto: -1m));

        Assert.Equal("O peso bruto do produto não pode ser negativo.", excecao.Message);
    }

    [Theory]
    [InlineData("altura")]
    [InlineData("largura")]
    [InlineData("profundidade")]
    public void Deve_impedir_dimensao_negativa(string campo)
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => campo switch
        {
            "altura" => CriarProduto(altura: -1m),
            "largura" => CriarProduto(largura: -1m),
            _ => CriarProduto(profundidade: -1m),
        });

        Assert.Equal($"O {campo} do produto não pode ser negativo.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_peso_bruto_menor_que_liquido()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            CriarProduto(pesoLiquido: 10m, pesoBruto: 9m));

        Assert.Equal("O peso bruto não pode ser menor que o peso líquido.", excecao.Message);
    }

    [Fact]
    public void Deve_permitir_estoque_atual_negativo()
    {
        var produto = CriarProduto(estoqueAtual: -5m);

        Assert.Equal(-5m, produto.EstoqueAtual);
    }

    [Fact]
    public void Ativar_e_inativar_devem_alterar_o_status()
    {
        // Arrange
        var produto = CriarProduto(ativo: false);
        Assert.False(produto.Ativo);

        // Act / Assert
        produto.Ativar();
        Assert.True(produto.Ativo);

        produto.Inativar();
        Assert.False(produto.Ativo);
    }

    [Fact]
    public void Deve_registrar_entrada_de_estoque()
    {
        var produto = CriarProduto(estoqueAtual: 10m);

        produto.RegistrarEntradaEstoque(5m);

        Assert.Equal(15m, produto.EstoqueAtual);
    }

    [Fact]
    public void Deve_registrar_saida_de_estoque()
    {
        var produto = CriarProduto(estoqueAtual: 10m);

        produto.RegistrarSaidaEstoque(3m);

        Assert.Equal(7m, produto.EstoqueAtual);
    }

    [Fact]
    public void Deve_permitir_saida_de_estoque_deixar_saldo_negativo()
    {
        var produto = CriarProduto(estoqueAtual: 2m);

        produto.RegistrarSaidaEstoque(5m);

        Assert.Equal(-3m, produto.EstoqueAtual);
    }

    [Fact]
    public void Deve_arredondar_quantidade_do_movimento_de_estoque()
    {
        var produto = CriarProduto(estoqueAtual: 0m);

        produto.RegistrarEntradaEstoque(2.123456789m);

        Assert.Equal(2.1235m, produto.EstoqueAtual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_impedir_entrada_de_estoque_com_quantidade_invalida(decimal quantidade)
    {
        var produto = CriarProduto();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() => produto.RegistrarEntradaEstoque(quantidade));

        Assert.Equal("A quantidade do movimento de estoque deve ser maior que zero.", excecao.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_impedir_saida_de_estoque_com_quantidade_invalida(decimal quantidade)
    {
        var produto = CriarProduto();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() => produto.RegistrarSaidaEstoque(quantidade));

        Assert.Equal("A quantidade do movimento de estoque deve ser maior que zero.", excecao.Message);
    }
}
