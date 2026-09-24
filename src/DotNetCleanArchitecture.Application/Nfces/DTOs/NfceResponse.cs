namespace DotNetCleanArchitecture.Application.Nfces.DTOs
{
    public record NfceResponse(
        Guid Id,
        Guid VendaId,
        int Numero,
        int Serie,
        string Ambiente,
        string TipoEmissao,
        string Status,
        string? ChaveAcesso,
        string? ProtocoloAutorizacao,
        string? QrCode,
        string? MotivoStatus,
        string? ProtocoloCancelamento,
        string? JustificativaCancelamento);
}
