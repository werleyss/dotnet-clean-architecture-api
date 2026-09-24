using DotNetCleanArchitecture.Application.Nfces;
using DotNetCleanArchitecture.Application.Nfces.DTOs;
using DotNetCleanArchitecture.Application.Tests.Fakes;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Application.Tests.Nfces;

public class CancelarNfceUseCaseTests
{
    private const string ChaveAcessoValida = "35240112345678000190650010000010011234567890";

    private static async Task<(FakeNfceRepositorio repositorio, Guid nfceId)> CriarNfceAutorizadaAsync()
    {
        var repositorio = new FakeNfceRepositorio();
        var nfce = new Nfce(Guid.NewGuid(), 1001, 1, AmbienteEmissao.Homologacao);
        nfce.Autorizar(ChaveAcessoValida, "135240000012345", "qrcode");
        await repositorio.AdicionarAsync(nfce);
        return (repositorio, nfce.Id);
    }

    [Fact]
    public async Task Deve_cancelar_nfce_autorizada()
    {
        var (repositorio, nfceId) = await CriarNfceAutorizadaAsync();
        var useCase = new CancelarNfceUseCase(repositorio);

        var response = await useCase.ExecutarAsync(new CancelarNfceRequest(
            nfceId, "135250000098765", "Cancelamento solicitado pelo cliente no mesmo dia"));

        Assert.Equal("Cancelada", response.Status);
        Assert.Equal("135250000098765", response.ProtocoloCancelamento);
        Assert.Equal("Cancelamento solicitado pelo cliente no mesmo dia", response.JustificativaCancelamento);
    }

    [Fact]
    public async Task Deve_impedir_cancelar_nfce_inexistente()
    {
        var repositorio = new FakeNfceRepositorio();
        var useCase = new CancelarNfceUseCase(repositorio);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new CancelarNfceRequest(
                Guid.NewGuid(), "protocolo", "justificativa bem detalhada")));

        Assert.Equal("NFC-e não encontrada.", excecao.Message);
    }

    [Fact]
    public async Task Deve_impedir_cancelar_nfce_pendente()
    {
        var repositorio = new FakeNfceRepositorio();
        var nfce = new Nfce(Guid.NewGuid(), 1001, 1, AmbienteEmissao.Homologacao);
        await repositorio.AdicionarAsync(nfce);
        var useCase = new CancelarNfceUseCase(repositorio);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new CancelarNfceRequest(
                nfce.Id, "protocolo", "justificativa bem detalhada")));

        Assert.Equal("Somente uma NFC-e autorizada pode ser cancelada.", excecao.Message);
    }

    [Fact]
    public async Task Deve_propagar_validacao_de_dominio_da_justificativa_curta()
    {
        var (repositorio, nfceId) = await CriarNfceAutorizadaAsync();
        var useCase = new CancelarNfceUseCase(repositorio);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new CancelarNfceRequest(nfceId, "protocolo", "curta")));

        Assert.Equal(
            "A justificativa de cancelamento deve ter ao menos 15 caracteres.",
            excecao.Message);
    }
}
