using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Domain.Tests.Nfes;

public class NfeTests
{
    private const string ChaveAcessoValida = "35240112345678000190550010000010011234567890";

    private static Nfe CriarNfe(
        Guid? vendaId = null,
        int numero = 501,
        int serie = 1,
        AmbienteEmissao ambiente = AmbienteEmissao.Homologacao,
        ModalidadeFrete modalidadeFrete = ModalidadeFrete.CifRemetente,
        string? informacoesComplementares = null,
        TipoEmissao tipoEmissao = TipoEmissao.Normal)
        => new Nfe(
            vendaId ?? Guid.NewGuid(),
            numero,
            serie,
            ambiente,
            modalidadeFrete,
            informacoesComplementares,
            tipoEmissao);

    private static Nfe CriarNfeAutorizada()
    {
        var nfe = CriarNfe();
        nfe.Autorizar(ChaveAcessoValida, "135240000012345");
        return nfe;
    }

    #region Criação

    [Fact]
    public void Deve_criar_nfe_pendente_com_dados_validos()
    {
        var vendaId = Guid.NewGuid();

        var nfe = new Nfe(vendaId, numero: 501, serie: 1,
            ambiente: AmbienteEmissao.Producao, modalidadeFrete: ModalidadeFrete.FobDestinatario,
            informacoesComplementares: "Pedido 123", tipoEmissao: TipoEmissao.Normal);

        Assert.NotNull(nfe);
        Assert.Equal(vendaId, nfe.VendaId);
        Assert.Equal(501, nfe.Numero);
        Assert.Equal(1, nfe.Serie);
        Assert.Equal(AmbienteEmissao.Producao, nfe.Ambiente);
        Assert.Equal(ModalidadeFrete.FobDestinatario, nfe.ModalidadeFrete);
        Assert.Equal("Pedido 123", nfe.InformacoesComplementares);
        Assert.Equal(TipoEmissao.Normal, nfe.TipoEmissao);
        Assert.Equal(StatusDocumentoFiscal.Pendente, nfe.Status);
        Assert.Null(nfe.ChaveAcesso);
        Assert.Null(nfe.ProtocoloAutorizacao);
        Assert.Null(nfe.DataAutorizacao);
    }

    [Fact]
    public void Deve_usar_emissao_normal_como_padrao()
    {
        var nfe = new Nfe(Guid.NewGuid(), 1, 1, AmbienteEmissao.Homologacao, ModalidadeFrete.SemTransporte);

        Assert.Equal(TipoEmissao.Normal, nfe.TipoEmissao);
    }

    [Fact]
    public void Deve_remover_espacos_das_informacoes_complementares()
    {
        var nfe = CriarNfe(informacoesComplementares: "  Pedido 123  ");

        Assert.Equal("Pedido 123", nfe.InformacoesComplementares);
    }

