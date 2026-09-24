namespace DotNetCleanArchitecture.Application.Nfces.DTOs
{
    public record CancelarNfceRequest(Guid NfceId, string ProtocoloCancelamento, string Justificativa);
}
