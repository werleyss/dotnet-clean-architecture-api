using DotNetCleanArchitecture.Application.Nfes;
using DotNetCleanArchitecture.Application.Nfes.DTOs;
using DotNetCleanArchitecture.Application.Tests.Fakes;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Application.Tests.Nfes;

public class RejeitarNfeUseCaseTests
{
    private static async Task<(FakeNfeRepositorio repositorio, Guid nfeId)> CriarNfeAsync()
    {
        var repositorio = new FakeNfeRepositorio();
        var nfe = new Nfe(Guid.NewGuid(), 501, 1, AmbienteEmissao.Homologacao, ModalidadeFrete.CifRemetente);
        await repositorio.AdicionarAsync(nfe);
        return (repositorio, nfe.Id);
    }

    [Fact]
    public async Task Deve_rejeitar_nfe_pendente()
    {
        var (repositorio, nfeId) = await CriarNfeAsync();
        var useCase = new RejeitarNfeUseCase(repositorio);

        var response = await useCase.ExecutarAsync(new RejeitarNfeRequest(nfeId, "Duplicidade de NF-e"));

        Assert.Equal("Rejeitada", response.Status);
        Assert.Equal("Duplicidade de NF-e", response.MotivoStatus);
    }

    [Fact]
    public async Task Deve_impedir_rejeitar_nfe_inexistente()
    {
        var repositorio = new FakeNfeRepositorio();
        var useCase = new RejeitarNfeUseCase(repositorio);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new RejeitarNfeRequest(Guid.NewGuid(), "motivo")));

        Assert.Equal("NF-e não encontrada.", excecao.Message);
    }

    [Fact]
    public async Task Deve_propagar_validacao_de_dominio_sem_motivo()
    {
        var (repositorio, nfeId) = await CriarNfeAsync();
        var useCase = new RejeitarNfeUseCase(repositorio);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new RejeitarNfeRequest(nfeId, "")));

        Assert.Equal("O motivo da rejeição deve ser informado.", excecao.Message);
    }

    [Fact]
    public async Task Deve_impedir_rejeitar_nfe_ja_rejeitada()
    {
        var (repositorio, nfeId) = await CriarNfeAsync();
        var useCase = new RejeitarNfeUseCase(repositorio);
        await useCase.ExecutarAsync(new RejeitarNfeRequest(nfeId, "Duplicidade de NF-e"));

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new RejeitarNfeRequest(nfeId, "outro motivo")));

        Assert.Equal("Somente uma NF-e pendente pode ser rejeitada.", excecao.Message);
    }
}