    [Fact]
    public void Deve_impedir_nfe_sem_venda()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarNfe(vendaId: Guid.Empty));

        Assert.Equal("A venda da NF-e deve ser informada.", excecao.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_impedir_numero_invalido(int numero)
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarNfe(numero: numero));

        Assert.Equal("O número da NF-e deve ser informado.", excecao.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_impedir_serie_invalida(int serie)
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarNfe(serie: serie));

        Assert.Equal("A série da NF-e deve ser informada.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_ambiente_invalido()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarNfe(ambiente: (AmbienteEmissao)99));

        Assert.Equal("O ambiente de emissão informado é inválido.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_tipo_emissao_invalido()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarNfe(tipoEmissao: (TipoEmissao)99));

        Assert.Equal("O tipo de emissão informado é inválido.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_modalidade_de_frete_invalida()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            CriarNfe(modalidadeFrete: (ModalidadeFrete)99));

        Assert.Equal("A modalidade de frete informada é inválida.", excecao.Message);
    }

    #endregion

    #region Autorizar

    [Fact]
    public void Deve_autorizar_nfe_pendente()
    {
        var nfe = CriarNfe();

        nfe.Autorizar(ChaveAcessoValida, "135240000012345");

        Assert.Equal(StatusDocumentoFiscal.Autorizada, nfe.Status);
        Assert.Equal(ChaveAcessoValida, nfe.ChaveAcesso);
        Assert.Equal("135240000012345", nfe.ProtocoloAutorizacao);
        Assert.NotNull(nfe.DataAutorizacao);
    }

    [Fact]
    public void Deve_normalizar_chave_de_acesso()
    {
        var nfe = CriarNfe();
        var chaveComMascara = "3524 0112 3456 7800 0190 5500 1000 0010 0112 3456 7890";

        nfe.Autorizar(chaveComMascara, "135240000012345");

        Assert.Equal(ChaveAcessoValida, nfe.ChaveAcesso);
    }

    [Fact]
    public void Deve_impedir_chave_de_acesso_com_tamanho_invalido()
    {
        var nfe = CriarNfe();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            nfe.Autorizar("12345", "135240000012345"));

        Assert.Equal("A chave de acesso da NF-e deve conter 44 dígitos.", excecao.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_impedir_autorizar_sem_protocolo(string? protocolo)
    {
        var nfe = CriarNfe();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            nfe.Autorizar(ChaveAcessoValida, protocolo!));

        Assert.Equal("O protocolo de autorização deve ser informado.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_autorizar_nfe_que_nao_esta_pendente()
    {
        var nfe = CriarNfeAutorizada();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            nfe.Autorizar(ChaveAcessoValida, "outro-protocolo"));

        Assert.Equal("Somente uma NF-e pendente pode ser autorizada.", excecao.Message);
    }

    #endregion

    #region Rejeitar

    [Fact]
    public void Deve_rejeitar_nfe_pendente()
    {
        var nfe = CriarNfe();

        nfe.Rejeitar("Duplicidade de NF-e");

        Assert.Equal(StatusDocumentoFiscal.Rejeitada, nfe.Status);
        Assert.Equal("Duplicidade de NF-e", nfe.MotivoStatus);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_impedir_rejeitar_sem_motivo(string? motivo)
    {
        var nfe = CriarNfe();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() => nfe.Rejeitar(motivo!));

        Assert.Equal("O motivo da rejeição deve ser informado.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_rejeitar_nfe_ja_autorizada()
    {
        var nfe = CriarNfeAutorizada();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() => nfe.Rejeitar("motivo"));

        Assert.Equal("Somente uma NF-e pendente pode ser rejeitada.", excecao.Message);
    }

    #endregion

    #region Denegar

    [Fact]
    public void Deve_denegar_nfe_pendente()
    {
        var nfe = CriarNfe();

        nfe.Denegar("CNPJ do destinatário irregular");

        Assert.Equal(StatusDocumentoFiscal.Denegada, nfe.Status);
        Assert.Equal("CNPJ do destinatário irregular", nfe.MotivoStatus);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_impedir_denegar_sem_motivo(string? motivo)
    {
        var nfe = CriarNfe();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() => nfe.Denegar(motivo!));

        Assert.Equal("O motivo da denegação deve ser informado.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_denegar_nfe_ja_rejeitada()
    {
        var nfe = CriarNfe();
        nfe.Rejeitar("Erro de schema");

        var excecao = Assert.Throws<ExcecaoDeDominio>(() => nfe.Denegar("motivo"));

        Assert.Equal("Somente uma NF-e pendente pode ser denegada.", excecao.Message);
    }

    #endregion

    #region Cancelar

    [Fact]
    public void Deve_cancelar_nfe_autorizada()
    {
        var nfe = CriarNfeAutorizada();

        nfe.Cancelar("135250000098765", "Cancelamento solicitado pelo cliente no mesmo dia");

        Assert.Equal(StatusDocumentoFiscal.Cancelada, nfe.Status);
        Assert.Equal("135250000098765", nfe.ProtocoloCancelamento);
        Assert.Equal("Cancelamento solicitado pelo cliente no mesmo dia", nfe.JustificativaCancelamento);
        Assert.NotNull(nfe.DataCancelamento);
    }

    [Fact]
    public void Deve_impedir_cancelar_nfe_pendente()
    {
        var nfe = CriarNfe();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            nfe.Cancelar("135250000098765", "Cancelamento solicitado pelo cliente no mesmo dia"));

        Assert.Equal("Somente uma NF-e autorizada pode ser cancelada.", excecao.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_impedir_cancelar_sem_protocolo(string? protocolo)
    {
        var nfe = CriarNfeAutorizada();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            nfe.Cancelar(protocolo!, "Cancelamento solicitado pelo cliente no mesmo dia"));

        Assert.Equal("O protocolo de cancelamento deve ser informado.", excecao.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("Motivo curto")]
    public void Deve_impedir_justificativa_de_cancelamento_curta(string? justificativa)
    {
        var nfe = CriarNfeAutorizada();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            nfe.Cancelar("135250000098765", justificativa!));

        Assert.Equal(
            "A justificativa de cancelamento deve ter ao menos 15 caracteres.",
            excecao.Message);
    }

    [Fact]
    public void Deve_impedir_cancelar_nfe_ja_cancelada()
    {
        var nfe = CriarNfeAutorizada();
        nfe.Cancelar("135250000098765", "Cancelamento solicitado pelo cliente no mesmo dia");

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            nfe.Cancelar("135250000098766", "Segunda tentativa de cancelamento da mesma nota"));

        Assert.Equal("Somente uma NF-e autorizada pode ser cancelada.", excecao.Message);
    }

    #endregion
}
