using DotNetCleanArchitecture.Domain.Core.Enum;

namespace DotNetCleanArchitecture.Application.Nfes.DTOs
{
    public record EmitirNfeRequest(
        Guid VendaId,
        int Numero,
        int Serie,
        AmbienteEmissao Ambiente,
        ModalidadeFrete ModalidadeFrete,
        string? InformacoesComplementares = null,
        TipoEmissao TipoEmissao = TipoEmissao.Normal);
}
