using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Domain.Tests.Vendas;

public class VendaTests
{
    private static Venda CriarVenda(
        int numero = 1,
        Guid? empresaId = null,
        Guid? clienteId = null,
        DateTime? dataEmissao = null,
        string? naturezaOperacao = "Venda de mercadoria",
        string? cfop = "5102",
        string? observacoes = null)
        => new Venda(
            numero,
            empresaId ?? Guid.NewGuid(),
            clienteId ?? Guid.NewGuid(),
            dataEmissao ?? DateTime.Now,
            naturezaOperacao!,
            cfop!,
            observacoes);

    #region Criação

    [Fact]
    public void Deve_criar_venda_com_dados_validos()
    {
        // Arrange
        var empresaId = Guid.NewGuid();
        var clienteId = Guid.NewGuid();
        var dataEmissao = DateTime.Now;

        // Act
        var venda = new Venda(
            numero: 1,
            empresaId: empresaId,
            clienteId: clienteId,
            dataEmissao: dataEmissao,
            naturezaOperacao: "Venda de mercadoria",
            cfop: "5102",
            observacoes: "Entrega agendada");

        // Assert
        Assert.NotNull(venda);
        Assert.Equal(1, venda.Numero);
        Assert.Equal(empresaId, venda.EmpresaId);
        Assert.Equal(clienteId, venda.ClienteId);
        Assert.Equal(dataEmissao, venda.DataEmissao);
        Assert.Equal("Venda de mercadoria", venda.NaturezaOperacao);
        Assert.Equal("5102", venda.CFOP);
        Assert.Equal("Entrega agendada", venda.Observacoes);
        Assert.Equal(StatusVenda.Orcamento, venda.Status);
        Assert.Empty(venda.Itens);
        Assert.Empty(venda.Pagamentos);
        Assert.Equal(0m, venda.ValorProdutos);
        Assert.Equal(0m, venda.ValorDesconto);
        Assert.Equal(0m, venda.ValorTotal);
    }

    [Fact]
    public void Deve_normalizar_cfop()
    {
        var venda = CriarVenda(cfop: "5.102");

        Assert.Equal("5102", venda.CFOP);
    }

    [Fact]
    public void Deve_remover_espacos_da_natureza_operacao_e_observacoes()
    {
        var venda = CriarVenda(
            naturezaOperacao: "  Venda de mercadoria  ",
            observacoes: "  Entrega agendada  ");

        Assert.Equal("Venda de mercadoria", venda.NaturezaOperacao);
        Assert.Equal("Entrega agendada", venda.Observacoes);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_impedir_venda_sem_numero(int numero)
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarVenda(numero: numero));

        Assert.Equal("O número da venda deve ser informado.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_venda_sem_empresa()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarVenda(empresaId: Guid.Empty));

        Assert.Equal("A empresa da venda deve ser informada.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_venda_sem_cliente()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarVenda(clienteId: Guid.Empty));

        Assert.Equal("O cliente da venda deve ser informado.", excecao.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_impedir_venda_sem_natureza_operacao(string? naturezaOperacao)
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarVenda(naturezaOperacao: naturezaOperacao));

        Assert.Equal("A natureza da operação deve ser informada.", excecao.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_impedir_venda_sem_cfop(string? cfop)
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarVenda(cfop: cfop));

        Assert.Equal("O CFOP da venda deve ser informado.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_cfop_com_tamanho_invalido()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarVenda(cfop: "123"));

        Assert.Equal("O CFOP deve conter 4 dígitos.", excecao.Message);
    }

    [Theory]
    [InlineData("1102")]
    [InlineData("2102")]
    [InlineData("3102")]
    public void Deve_impedir_cfop_de_entrada(string cfop)
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarVenda(cfop: cfop));

        Assert.Equal(
            "O CFOP de uma venda deve iniciar com 5, 6 ou 7 (operação de saída).",
            excecao.Message);
    }

    #endregion

    #region Itens

    [Fact]
    public void Deve_adicionar_item_e_recalcular_totais()
    {
        var venda = CriarVenda();

        venda.AdicionarItem(Guid.NewGuid(), quantidade: 2, valorUnitario: 50m);

        var item = Assert.Single(venda.Itens);
        Assert.Equal(2m, item.Quantidade);
        Assert.Equal(50m, item.ValorUnitario);
        Assert.Equal(100m, item.ValorTotal);
        Assert.Equal(100m, venda.ValorProdutos);
        Assert.Equal(0m, venda.ValorDesconto);
        Assert.Equal(100m, venda.ValorTotal);
    }

    [Fact]
    public void Deve_somar_multiplos_itens_com_desconto_nos_totais()
    {
        var venda = CriarVenda();

        venda.AdicionarItem(Guid.NewGuid(), quantidade: 2, valorUnitario: 50m);
        venda.AdicionarItem(Guid.NewGuid(), quantidade: 1, valorUnitario: 30m, valorDesconto: 5m);

        Assert.Equal(130m, venda.ValorProdutos);
        Assert.Equal(5m, venda.ValorDesconto);
        Assert.Equal(125m, venda.ValorTotal);
    }

