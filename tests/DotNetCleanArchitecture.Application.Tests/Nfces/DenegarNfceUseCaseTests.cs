using DotNetCleanArchitecture.Application.Nfces;
using DotNetCleanArchitecture.Application.Nfces.DTOs;
using DotNetCleanArchitecture.Application.Tests.Fakes;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Application.Tests.Nfces;

public class DenegarNfceUseCaseTests
{
    private static async Task<(FakeNfceRepositorio repositorio, Guid nfceId)> CriarNfceAsync()
    {
        var repositorio = new FakeNfceRepositorio();
        var nfce = new Nfce(Guid.NewGuid(), 1001, 1, AmbienteEmissao.Homologacao);
        await repositorio.AdicionarAsync(nfce);
        return (repositorio, nfce.Id);
    }

    [Fact]
    public async Task Deve_denegar_nfce_pendente()
    {
        var (repositorio, nfceId) = await CriarNfceAsync();
        var useCase = new DenegarNfceUseCase(repositorio);

        var response = await useCase.ExecutarAsync(
            new DenegarNfceRequest(nfceId, "CNPJ do destinatário irregular"));

        Assert.Equal("Denegada", response.Status);
        Assert.Equal("CNPJ do destinatário irregular", response.MotivoStatus);
    }

    [Fact]
    public async Task Deve_impedir_denegar_nfce_inexistente()
    {
        var repositorio = new FakeNfceRepositorio();
        var useCase = new DenegarNfceUseCase(repositorio);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new DenegarNfceRequest(Guid.NewGuid(), "motivo")));

        Assert.Equal("NFC-e não encontrada.", excecao.Message);
    }

    [Fact]
    public async Task Deve_propagar_validacao_de_dominio_sem_motivo()
    {
        var (repositorio, nfceId) = await CriarNfceAsync();
        var useCase = new DenegarNfceUseCase(repositorio);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new DenegarNfceRequest(nfceId, "")));

        Assert.Equal("O motivo da denegação deve ser informado.", excecao.Message);
    }

    [Fact]
    public async Task Deve_impedir_denegar_nfce_ja_denegada()
    {
        var (repositorio, nfceId) = await CriarNfceAsync();
        var useCase = new DenegarNfceUseCase(repositorio);
        await useCase.ExecutarAsync(new DenegarNfceRequest(nfceId, "CNPJ do destinatário irregular"));

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new DenegarNfceRequest(nfceId, "outro motivo")));

        Assert.Equal("Somente uma NFC-e pendente pode ser denegada.", excecao.Message);
    }
}
