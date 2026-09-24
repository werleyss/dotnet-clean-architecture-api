using DotNetCleanArchitecture.Application.Nfes;
using DotNetCleanArchitecture.Application.Nfes.DTOs;
using DotNetCleanArchitecture.Application.Tests.Fakes;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Application.Tests.Nfes;

public class CancelarNfeUseCaseTests
{
    private const string ChaveAcessoValida = "35240112345678000190550010000010011234567890";

    private static async Task<(FakeNfeRepositorio repositorio, Guid nfeId)> CriarNfeAutorizadaAsync()
    {
        var repositorio = new FakeNfeRepositorio();
        var nfe = new Nfe(Guid.NewGuid(), 501, 1, AmbienteEmissao.Homologacao, ModalidadeFrete.CifRemetente);
        nfe.Autorizar(ChaveAcessoValida, "135240000012345");
        await repositorio.AdicionarAsync(nfe);
        return (repositorio, nfe.Id);
    }

    [Fact]
    public async Task Deve_cancelar_nfe_autorizada()
    {
        var (repositorio, nfeId) = await CriarNfeAutorizadaAsync();
        var useCase = new CancelarNfeUseCase(repositorio);

        var response = await useCase.ExecutarAsync(new CancelarNfeRequest(
            nfeId, "135250000098765", "Cancelamento solicitado pelo cliente no mesmo dia"));

        Assert.Equal("Cancelada", response.Status);
        Assert.Equal("135250000098765", response.ProtocoloCancelamento);
        Assert.Equal("Cancelamento solicitado pelo cliente no mesmo dia", response.JustificativaCancelamento);
    }

    [Fact]
    public async Task Deve_impedir_cancelar_nfe_inexistente()
    {
        var repositorio = new FakeNfeRepositorio();
        var useCase = new CancelarNfeUseCase(repositorio);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new CancelarNfeRequest(
                Guid.NewGuid(), "protocolo", "justificativa bem detalhada")));

        Assert.Equal("NF-e não encontrada.", excecao.Message);
    }

    [Fact]
    public async Task Deve_impedir_cancelar_nfe_pendente()
    {
        var repositorio = new FakeNfeRepositorio();
        var nfe = new Nfe(Guid.NewGuid(), 501, 1, AmbienteEmissao.Homologacao, ModalidadeFrete.CifRemetente);
        await repositorio.AdicionarAsync(nfe);
        var useCase = new CancelarNfeUseCase(repositorio);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new CancelarNfeRequest(
                nfe.Id, "protocolo", "justificativa bem detalhada")));

        Assert.Equal("Somente uma NF-e autorizada pode ser cancelada.", excecao.Message);
    }

    [Fact]
    public async Task Deve_propagar_validacao_de_dominio_da_justificativa_curta()
    {
        var (repositorio, nfeId) = await CriarNfeAutorizadaAsync();
        var useCase = new CancelarNfeUseCase(repositorio);

        var excecao = await Assert.ThrowsAsync<ExcecaoDeDominio>(() =>
            useCase.ExecutarAsync(new CancelarNfeRequest(nfeId, "protocolo", "curta")));

        Assert.Equal(
            "A justificativa de cancelamento deve ter ao menos 15 caracteres.",
            excecao.Message);
    }
}
