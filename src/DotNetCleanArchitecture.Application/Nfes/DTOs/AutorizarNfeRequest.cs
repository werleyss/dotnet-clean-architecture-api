namespace DotNetCleanArchitecture.Application.Nfes.DTOs
{
    public record AutorizarNfeRequest(Guid NfeId, string ChaveAcesso, string ProtocoloAutorizacao);
}
