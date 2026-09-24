using DotNetCleanArchitecture.Application.Nfes;
using DotNetCleanArchitecture.Application.Nfes.DTOs;
using DotNetCleanArchitecture.Application.Tests.Fakes;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Application.Tests.Nfes;

public class AutorizarNfeUseCaseTests
{
    private const string ChaveAcessoValida = "35240112345678000190550010000010011234567890";

    private static async Task<(FakeNfeRepositorio repositorio, Guid nfeId)> CriarNfeAsync()
    {
        var repositorio = new FakeNfeRepositorio();
        var nfe = new Nfe(Guid.NewGuid(), 501, 1, AmbienteEmissao.Homologacao, ModalidadeFrete.CifRemetente);
        await repositorio.AdicionarAsync(nfe);
        return (repositorio, nfe.Id);
    }

    [Fact]
    public async Task Deve_autorizar_nfe_pendente()
    {
        var (repositorio, nfeId) = await CriarNfeAsync();
        var useCase = new AutorizarNfeUseCase(repositorio);

        var response = await useCase.ExecutarAsync(
            new AutorizarNfeRequest(nfeId, ChaveAcessoValida, "135240000012345"));

        Assert.Equal("Autorizada", response.Status);
        Assert.Equal(ChaveAcessoValida, response.ChaveAcesso);
        Assert.Equal("135240000012345", response.ProtocoloAutorizacao);
    }

    [Fact]
    public async Task Deve_impedir_autorizar_nfe_inexistente()
    {
        var repositorio = new FakeNfeRepositorio();
        var useCase = new AutorizarNfeUseCase(repositorio);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new AutorizarNfeRequest(Guid.NewGuid(), ChaveAcessoValida, "protocolo")));

        Assert.Equal("NF-e não encontrada.", excecao.Message);
    }

    [Fact]
    public async Task Deve_propagar_validacao_de_dominio_da_chave_invalida()
    {
        var (repositorio, nfeId) = await CriarNfeAsync();
        var useCase = new AutorizarNfeUseCase(repositorio);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new AutorizarNfeRequest(nfeId, "123", "protocolo")));

        Assert.Equal("A chave de acesso da NF-e deve conter 44 dígitos.", excecao.Message);
    }

    [Fact]
    public async Task Deve_impedir_autorizar_nfe_ja_autorizada()
    {
        var (repositorio, nfeId) = await CriarNfeAsync();
        var useCase = new AutorizarNfeUseCase(repositorio);
        await useCase.ExecutarAsync(new AutorizarNfeRequest(nfeId, ChaveAcessoValida, "135240000012345"));

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new AutorizarNfeRequest(nfeId, ChaveAcessoValida, "outro-protocolo")));

        Assert.Equal("Somente uma NF-e pendente pode ser autorizada.", excecao.Message);
    }
}
