using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Application.Nfces.DTOs
{
    internal static class NfceMapper
    {
        public static NfceResponse ParaResponse(Nfce nfce)
            => new(
                nfce.Id,
                nfce.VendaId,
                nfce.Numero,
                nfce.Serie,
                nfce.Ambiente.ToString(),
                nfce.TipoEmissao.ToString(),
                nfce.Status.ToString(),
                nfce.ChaveAcesso,
                nfce.ProtocoloAutorizacao,
                nfce.QrCode,
                nfce.MotivoStatus,
                nfce.ProtocoloCancelamento,
                nfce.JustificativaCancelamento);
    }
}
