using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Core.ObjetosValor;

namespace DotNetCleanArchitecture.Domain.Tests.ObjetosValor;

public class EnderecoTests
{
    [Fact]
    public void Deve_criar_endereco_com_dados_validos()
    {
        // Act
        var endereco = Endereco.Criar(
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
            pais: "Brasil");

        // Assert
        Assert.NotNull(endereco);
        Assert.Equal("Arno 33 Alameda 3", endereco.Logradouro);
        Assert.Equal("33", endereco.Numero);
        Assert.Null(endereco.Complemento);
        Assert.Equal("Plano Diretor Norte", endereco.Bairro);
        Assert.Equal(1721000, endereco.CodigoIBGE);
        Assert.Equal("Palmas", endereco.Cidade);
        Assert.Equal(17, endereco.CodigoUf);
        Assert.Equal("TO", endereco.Uf);
        Assert.Equal("77001262", endereco.Cep);
        Assert.Equal(1058, endereco.CodigoPais);
        Assert.Equal("Brasil", endereco.Pais);
    }

    [Fact]
    public void Deve_normalizar_uf_para_maiusculo()
    {
        // Act
        var endereco = Endereco.Criar(
            "Arno 33 Alameda 3",
            "33",
            null,
            "Plano Diretor Norte",
            1721000,
            "Palmas",
            17,
            "to",
            "77001262",
            1058,
            "Brasil");

        // Assert
        Assert.Equal("TO", endereco.Uf);
    }

    [Fact]
    public void Deve_remover_espacos_dos_dados()
    {
        // Act
        var endereco = Endereco.Criar(
            "  Arno 33 Alameda 3  ",
            " 33 ",
            "  Sala 01  ",
            " Plano Diretor Norte ",
            1721000,
            " Palmas ",
            17,
            " TO ",
            " 77001262 ",
            1058,
            " Brasil ");

        // Assert
        Assert.Equal("Arno 33 Alameda 3", endereco.Logradouro);
        Assert.Equal("33", endereco.Numero);
        Assert.Equal("Sala 01", endereco.Complemento);
        Assert.Equal("Plano Diretor Norte", endereco.Bairro);
        Assert.Equal("Palmas", endereco.Cidade);
        Assert.Equal("TO", endereco.Uf);
        Assert.Equal("77001262", endereco.Cep);
        Assert.Equal("Brasil", endereco.Pais);
    }

    [Fact]
    public void Deve_impedir_endereco_sem_logradouro()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            Endereco.Criar(
                "",
                "33",
                null,
                "Plano Diretor Norte",
                1721000,
                "Palmas",
                17,
                "TO",
                "77001262",
                1058,
                "Brasil"));

        Assert.Equal(
            "O logradouro deve ser informado.",
            excecao.Message);
    }

    [Fact]
    public void Deve_impedir_endereco_sem_numero()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            Endereco.Criar(
                "Arno 33 Alameda 3",
                "",
                null,
                "Plano Diretor Norte",
                1721000,
                "Palmas",
                17,
                "TO",
                "77001262",
                1058,
                "Brasil"));

        Assert.Equal(
            "O número deve ser informado.",
            excecao.Message);
    }

    [Fact]
    public void Deve_impedir_endereco_sem_bairro()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            Endereco.Criar(
                "Arno 33 Alameda 3",
                "33",
                null,
                "",
                1721000,
                "Palmas",
                17,
                "TO",
                "77001262",
                1058,
                "Brasil"));

        Assert.Equal(
            "O bairro deve ser informado.",
            excecao.Message);
    }

    [Fact]
    public void Deve_impedir_endereco_sem_codigo_ibge()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            Endereco.Criar(
                "Arno 33 Alameda 3",
                "33",
                null,
                "Plano Diretor Norte",
                0,
                "Palmas",
                17,
                "TO",
                "77001262",
                1058,
                "Brasil"));

        Assert.Equal(
            "O código do IBGE deve ser informado.",
            excecao.Message);
    }

    [Fact]
    public void Deve_impedir_endereco_sem_cidade()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            Endereco.Criar(
                "Arno 33 Alameda 3",
                "33",
                null,
                "Plano Diretor Norte",
                1721000,
                "",
                17,
                "TO",
                "77001262",
                1058,
                "Brasil"));

        Assert.Equal(
            "A cidade deve ser informada.",
            excecao.Message);
    }

    [Fact]
    public void Deve_impedir_endereco_sem_codigo_uf()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            Endereco.Criar(
                "Arno 33 Alameda 3",
                "33",
                null,
                "Plano Diretor Norte",
                1721000,
                "Palmas",
                0,
                "TO",
                "77001262",
                1058,
                "Brasil"));

        Assert.Equal(
            "O código da UF deve ser informado.",
            excecao.Message);
    }

    [Fact]
    public void Deve_impedir_endereco_sem_uf()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            Endereco.Criar(
                "Arno 33 Alameda 3",
                "33",
                null,
                "Plano Diretor Norte",
                1721000,
                "Palmas",
                17,
                "",
                "77001262",
                1058,
                "Brasil"));

        Assert.Equal(
            "A UF deve ser informada.",
            excecao.Message);
    }

    [Fact]
    public void Deve_impedir_endereco_sem_cep()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            Endereco.Criar(
                "Arno 33 Alameda 3",
                "33",
                null,
                "Plano Diretor Norte",
                1721000,
                "Palmas",
                17,
                "TO",
                "",
                1058,
                "Brasil"));

        Assert.Equal(
            "O CEP deve ser informado.",
            excecao.Message);
    }

    [Fact]
    public void Deve_impedir_endereco_sem_codigo_pais()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            Endereco.Criar(
                "Arno 33 Alameda 3",
                "33",
                null,
                "Plano Diretor Norte",
                1721000,
                "Palmas",
                17,
                "TO",
                "77001262",
                0,
                "Brasil"));

        Assert.Equal(
            "O código do país deve ser informado.",
            excecao.Message);
    }

    [Fact]
    public void Deve_impedir_endereco_sem_pais()
    {
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            Endereco.Criar(
                "Arno 33 Alameda 3",
                "33",
                null,
                "Plano Diretor Norte",
                1721000,
                "Palmas",
                17,
                "TO",
                "77001262",
                1058,
                ""));

        Assert.Equal(
            "O país deve ser informado.",
            excecao.Message);
    }
}