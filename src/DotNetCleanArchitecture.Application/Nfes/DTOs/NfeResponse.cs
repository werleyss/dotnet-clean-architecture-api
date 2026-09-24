namespace DotNetCleanArchitecture.Application.Nfes.DTOs
{
    public record NfeResponse(
        Guid Id,
        Guid VendaId,
        int Numero,
        int Serie,
        string Ambiente,
        string ModalidadeFrete,
        string? InformacoesComplementares,
        string TipoEmissao,
        string Status,
        string? ChaveAcesso,
        string? ProtocoloAutorizacao,
        string? MotivoStatus,
        string? ProtocoloCancelamento,
        string? JustificativaCancelamento);
}
