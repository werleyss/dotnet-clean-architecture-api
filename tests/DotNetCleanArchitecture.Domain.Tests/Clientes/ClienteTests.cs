using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Core.ObjetosValor;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Domain.Tests.Clientes;

public class ClienteTests
{
    private static Documento DocumentoValido()
        => Cnpj.Criar("11.222.333/0001-81");

    private static Endereco EnderecoValido()
        => Endereco.Criar(
            logradouro: "Arno 33 Alameda 3",
            numero: "33",
            complemento: null,
            bairro: "Plano Diretor Norte",
            codigoIBGE: 1721000,
            cidade: "Palmas",
            codigoUf: 17,
            uf: "TO",
            cep: "77001262",
            codigoPais: 1058,
            pais: "BRASIL");

    [Fact]
    public void Deve_criar_cliente_contribuinte_com_dados_validos()
    {
        // Arrange
        var documento = DocumentoValido();
        var endereco = EnderecoValido();

        // Act
        var cliente = new Cliente(
            nome: "Locações de Automóveis",
            fantasia: "LA Automóveis",
            documento: documento,
            indicadorIE: IndicadorIE.Contribuinte,
            ie: "123456789",
            im: "987654321",
            celular: "63999857658",
            fone: "6332255566",
            email: "contato@la.com.br",
            endereco: endereco);

        // Assert
        Assert.NotNull(cliente);
        Assert.Equal("Locações de Automóveis", cliente.Nome);
        Assert.Equal("LA Automóveis", cliente.Fantasia);
        Assert.Equal(documento, cliente.Documento);
        Assert.Equal(IndicadorIE.Contribuinte, cliente.IndicadorIE);
        Assert.Equal("123456789", cliente.IE);
        Assert.Equal("987654321", cliente.IM);
        Assert.Equal("63999857658", cliente.Celular);
        Assert.Equal("6332255566", cliente.Fone);
        Assert.Equal("contato@la.com.br", cliente.Email);
        Assert.Equal(endereco, cliente.Endereco);
    }

    [Theory]
    [InlineData(IndicadorIE.ContribuinteIsento)]
    [InlineData(IndicadorIE.NaoContribuinte)]
    public void Deve_criar_cliente_sem_ie_quando_nao_for_contribuinte(IndicadorIE indicadorIE)
    {
        // Act
        var cliente = new Cliente(
            nome: "João da Silva",
            fantasia: null,
            documento: Cpf.Criar("529.982.247-25"),
            indicadorIE: indicadorIE,
            ie: null,
            im: null,
            celular: null,
            fone: null,
            email: null,
            endereco: EnderecoValido());

        // Assert
        Assert.NotNull(cliente);
        Assert.Equal(indicadorIE, cliente.IndicadorIE);
        Assert.Null(cliente.Fantasia);
        Assert.Null(cliente.IE);
    }

    [Fact]
    public void Deve_remover_espacos_dos_dados()
    {
        // Act
        var cliente = new Cliente(
            nome: "  Locações de Automóveis  ",
            fantasia: "  LA Automóveis  ",
            documento: DocumentoValido(),
            indicadorIE: IndicadorIE.Contribuinte,
            ie: "  123456789  ",
            im: "  987654321  ",
            celular: "  63999857658  ",
            fone: "  6332255566  ",
            email: "  contato@la.com.br  ",
            endereco: EnderecoValido());

        // Assert
        Assert.Equal("Locações de Automóveis", cliente.Nome);
        Assert.Equal("LA Automóveis", cliente.Fantasia);
        Assert.Equal("123456789", cliente.IE);
        Assert.Equal("987654321", cliente.IM);
        Assert.Equal("63999857658", cliente.Celular);
        Assert.Equal("6332255566", cliente.Fone);
        Assert.Equal("contato@la.com.br", cliente.Email);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_impedir_cliente_sem_nome(string? nome)
    {
        // Act
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            new Cliente(
                nome: nome!,
                fantasia: "LA Automóveis",
                documento: DocumentoValido(),
                indicadorIE: IndicadorIE.NaoContribuinte,
                ie: null,
                im: null,
                celular: null,
                fone: null,
                email: null,
                endereco: EnderecoValido()));

        // Assert
        Assert.Equal("O nome do cliente deve ser informado.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_cliente_sem_documento()
    {
        // Act
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            new Cliente(
                nome: "Locações de Automóveis",
                fantasia: "LA Automóveis",
                documento: null!,
                indicadorIE: IndicadorIE.NaoContribuinte,
                ie: null,
                im: null,
                celular: null,
                fone: null,
                email: null,
                endereco: EnderecoValido()));

        // Assert
        Assert.Equal("O documento do cliente deve ser informado.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_cliente_sem_endereco()
    {
        // Act
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            new Cliente(
                nome: "Locações de Automóveis",
                fantasia: "LA Automóveis",
                documento: DocumentoValido(),
                indicadorIE: IndicadorIE.NaoContribuinte,
                ie: null,
                im: null,
                celular: null,
                fone: null,
                email: null,
                endereco: null!));

        // Assert
        Assert.Equal("O endereço do cliente deve ser informado.", excecao.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Deve_impedir_contribuinte_sem_inscricao_estadual(string? ie)
    {
        // Act
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            new Cliente(
                nome: "Locações de Automóveis",
                fantasia: "LA Automóveis",
                documento: DocumentoValido(),
                indicadorIE: IndicadorIE.Contribuinte,
                ie: ie,
                im: null,
                celular: null,
                fone: null,
                email: null,
                endereco: EnderecoValido()));

        // Assert
        Assert.Equal(
            "A inscrição estadual deve ser informada para contribuinte.",
            excecao.Message);
    }

    [Theory]
    [InlineData(IndicadorIE.ContribuinteIsento)]
    [InlineData(IndicadorIE.NaoContribuinte)]
    public void Deve_impedir_inscricao_estadual_quando_nao_for_contribuinte(IndicadorIE indicadorIE)
    {
        // Act
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            new Cliente(
                nome: "Locações de Automóveis",
                fantasia: "LA Automóveis",
                documento: DocumentoValido(),
                indicadorIE: indicadorIE,
                ie: "123456789",
                im: null,
                celular: null,
                fone: null,
                email: null,
                endereco: EnderecoValido()));

        // Assert
        Assert.Equal(
            "A inscrição estadual não deve ser informada para não contribuinte ou isento.",
            excecao.Message);
    }
}
