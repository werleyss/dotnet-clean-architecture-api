using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Core.ObjetosValor;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Domain.Tests;

public class EmpresaTests
{
    [Fact]
    public void Deve_criar_empresa_com_dados_valido()
    {
        // Arrange
        var documento = Cnpj.Criar("11.222.333/0001-81");

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
            pais: "BRASIL");

        // Act 
        var empresa = new Empresa(
            nome: "Locações de Automóveis",
            fantasia: "LA Automóveis",
            crt: CRT.SimplesNacional,
            documento: documento,
            ie: "123456789",
            iest: null,
            im: null,
            cnae: "7711000",
            fone: "63999857658",
            endereco: endereco);

        // Assert 
        Assert.NotNull(empresa);
        Assert.Equal("Locações de Automóveis", empresa.Nome);
        Assert.Equal("LA Automóveis", empresa.Fantasia);
        Assert.Equal(CRT.SimplesNacional, empresa.CRT);
        Assert.Equal(documento, empresa.Documento);
        Assert.Equal(endereco, empresa.Endereco);
    }

    [Fact]
    public void Deve_impedir_empresa_sem_nome()
    {
        // Arrange
        var documento = Cnpj.Criar("11222333000181");

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
            pais: "BRASIL");

        // Act 
        var excecao = Assert.Throws<ExcecaoDeDominio>(() => 
            new Empresa(
                nome: "",
                fantasia: "LA Automóveis",
                crt: CRT.SimplesNacional,
                documento: documento,
                ie: "123456789",
                iest: null,
                im: null,
                cnae: "7711000",
                fone: "63999857658",
                endereco: endereco));

        // Assert 
        Assert.Equal("O nome da empresa deve ser informado.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_empresa_sem_fantasia()
    {
        // Arrange
        var documento = Cnpj.Criar("11222333000181");

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
            pais: "BRASIL");

        // Act 
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            new Empresa(
                nome: "Locações de Automóveis",
                fantasia: "",
                crt: CRT.SimplesNacional,
                documento: documento,
                ie: "123456789",
                iest: null,
                im: null,
                cnae: "7711000",
                fone: "63999857658",
                endereco: endereco));

        // Assert 
        Assert.Equal("O nome fantasia deve ser informado.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_empresa_sem_documento()
    {
        // Arrange
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
            pais: "BRASIL");

        // Act 
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            new Empresa(
                nome: "Locações de Automóveis",
                fantasia: "LA Automóveis",
                crt: CRT.SimplesNacional,
                documento: null!,
                ie: "123456789",
                iest: null,
                im: null,
                cnae: "7711000",
                fone: "63999857658",
                endereco: endereco));

        // Assert 
        Assert.Equal("O documento da empresa deve ser informado.", excecao.Message);
    }

    [Fact]
    public void Deve_impedir_empresa_sem_endereco()
    {
        // Arrange
        var documento = Cnpj.Criar("11222333000181");

        // Act 
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            new Empresa(
                nome: "Locações de Automóveis",
                fantasia: "LA Automóveis",
                crt: CRT.SimplesNacional,
                documento: documento,
                ie: "123456789",
                iest: null,
                im: null,
                cnae: "7711000",
                fone: "63999857658",
                endereco: null!));

        // Assert 
        Assert.Equal("O endereço da empresa deve ser informado.", excecao.Message);
    }
}
