namespace DotNetCleanArchitecture.Application.Nfes.DTOs
{
    public record CancelarNfeRequest(Guid NfeId, string ProtocoloCancelamento, string Justificativa);
}
