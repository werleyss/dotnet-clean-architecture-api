using DotNetCleanArchitecture.Application.Empresas;
using DotNetCleanArchitecture.Application.Empresas.DTOs;
using DotNetCleanArchitecture.Application.Tests.Fakes;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;

namespace DotNetCleanArchitecture.Application.Tests.Empresas;

public class AtualizarEmpresaUseCaseTests
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

    private static async Task<(FakeEmpresaRepositorio repositorio, Guid empresaId)> CadastrarEmpresaAsync()
    {
        var repositorio = new FakeEmpresaRepositorio();
        var cadastrar = new CadastrarEmpresaUseCase(repositorio);

        var response = await cadastrar.ExecutarAsync(new CadastrarEmpresaRequest(
            "Locações de Automóveis",
            "LA Automóveis",
            CRT.SimplesNacional,
            TipoDocumento.CNPJ,
            "11222333000181",
            "123456789",
            null,
            null,
            "7711000",
            "63999857658",
            EnderecoValido()));

        return (repositorio, response.Id);
    }

    [Fact]
    public async Task Deve_atualizar_dados_da_empresa()
    {
        var (repositorio, empresaId) = await CadastrarEmpresaAsync();
        var useCase = new AtualizarEmpresaUseCase(repositorio);

        var request = new AtualizarEmpresaRequest(
            empresaId,
            "Novo Nome",
            "Novo Fantasia",
            CRT.RegimeNormal,
            "987654321",
            null,
            null,
            "7712000",
            "63999999999",
            EnderecoValido(cidade: "Araguaína"));

        var response = await useCase.ExecutarAsync(request);

        Assert.Equal("Novo Nome", response.Nome);
        Assert.Equal("Novo Fantasia", response.Fantasia);
        Assert.Equal("RegimeNormal", response.CRT);
        Assert.Equal("987654321", response.IE);
        Assert.Equal("Araguaína", response.Cidade);

        var salva = await repositorio.ObterPorIdAsync(empresaId);
        Assert.Equal("Novo Nome", salva!.Nome);
    }

    [Fact]
    public async Task Nao_deve_alterar_o_documento_da_empresa()
    {
        var (repositorio, empresaId) = await CadastrarEmpresaAsync();
        var useCase = new AtualizarEmpresaUseCase(repositorio);

        var request = new AtualizarEmpresaRequest(
            empresaId,
            "Novo Nome",
            "Novo Fantasia",
            CRT.SimplesNacional,
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
    public async Task Deve_impedir_atualizar_empresa_inexistente()
    {
        var repositorio = new FakeEmpresaRepositorio();
        var useCase = new AtualizarEmpresaUseCase(repositorio);

        var request = new AtualizarEmpresaRequest(
            Guid.NewGuid(),
            "Nome",
            "Fantasia",
            CRT.SimplesNacional,
            null,
            null,
            null,
            null,
            null,
            EnderecoValido());

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() => useCase.ExecutarAsync(request));

        Assert.Equal("Empresa não encontrada.", excecao.Message);
    }

    [Fact]
    public async Task Deve_propagar_validacao_de_dominio_ao_atualizar()
    {
        var (repositorio, empresaId) = await CadastrarEmpresaAsync();
        var useCase = new AtualizarEmpresaUseCase(repositorio);

        var request = new AtualizarEmpresaRequest(
            empresaId,
            "",
            "Fantasia",
            CRT.SimplesNacional,
            null,
            null,
            null,
            null,
            null,
            EnderecoValido());

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() => useCase.ExecutarAsync(request));

        Assert.Equal("O nome da empresa deve ser informado.", excecao.Message);
    }
}
