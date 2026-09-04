using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Domain.Tests.Nfces;

public class NfceTests
{
    private const string ChaveAcessoValida = "35240112345678000190650010000010011234567890";

    private static Nfce CriarNfce(
        Guid? vendaId = null,
        int numero = 1001,
        int serie = 1,
        AmbienteEmissao ambiente = AmbienteEmissao.Homologacao,
        TipoEmissao tipoEmissao = TipoEmissao.Normal)
        => new Nfce(vendaId ?? Guid.NewGuid(), numero, serie, ambiente, tipoEmissao);

    private static Nfce CriarNfceAutorizada()
    {
        var nfce = CriarNfce();
        nfce.Autorizar(ChaveAcessoValida, "135240000012345", "https://consulta.fazenda/qr?p=chave");
        return nfce;
    }

    #region Criação

    [Fact]
    public void Deve_criar_nfce_pendente_com_dados_validos()
    {
        var vendaId = Guid.NewGuid();

        var nfce = new Nfce(vendaId, numero: 1001, serie: 1,
            ambiente: AmbienteEmissao.Producao, tipoEmissao: TipoEmissao.Normal);

        Assert.NotNull(nfce);
        Assert.Equal(vendaId, nfce.VendaId);
        Assert.Equal(1001, nfce.Numero);
        Assert.Equal(1, nfce.Serie);
        Assert.Equal(AmbienteEmissao.Producao, nfce.Ambiente);
        Assert.Equal(TipoEmissao.Normal, nfce.TipoEmissao);
        Assert.Equal(StatusDocumentoFiscal.Pendente, nfce.Status);
        Assert.Null(nfce.ChaveAcesso);
        Assert.Null(nfce.ProtocoloAutorizacao);
        Assert.Null(nfce.DataAutorizacao);
    }

    [Fact]
    public void Deve_usar_emissao_normal_como_padrao()
    {
        var nfce = new Nfce(Guid.NewGuid(), 1, 1, AmbienteEmissao.Homologacao);

        Assert.Equal(TipoEmissao.Normal, nfce.TipoEmissao);
    }

