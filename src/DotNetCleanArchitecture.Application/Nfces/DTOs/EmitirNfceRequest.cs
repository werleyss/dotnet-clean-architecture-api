using DotNetCleanArchitecture.Domain.Core.Enum;

namespace DotNetCleanArchitecture.Application.Nfces.DTOs
{
    public record EmitirNfceRequest(
        Guid VendaId,
        int Numero,
        int Serie,
        AmbienteEmissao Ambiente,
        TipoEmissao TipoEmissao = TipoEmissao.Normal);
}