    [Fact]
    public void Deve_arredondar_quantidade_do_item()
    {
        var venda = CriarVenda();

        venda.AdicionarItem(Guid.NewGuid(), quantidade: 2.123456789m, valorUnitario: 10m);

        Assert.Equal(2.1235m, venda.Itens.Single().Quantidade);
    }

    [Fact]
    public void Deve_impedir_item_com_produto_vazio()
    {
        var venda = CriarVenda();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            venda.AdicionarItem(Guid.Empty, quantidade: 1, valorUnitario: 10m));

        Assert.Equal("O produto do item deve ser informado.", excecao.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_impedir_item_com_quantidade_invalida(decimal quantidade)
    {
        var venda = CriarVenda();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            venda.AdicionarItem(Guid.NewGuid(), quantidade, valorUnitario: 10m));

        Assert.Equal("A quantidade do item deve ser maior que zero.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_item_com_valor_unitario_negativo()
    {
        var venda = CriarVenda();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            venda.AdicionarItem(Guid.NewGuid(), quantidade: 1, valorUnitario: -1m));

        Assert.Equal("O valor unitário do item não pode ser negativo.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_item_com_desconto_maior_que_valor_bruto()
    {
        var venda = CriarVenda();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            venda.AdicionarItem(Guid.NewGuid(), quantidade: 1, valorUnitario: 10m, valorDesconto: 11m));

        Assert.Equal(
            "O valor de desconto do item não pode ser maior que o valor bruto.",
            excecao.Message);
    }

    [Fact]
    public void Deve_impedir_item_com_cst_e_csosn_juntos()
    {
        var venda = CriarVenda();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            venda.AdicionarItem(
                Guid.NewGuid(),
                quantidade: 1,
                valorUnitario: 10m,
                cst: CstIcms.Isenta,
                csosn: Csosn.TributadaComPermissaoDeCredito));

        Assert.Equal("Informe o CST ou o CSOSN do item, não ambos.", excecao.Message);
    }

    [Fact]
    public void Deve_remover_item_e_recalcular_totais()
    {
        var venda = CriarVenda();
        venda.AdicionarItem(Guid.NewGuid(), quantidade: 2, valorUnitario: 50m);
        var itemId = venda.Itens.Single().Id;

        venda.RemoverItem(itemId);

        Assert.Empty(venda.Itens);
        Assert.Equal(0m, venda.ValorProdutos);
        Assert.Equal(0m, venda.ValorTotal);
    }

    [Fact]
    public void Deve_impedir_remover_item_inexistente()
    {
        var venda = CriarVenda();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() => venda.RemoverItem(Guid.NewGuid()));

        Assert.Equal("Item não encontrado na venda.", excecao.Message);
    }

    #endregion

    #region Frete e despesas

    [Fact]
    public void Deve_definir_frete_e_somar_ao_total()
    {
        var venda = CriarVenda();
        venda.AdicionarItem(Guid.NewGuid(), quantidade: 1, valorUnitario: 100m);

        venda.DefinirFrete(10m);

        Assert.Equal(10m, venda.ValorFrete);
        Assert.Equal(110m, venda.ValorTotal);
    }

    [Fact]
    public void Deve_impedir_frete_negativo()
    {
        var venda = CriarVenda();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() => venda.DefinirFrete(-1m));

