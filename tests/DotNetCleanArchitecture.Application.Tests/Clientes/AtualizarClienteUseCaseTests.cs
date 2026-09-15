using DotNetCleanArchitecture.Application.Clientes;
using DotNetCleanArchitecture.Application.Clientes.DTOs;
using DotNetCleanArchitecture.Application.Tests.Fakes;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;

namespace DotNetCleanArchitecture.Application.Tests.Clientes;

public class AtualizarClienteUseCaseTests
{
    private static EnderecoRequest EnderecoValido(string cidade = "Palmas", string uf = "TO")
        => new(
            "Arno 33 Alameda 3",
            "33",
            null,
            "Plano Diretor Norte",
            1721000,
            cidade,
            17,
            uf,
            "77001262",
            1058,
            "Brasil");

    private static async Task<(FakeClienteRepositorio repositorio, Guid clienteId)> CadastrarClienteAsync()
    {
        var repositorio = new FakeClienteRepositorio();
        var cadastrar = new CadastrarClienteUseCase(repositorio);

        var response = await cadastrar.ExecutarAsync(new CadastrarClienteRequest(
            "Locações de Automóveis",
            "LA Automóveis",
            TipoDocumento.CNPJ,
            "11222333000181",
            IndicadorIE.NaoContribuinte,
            null,
            null,
            null,
            null,
            null,
            EnderecoValido()));

        return (repositorio, response.Id);
    }

    [Fact]
    public async Task Deve_atualizar_dados_do_cliente()
    {
        var (repositorio, clienteId) = await CadastrarClienteAsync();
        var useCase = new AtualizarClienteUseCase(repositorio);

        var request = new AtualizarClienteRequest(
            clienteId,
            "Novo Nome",
            "Novo Fantasia",
            IndicadorIE.NaoContribuinte,
            null,
            null,
            "63999999999",
            null,
            "novo@email.com",
            EnderecoValido(cidade: "Araguaína"));

        var response = await useCase.ExecutarAsync(request);

        Assert.Equal("Novo Nome", response.Nome);
        Assert.Equal("Novo Fantasia", response.Fantasia);
        Assert.Equal("63999999999", response.Celular);
        Assert.Equal("novo@email.com", response.Email);
        Assert.Equal("Araguaína", response.Cidade);

        var salvo = await repositorio.ObterPorIdAsync(clienteId);
        Assert.Equal("Novo Nome", salvo!.Nome);
    }

    [Fact]
    public async Task Nao_deve_alterar_o_documento_do_cliente()
    {
        var (repositorio, clienteId) = await CadastrarClienteAsync();
        var useCase = new AtualizarClienteUseCase(repositorio);

        var request = new AtualizarClienteRequest(
            clienteId,
            "Novo Nome",
            null,
            IndicadorIE.NaoContribuinte,
            null,
            null,
            null,
            null,
            null,
            EnderecoValido());

        var response = await useCase.ExecutarAsync(request);

        Assert.Equal("11222333000181", response.NumeroDocumento);
    }

    [Fact]
    public async Task Deve_impedir_atualizar_cliente_inexistente()
    {
        var repositorio = new FakeClienteRepositorio();
        var useCase = new AtualizarClienteUseCase(repositorio);

        var request = new AtualizarClienteRequest(
            Guid.NewGuid(),
            "Nome",
            null,
            IndicadorIE.NaoContribuinte,
            null,
            null,
            null,
            null,
            null,
            EnderecoValido());

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() => useCase.ExecutarAsync(request));

        Assert.Equal("Cliente não encontrado.", excecao.Message);
    }

    [Fact]
    public async Task Deve_propagar_validacao_de_dominio_ao_atualizar()
    {
        var (repositorio, clienteId) = await CadastrarClienteAsync();
        var useCase = new AtualizarClienteUseCase(repositorio);

        var request = new AtualizarClienteRequest(
            clienteId,
            "",
            null,
            IndicadorIE.NaoContribuinte,
            null,
            null,
            null,
            null,
            null,
            EnderecoValido());

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() => useCase.ExecutarAsync(request));

        Assert.Equal("O nome do cliente deve ser informado.", excecao.Message);
    }
}
