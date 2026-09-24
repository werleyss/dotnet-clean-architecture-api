using DotNetCleanArchitecture.Application.Empresas;
using DotNetCleanArchitecture.Application.Empresas.DTOs;
using DotNetCleanArchitecture.Application.Tests.Fakes;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;

namespace DotNetCleanArchitecture.Application.Tests.Empresas;

public class CadastrarEmpresaUseCaseTests
{
    private static EnderecoRequest EnderecoValido()
        => new(
            Logradouro: "Arno 33 Alameda 3",
            Numero: "33",
            Complemento: null,
            Bairro: "Plano Diretor Norte",
            CodigoIBGE: 1721000,
            Cidade: "Palmas",
            CodigoUf: 17,
            Uf: "TO",
            Cep: "77001262",
            CodigoPais: 1058,
            Pais: "Brasil");

    private static CadastrarEmpresaRequest RequestValido(
        string nome = "Locações de Automóveis",
        string fantasia = "LA Automóveis",
        TipoDocumento tipoDocumento = TipoDocumento.CNPJ,
        string numeroDocumento = "11222333000181",
        CRT crt = CRT.SimplesNacional)
        => new(
            nome,
            fantasia,
            crt,
            tipoDocumento,
            numeroDocumento,
            "123456789",
            null,
            null,
            "7711000",
            "63999857658",
            EnderecoValido());

    [Fact]
    public async Task Deve_cadastrar_empresa_com_dados_validos()
    {
        var repositorio = new FakeEmpresaRepositorio();
        var useCase = new CadastrarEmpresaUseCase(repositorio);

        var response = await useCase.ExecutarAsync(RequestValido());

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("Locações de Automóveis", response.Nome);
        Assert.Equal("LA Automóveis", response.Fantasia);
        Assert.Equal("SimplesNacional", response.CRT);
        Assert.Equal("CNPJ", response.TipoDocumento);
        Assert.Equal("11222333000181", response.NumeroDocumento);
        Assert.Equal("Palmas", response.Cidade);
        Assert.Equal("TO", response.Uf);

        var salva = await repositorio.ObterPorIdAsync(response.Id);
        Assert.NotNull(salva);
    }

    [Fact]
    public async Task Deve_impedir_cadastro_com_documento_ja_existente()
    {
        var repositorio = new FakeEmpresaRepositorio();
        var useCase = new CadastrarEmpresaUseCase(repositorio);
        await useCase.ExecutarAsync(RequestValido());

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(RequestValido(nome: "Outro Nome")));

        Assert.Equal("Já existe uma empresa cadastrada com esse documento.", excecao.Message);
    }

    [Fact]
    public async Task Deve_propagar_validacao_de_dominio_do_documento_invalido()
    {
        var repositorio = new FakeEmpresaRepositorio();
        var useCase = new CadastrarEmpresaUseCase(repositorio);

        var request = RequestValido(numeroDocumento: "11111111111111");

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() => useCase.ExecutarAsync(request));

        Assert.Equal("CNPJ inválido.", excecao.Message);
    }

    [Fact]
    public async Task Deve_propagar_validacao_de_dominio_sem_fantasia()
    {
        var repositorio = new FakeEmpresaRepositorio();
        var useCase = new CadastrarEmpresaUseCase(repositorio);

        var request = RequestValido(fantasia: "");

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() => useCase.ExecutarAsync(request));

        Assert.Equal("O nome fantasia deve ser informado.", excecao.Message);
    }

    [Fact]
    public async Task Deve_impedir_tipo_de_documento_invalido()
    {
        var repositorio = new FakeEmpresaRepositorio();
        var useCase = new CadastrarEmpresaUseCase(repositorio);

        var request = RequestValido(tipoDocumento: (TipoDocumento)99);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() => useCase.ExecutarAsync(request));

        Assert.Equal("O tipo de documento informado é inválido.", excecao.Message);
    }
}