        Assert.Equal("O valor do frete não pode ser negativo.", excecao.Message);
    }

    [Fact]
    public void Deve_definir_outras_despesas_e_somar_ao_total()
    {
        var venda = CriarVenda();
        venda.AdicionarItem(Guid.NewGuid(), quantidade: 1, valorUnitario: 100m);

        venda.DefinirOutrasDespesas(5m);

        Assert.Equal(5m, venda.ValorOutrasDespesas);
        Assert.Equal(105m, venda.ValorTotal);
    }

    [Fact]
    public void Deve_impedir_outras_despesas_negativas()
    {
        var venda = CriarVenda();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() => venda.DefinirOutrasDespesas(-1m));

        Assert.Equal("O valor de outras despesas não pode ser negativo.", excecao.Message);
    }

    #endregion

    #region Pagamentos

    [Fact]
    public void Deve_adicionar_pagamento()
    {
        var venda = CriarVenda();

        venda.AdicionarPagamento(TipoPagamento.Pix, 50m);

        var pagamento = Assert.Single(venda.Pagamentos);
        Assert.Equal(TipoPagamento.Pix, pagamento.FormaPagamento);
        Assert.Equal(50m, pagamento.Valor);
        Assert.Equal(1, pagamento.QtdParcelas);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_impedir_pagamento_com_valor_invalido(decimal valor)
    {
        var venda = CriarVenda();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            venda.AdicionarPagamento(TipoPagamento.Dinheiro, valor));

        Assert.Equal("O valor do pagamento deve ser maior que zero.", excecao.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_impedir_pagamento_com_parcelas_invalidas(int qtdParcelas)
    {
        var venda = CriarVenda();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            venda.AdicionarPagamento(TipoPagamento.CartaoCredito, 50m, qtdParcelas));

        Assert.Equal("A quantidade de parcelas deve ser maior que zero.", excecao.Message);
    }

    [Fact]
    public void Deve_remover_pagamento()
    {
        var venda = CriarVenda();
        venda.AdicionarPagamento(TipoPagamento.Pix, 50m);
        var pagamentoId = venda.Pagamentos.Single().Id;

        venda.RemoverPagamento(pagamentoId);

        Assert.Empty(venda.Pagamentos);
    }

    [Fact]
    public void Deve_impedir_remover_pagamento_inexistente()
    {
        var venda = CriarVenda();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() => venda.RemoverPagamento(Guid.NewGuid()));

        Assert.Equal("Pagamento não encontrado na venda.", excecao.Message);
    }

    #endregion

    #region Faturar

    [Fact]
    public void Deve_faturar_venda_com_itens_e_pagamentos_batendo()
    {
        var venda = CriarVenda();
        venda.AdicionarItem(Guid.NewGuid(), quantidade: 1, valorUnitario: 100m);
        venda.AdicionarPagamento(TipoPagamento.Dinheiro, 60m);
        venda.AdicionarPagamento(TipoPagamento.CartaoCredito, 40m, qtdParcelas: 3);

        venda.Faturar();

        Assert.Equal(StatusVenda.Faturada, venda.Status);
    }

    [Fact]
    public void Deve_impedir_faturar_sem_itens()
    {
        var venda = CriarVenda();
        venda.AdicionarPagamento(TipoPagamento.Dinheiro, 10m);

        var excecao = Assert.Throws<ExcecaoDeDominio>(() => venda.Faturar());

        Assert.Equal("A venda deve ter ao menos um item para ser faturada.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_faturar_sem_pagamentos()
    {
        var venda = CriarVenda();
        venda.AdicionarItem(Guid.NewGuid(), quantidade: 1, valorUnitario: 100m);

        var excecao = Assert.Throws<ExcecaoDeDominio>(() => venda.Faturar());

        Assert.Equal("A venda deve ter ao menos um pagamento para ser faturada.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_faturar_quando_soma_pagamentos_diferente_do_total()
    {
        var venda = CriarVenda();
        venda.AdicionarItem(Guid.NewGuid(), quantidade: 1, valorUnitario: 100m);
        venda.AdicionarPagamento(TipoPagamento.Dinheiro, 50m);

        var excecao = Assert.Throws<ExcecaoDeDominio>(() => venda.Faturar());

        Assert.Equal(
            "A soma dos pagamentos deve ser igual ao valor total da venda.",
            excecao.Message);
    }

    [Fact]
    public void Deve_impedir_faturar_venda_ja_faturada()
    {
        var venda = CriarVenda();
        venda.AdicionarItem(Guid.NewGuid(), quantidade: 1, valorUnitario: 100m);
        venda.AdicionarPagamento(TipoPagamento.Dinheiro, 100m);
        venda.Faturar();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() => venda.Faturar());

        Assert.Equal("Somente vendas em orçamento podem ser faturadas.", excecao.Message);
    }

    #endregion

    #region Edição bloqueada fora de orçamento

    [Fact]
    public void Deve_impedir_adicionar_item_em_venda_faturada()
    {
        var venda = CriarVenda();
        venda.AdicionarItem(Guid.NewGuid(), quantidade: 1, valorUnitario: 100m);
        venda.AdicionarPagamento(TipoPagamento.Dinheiro, 100m);
        venda.Faturar();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            venda.AdicionarItem(Guid.NewGuid(), quantidade: 1, valorUnitario: 10m));

        Assert.Equal(
            "Não é possível alterar uma venda que não está em orçamento.",
            excecao.Message);
    }

    [Fact]
    public void Deve_impedir_adicionar_pagamento_em_venda_cancelada()
    {
        var venda = CriarVenda();
        venda.Cancelar();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            venda.AdicionarPagamento(TipoPagamento.Dinheiro, 10m));

        Assert.Equal(
            "Não é possível alterar uma venda que não está em orçamento.",
            excecao.Message);
    }

    #endregion

    #region Cancelar

    [Fact]
    public void Deve_cancelar_venda_em_orcamento()
    {
        var venda = CriarVenda();

        venda.Cancelar();

        Assert.Equal(StatusVenda.Cancelada, venda.Status);
    }

    [Fact]
    public void Deve_cancelar_venda_faturada()
    {
        var venda = CriarVenda();
        venda.AdicionarItem(Guid.NewGuid(), quantidade: 1, valorUnitario: 100m);
        venda.AdicionarPagamento(TipoPagamento.Dinheiro, 100m);
        venda.Faturar();

        venda.Cancelar();

        Assert.Equal(StatusVenda.Cancelada, venda.Status);
    }

    [Fact]
    public void Deve_impedir_cancelar_venda_ja_cancelada()
    {
        var venda = CriarVenda();
        venda.Cancelar();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() => venda.Cancelar());

        Assert.Equal("A venda já está cancelada.", excecao.Message);
    }

    #endregion
}
