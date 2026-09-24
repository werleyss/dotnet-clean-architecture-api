using DotNetCleanArchitecture.Application.Nfes;
using DotNetCleanArchitecture.Application.Nfes.DTOs;
using DotNetCleanArchitecture.Application.Tests.Fakes;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Application.Tests.Nfes;

public class DenegarNfeUseCaseTests
{
    private static async Task<(FakeNfeRepositorio repositorio, Guid nfeId)> CriarNfeAsync()
    {
        var repositorio = new FakeNfeRepositorio();
        var nfe = new Nfe(Guid.NewGuid(), 501, 1, AmbienteEmissao.Homologacao, ModalidadeFrete.CifRemetente);
        await repositorio.AdicionarAsync(nfe);
        return (repositorio, nfe.Id);
    }

    [Fact]
    public async Task Deve_denegar_nfe_pendente()
    {
        var (repositorio, nfeId) = await CriarNfeAsync();
        var useCase = new DenegarNfeUseCase(repositorio);

        var response = await useCase.ExecutarAsync(
            new DenegarNfeRequest(nfeId, "CNPJ do destinatário irregular"));

        Assert.Equal("Denegada", response.Status);
        Assert.Equal("CNPJ do destinatário irregular", response.MotivoStatus);
    }

    [Fact]
    public async Task Deve_impedir_denegar_nfe_inexistente()
    {
        var repositorio = new FakeNfeRepositorio();
        var useCase = new DenegarNfeUseCase(repositorio);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new DenegarNfeRequest(Guid.NewGuid(), "motivo")));

        Assert.Equal("NF-e não encontrada.", excecao.Message);
    }

    [Fact]
    public async Task Deve_propagar_validacao_de_dominio_sem_motivo()
    {
        var (repositorio, nfeId) = await CriarNfeAsync();
        var useCase = new DenegarNfeUseCase(repositorio);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new DenegarNfeRequest(nfeId, "")));

        Assert.Equal("O motivo da denegação deve ser informado.", excecao.Message);
    }

    [Fact]
    public async Task Deve_impedir_denegar_nfe_ja_denegada()
    {
        var (repositorio, nfeId) = await CriarNfeAsync();
        var useCase = new DenegarNfeUseCase(repositorio);
        await useCase.ExecutarAsync(new DenegarNfeRequest(nfeId, "CNPJ do destinatário irregular"));

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new DenegarNfeRequest(nfeId, "outro motivo")));

        Assert.Equal("Somente uma NF-e pendente pode ser denegada.", excecao.Message);
    }
}
