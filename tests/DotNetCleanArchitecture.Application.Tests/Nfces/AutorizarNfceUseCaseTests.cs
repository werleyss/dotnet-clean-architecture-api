using DotNetCleanArchitecture.Application.Nfces;
using DotNetCleanArchitecture.Application.Nfces.DTOs;
using DotNetCleanArchitecture.Application.Tests.Fakes;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Application.Tests.Nfces;

public class AutorizarNfceUseCaseTests
{
    private const string ChaveAcessoValida = "35240112345678000190650010000010011234567890";

    private static async Task<(FakeNfceRepositorio repositorio, Guid nfceId)> CriarNfceAsync()
    {
        var repositorio = new FakeNfceRepositorio();
        var nfce = new Nfce(Guid.NewGuid(), 1001, 1, AmbienteEmissao.Homologacao);
        await repositorio.AdicionarAsync(nfce);
        return (repositorio, nfce.Id);
    }

    [Fact]
    public async Task Deve_autorizar_nfce_pendente()
    {
        var (repositorio, nfceId) = await CriarNfceAsync();
        var useCase = new AutorizarNfceUseCase(repositorio);

        var response = await useCase.ExecutarAsync(
            new AutorizarNfceRequest(nfceId, ChaveAcessoValida, "135240000012345", "qrcode"));

        Assert.Equal("Autorizada", response.Status);
        Assert.Equal(ChaveAcessoValida, response.ChaveAcesso);
        Assert.Equal("135240000012345", response.ProtocoloAutorizacao);
        Assert.Equal("qrcode", response.QrCode);
    }

    [Fact]
    public async Task Deve_impedir_autorizar_nfce_inexistente()
    {
        var repositorio = new FakeNfceRepositorio();
        var useCase = new AutorizarNfceUseCase(repositorio);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(
                new AutorizarNfceRequest(Guid.NewGuid(), ChaveAcessoValida, "protocolo", "qrcode")));

        Assert.Equal("NFC-e não encontrada.", excecao.Message);
    }

    [Fact]
    public async Task Deve_propagar_validacao_de_dominio_da_chave_invalida()
    {
        var (repositorio, nfceId) = await CriarNfceAsync();
        var useCase = new AutorizarNfceUseCase(repositorio);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new AutorizarNfceRequest(nfceId, "123", "protocolo", "qrcode")));

        Assert.Equal("A chave de acesso da NFC-e deve conter 44 dígitos.", excecao.Message);
    }

    [Fact]
    public async Task Deve_impedir_autorizar_nfce_ja_autorizada()
    {
        var (repositorio, nfceId) = await CriarNfceAsync();
        var useCase = new AutorizarNfceUseCase(repositorio);
        await useCase.ExecutarAsync(
            new AutorizarNfceRequest(nfceId, ChaveAcessoValida, "135240000012345", "qrcode"));

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(
                new AutorizarNfceRequest(nfceId, ChaveAcessoValida, "outro-protocolo", "qrcode")));

        Assert.Equal("Somente uma NFC-e pendente pode ser autorizada.", excecao.Message);
    }
}
