namespace DotNetCleanArchitecture.Application.Nfces.DTOs
{
    public record AutorizarNfceRequest(
        Guid NfceId,
        string ChaveAcesso,
        string ProtocoloAutorizacao,
        string QrCode);
}
