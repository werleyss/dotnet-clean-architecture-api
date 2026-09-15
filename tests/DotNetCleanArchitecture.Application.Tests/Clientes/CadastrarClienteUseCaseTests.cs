using DotNetCleanArchitecture.Application.Clientes;
using DotNetCleanArchitecture.Application.Clientes.DTOs;
using DotNetCleanArchitecture.Application.Tests.Fakes;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;

namespace DotNetCleanArchitecture.Application.Tests.Clientes;

public class CadastrarClienteUseCaseTests
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

    private static CadastrarClienteRequest RequestValido(
        string nome = "Locações de Automóveis",
        string? fantasia = "LA Automóveis",
        TipoDocumento tipoDocumento = TipoDocumento.CNPJ,
        string numeroDocumento = "11222333000181",
        IndicadorIE indicadorIE = IndicadorIE.NaoContribuinte,
        string? ie = null)
        => new(
            nome,
            fantasia,
            tipoDocumento,
            numeroDocumento,
            indicadorIE,
            ie,
            null,
            null,
            null,
            null,
            EnderecoValido());

    [Fact]
    public async Task Deve_cadastrar_cliente_com_dados_validos()
    {
        var repositorio = new FakeClienteRepositorio();
        var useCase = new CadastrarClienteUseCase(repositorio);

        var response = await useCase.ExecutarAsync(RequestValido());

        Assert.NotEqual(Guid.Empty, response.Id);
        Assert.Equal("Locações de Automóveis", response.Nome);
        Assert.Equal("LA Automóveis", response.Fantasia);
        Assert.Equal("CNPJ", response.TipoDocumento);
        Assert.Equal("11222333000181", response.NumeroDocumento);
        Assert.Equal("Palmas", response.Cidade);
        Assert.Equal("TO", response.Uf);

        var salvo = await repositorio.ObterPorIdAsync(response.Id);
        Assert.NotNull(salvo);
    }

    [Fact]
    public async Task Deve_cadastrar_cliente_com_cpf()
    {
        var repositorio = new FakeClienteRepositorio();
        var useCase = new CadastrarClienteUseCase(repositorio);

        var request = RequestValido(tipoDocumento: TipoDocumento.CPF, numeroDocumento: "52998224725");

        var response = await useCase.ExecutarAsync(request);

        Assert.Equal("CPF", response.TipoDocumento);
        Assert.Equal("52998224725", response.NumeroDocumento);
    }

    [Fact]
    public async Task Deve_impedir_cadastro_com_documento_ja_existente()
    {
        var repositorio = new FakeClienteRepositorio();
        var useCase = new CadastrarClienteUseCase(repositorio);
        await useCase.ExecutarAsync(RequestValido());

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(RequestValido(nome: "Outro Nome")));

        Assert.Equal("Já existe um cliente cadastrado com esse documento.", excecao.Message);
    }

    [Fact]
    public async Task Deve_propagar_validacao_de_dominio_do_documento_invalido()
    {
        var repositorio = new FakeClienteRepositorio();
        var useCase = new CadastrarClienteUseCase(repositorio);

        var request = RequestValido(numeroDocumento: "11111111111111");

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() => useCase.ExecutarAsync(request));

        Assert.Equal("CNPJ inválido.", excecao.Message);
    }

    [Fact]
    public async Task Deve_impedir_tipo_de_documento_invalido()
    {
        var repositorio = new FakeClienteRepositorio();
        var useCase = new CadastrarClienteUseCase(repositorio);

        var request = RequestValido(tipoDocumento: (TipoDocumento)99);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() => useCase.ExecutarAsync(request));

        Assert.Equal("O tipo de documento informado é inválido.", excecao.Message);
    }
}