    [Fact]
    public void Deve_impedir_nfce_sem_venda()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarNfce(vendaId: Guid.Empty));

        Assert.Equal("A venda da NFC-e deve ser informada.", excecao.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_impedir_numero_invalido(int numero)
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarNfce(numero: numero));

        Assert.Equal("O número da NFC-e deve ser informado.", excecao.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Deve_impedir_serie_invalida(int serie)
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarNfce(serie: serie));

        Assert.Equal("A série da NFC-e deve ser informada.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_ambiente_invalido()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarNfce(ambiente: (AmbienteEmissao)99));

        Assert.Equal("O ambiente de emissão informado é inválido.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_tipo_emissao_invalido()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => CriarNfce(tipoEmissao: (TipoEmissao)99));

        Assert.Equal("O tipo de emissão informado é inválido.", excecao.Message);
    }

    #endregion

    #region Autorizar

    [Fact]
    public void Deve_autorizar_nfce_pendente()
    {
        var nfce = CriarNfce();

        nfce.Autorizar(ChaveAcessoValida, "135240000012345", "https://consulta.fazenda/qr?p=chave");

        Assert.Equal(StatusDocumentoFiscal.Autorizada, nfce.Status);
        Assert.Equal(ChaveAcessoValida, nfce.ChaveAcesso);
        Assert.Equal("135240000012345", nfce.ProtocoloAutorizacao);
        Assert.Equal("https://consulta.fazenda/qr?p=chave", nfce.QrCode);
        Assert.NotNull(nfce.DataAutorizacao);
    }

    [Fact]
    public void Deve_normalizar_chave_de_acesso()
    {
        var nfce = CriarNfce();
        var chaveComMascara = "3524 0112 3456 7800 0190 6500 1000 0010 0112 3456 7890";

        nfce.Autorizar(chaveComMascara, "135240000012345", "qrcode");

        Assert.Equal(ChaveAcessoValida, nfce.ChaveAcesso);
    }

    [Fact]
    public void Deve_impedir_chave_de_acesso_com_tamanho_invalido()
    {
        var nfce = CriarNfce();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            nfce.Autorizar("12345", "135240000012345", "qrcode"));

        Assert.Equal("A chave de acesso da NFC-e deve conter 44 dígitos.", excecao.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_impedir_autorizar_sem_protocolo(string? protocolo)
    {
        var nfce = CriarNfce();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            nfce.Autorizar(ChaveAcessoValida, protocolo!, "qrcode"));

        Assert.Equal("O protocolo de autorização deve ser informado.", excecao.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_impedir_autorizar_sem_qrcode(string? qrCode)
    {
        var nfce = CriarNfce();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            nfce.Autorizar(ChaveAcessoValida, "135240000012345", qrCode!));

        Assert.Equal("O QR Code deve ser informado.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_autorizar_nfce_que_nao_esta_pendente()
    {
        var nfce = CriarNfceAutorizada();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            nfce.Autorizar(ChaveAcessoValida, "outro-protocolo", "qrcode"));

        Assert.Equal("Somente uma NFC-e pendente pode ser autorizada.", excecao.Message);
    }

    #endregion

    #region Rejeitar

    [Fact]
    public void Deve_rejeitar_nfce_pendente()
    {
        var nfce = CriarNfce();

        nfce.Rejeitar("Duplicidade de NF-e");

        Assert.Equal(StatusDocumentoFiscal.Rejeitada, nfce.Status);
        Assert.Equal("Duplicidade de NF-e", nfce.MotivoStatus);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_impedir_rejeitar_sem_motivo(string? motivo)
    {
        var nfce = CriarNfce();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() => nfce.Rejeitar(motivo!));

        Assert.Equal("O motivo da rejeição deve ser informado.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_rejeitar_nfce_ja_autorizada()
    {
        var nfce = CriarNfceAutorizada();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() => nfce.Rejeitar("motivo"));

        Assert.Equal("Somente uma NFC-e pendente pode ser rejeitada.", excecao.Message);
    }

    #endregion

    #region Denegar

    [Fact]
    public void Deve_denegar_nfce_pendente()
    {
        var nfce = CriarNfce();

        nfce.Denegar("CNPJ do destinatário irregular");

        Assert.Equal(StatusDocumentoFiscal.Denegada, nfce.Status);
        Assert.Equal("CNPJ do destinatário irregular", nfce.MotivoStatus);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_impedir_denegar_sem_motivo(string? motivo)
    {
        var nfce = CriarNfce();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() => nfce.Denegar(motivo!));

        Assert.Equal("O motivo da denegação deve ser informado.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_denegar_nfce_ja_rejeitada()
    {
        var nfce = CriarNfce();
        nfce.Rejeitar("Erro de schema");

        var excecao = Assert.Throws<ExcecaoDeDominio>(() => nfce.Denegar("motivo"));

        Assert.Equal("Somente uma NFC-e pendente pode ser denegada.", excecao.Message);
    }

    #endregion

    #region Cancelar

    [Fact]
    public void Deve_cancelar_nfce_autorizada()
    {
        var nfce = CriarNfceAutorizada();

        nfce.Cancelar("135250000098765", "Cancelamento solicitado pelo cliente no mesmo dia");

        Assert.Equal(StatusDocumentoFiscal.Cancelada, nfce.Status);
        Assert.Equal("135250000098765", nfce.ProtocoloCancelamento);
        Assert.Equal("Cancelamento solicitado pelo cliente no mesmo dia", nfce.JustificativaCancelamento);
        Assert.NotNull(nfce.DataCancelamento);
    }

    [Fact]
    public void Deve_impedir_cancelar_nfce_pendente()
    {
        var nfce = CriarNfce();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            nfce.Cancelar("135250000098765", "Cancelamento solicitado pelo cliente no mesmo dia"));

        Assert.Equal("Somente uma NFC-e autorizada pode ser cancelada.", excecao.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_impedir_cancelar_sem_protocolo(string? protocolo)
    {
        var nfce = CriarNfceAutorizada();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            nfce.Cancelar(protocolo!, "Cancelamento solicitado pelo cliente no mesmo dia"));

        Assert.Equal("O protocolo de cancelamento deve ser informado.", excecao.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("Motivo curto")]
    public void Deve_impedir_justificativa_de_cancelamento_curta(string? justificativa)
    {
        var nfce = CriarNfceAutorizada();

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            nfce.Cancelar("135250000098765", justificativa!));

        Assert.Equal(
            "A justificativa de cancelamento deve ter ao menos 15 caracteres.",
            excecao.Message);
    }

    [Fact]
    public void Deve_impedir_cancelar_nfce_ja_cancelada()
    {
        var nfce = CriarNfceAutorizada();
        nfce.Cancelar("135250000098765", "Cancelamento solicitado pelo cliente no mesmo dia");

        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            nfce.Cancelar("135250000098766", "Segunda tentativa de cancelamento da mesma nota"));

        Assert.Equal("Somente uma NFC-e autorizada pode ser cancelada.", excecao.Message);
    }

    #endregion
}
