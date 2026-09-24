using DotNetCleanArchitecture.Application.Nfces;
using DotNetCleanArchitecture.Application.Nfces.DTOs;
using DotNetCleanArchitecture.Application.Tests.Fakes;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Application.Tests.Nfces;

public class RejeitarNfceUseCaseTests
{
    private static async Task<(FakeNfceRepositorio repositorio, Guid nfceId)> CriarNfceAsync()
    {
        var repositorio = new FakeNfceRepositorio();
        var nfce = new Nfce(Guid.NewGuid(), 1001, 1, AmbienteEmissao.Homologacao);
        await repositorio.AdicionarAsync(nfce);
        return (repositorio, nfce.Id);
    }

    [Fact]
    public async Task Deve_rejeitar_nfce_pendente()
    {
        var (repositorio, nfceId) = await CriarNfceAsync();
        var useCase = new RejeitarNfceUseCase(repositorio);

        var response = await useCase.ExecutarAsync(new RejeitarNfceRequest(nfceId, "Duplicidade de NF-e"));

        Assert.Equal("Rejeitada", response.Status);
        Assert.Equal("Duplicidade de NF-e", response.MotivoStatus);
    }

    [Fact]
    public async Task Deve_impedir_rejeitar_nfce_inexistente()
    {
        var repositorio = new FakeNfceRepositorio();
        var useCase = new RejeitarNfceUseCase(repositorio);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new RejeitarNfceRequest(Guid.NewGuid(), "motivo")));

        Assert.Equal("NFC-e não encontrada.", excecao.Message);
    }

    [Fact]
    public async Task Deve_propagar_validacao_de_dominio_sem_motivo()
    {
        var (repositorio, nfceId) = await CriarNfceAsync();
        var useCase = new RejeitarNfceUseCase(repositorio);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new RejeitarNfceRequest(nfceId, "")));

        Assert.Equal("O motivo da rejeição deve ser informado.", excecao.Message);
    }

    [Fact]
    public async Task Deve_impedir_rejeitar_nfce_ja_rejeitada()
    {
        var (repositorio, nfceId) = await CriarNfceAsync();
        var useCase = new RejeitarNfceUseCase(repositorio);
        await useCase.ExecutarAsync(new RejeitarNfceRequest(nfceId, "Duplicidade de NF-e"));

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new RejeitarNfceRequest(nfceId, "outro motivo")));

        Assert.Equal("Somente uma NFC-e pendente pode ser rejeitada.", excecao.Message);
    }
}
