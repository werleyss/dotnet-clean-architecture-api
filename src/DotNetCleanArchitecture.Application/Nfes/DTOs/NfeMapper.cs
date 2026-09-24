using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Application.Nfes.DTOs
{
    internal static class NfeMapper
    {
        public static NfeResponse ParaResponse(Nfe nfe)
            => new(
                nfe.Id,
                nfe.VendaId,
                nfe.Numero,
                nfe.Serie,
                nfe.Ambiente.ToString(),
                nfe.ModalidadeFrete.ToString(),
                nfe.InformacoesComplementares,
                nfe.TipoEmissao.ToString(),
                nfe.Status.ToString(),
                nfe.ChaveAcesso,
                nfe.ProtocoloAutorizacao,
                nfe.MotivoStatus,
                nfe.ProtocoloCancelamento,
                nfe.JustificativaCancelamento);
    }
}
