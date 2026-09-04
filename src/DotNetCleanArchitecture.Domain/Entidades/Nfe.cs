using System.Text.RegularExpressions;
using DotNetCleanArchitecture.Domain.Core.Entidades;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;

namespace DotNetCleanArchitecture.Domain.Entidades
{
    public class Nfe : Entidade
    {
        public Guid VendaId { get; private set; }
        public int Numero { get; private set; }
        public int Serie { get; private set; }
        public AmbienteEmissao Ambiente { get; private set; }
        public TipoEmissao TipoEmissao { get; private set; }
        public ModalidadeFrete ModalidadeFrete { get; private set; }
        public string? InformacoesComplementares { get; private set; }
        public StatusDocumentoFiscal Status { get; private set; }
        public DateTime DataEmissao { get; private set; }

        public string? ChaveAcesso { get; private set; }
        public string? ProtocoloAutorizacao { get; private set; }
        public DateTime? DataAutorizacao { get; private set; }
        public string? MotivoStatus { get; private set; }

        public string? ProtocoloCancelamento { get; private set; }
        public string? JustificativaCancelamento { get; private set; }
        public DateTime? DataCancelamento { get; private set; }

        private Nfe()
        {
        }

        public Nfe(Guid vendaId,
                  int numero,
                  int serie,
                  AmbienteEmissao ambiente,
                  ModalidadeFrete modalidadeFrete,
                  string? informacoesComplementares = null,
                  TipoEmissao tipoEmissao = TipoEmissao.Normal)
        {
            ValidarVendaId(vendaId);
            ValidarNumero(numero);
            ValidarSerie(serie);
            ValidarAmbiente(ambiente);
            ValidarTipoEmissao(tipoEmissao);
            ValidarModalidadeFrete(modalidadeFrete);

            VendaId = vendaId;
            Numero = numero;
            Serie = serie;
            Ambiente = ambiente;
            TipoEmissao = tipoEmissao;
            ModalidadeFrete = modalidadeFrete;
            InformacoesComplementares = informacoesComplementares?.Trim();
            Status = StatusDocumentoFiscal.Pendente;
            DataEmissao = DateTime.Now;
        }

        public void Autorizar(string chaveAcesso, string protocoloAutorizacao)
        {
            ValidarStatusPendente("autorizada");

            chaveAcesso = ValidarChaveAcesso(chaveAcesso);
            ValidarTextoObrigatorio(protocoloAutorizacao, "O protocolo de autorização");

            ChaveAcesso = chaveAcesso;
            ProtocoloAutorizacao = protocoloAutorizacao.Trim();
            DataAutorizacao = DateTime.Now;
            Status = StatusDocumentoFiscal.Autorizada;
        }

        public void Rejeitar(string motivo)
        {
            ValidarStatusPendente("rejeitada");
            ValidarTextoObrigatorio(motivo, "O motivo da rejeição");

            MotivoStatus = motivo.Trim();
            Status = StatusDocumentoFiscal.Rejeitada;
        }

        public void Denegar(string motivo)
        {
            ValidarStatusPendente("denegada");
            ValidarTextoObrigatorio(motivo, "O motivo da denegação");

            MotivoStatus = motivo.Trim();
            Status = StatusDocumentoFiscal.Denegada;
        }

        public void Cancelar(string protocoloCancelamento, string justificativa)
        {
            if (Status != StatusDocumentoFiscal.Autorizada)
                throw new ExcecaoDeDominio(
                    "Somente uma NF-e autorizada pode ser cancelada.");

            ValidarTextoObrigatorio(protocoloCancelamento, "O protocolo de cancelamento");
            ValidarJustificativaCancelamento(justificativa);

            ProtocoloCancelamento = protocoloCancelamento.Trim();
            JustificativaCancelamento = justificativa.Trim();
            DataCancelamento = DateTime.Now;
            Status = StatusDocumentoFiscal.Cancelada;
        }

        private static void ValidarVendaId(Guid vendaId)
        {
            if (vendaId == Guid.Empty)
                throw new ExcecaoDeDominio(
                    "A venda da NF-e deve ser informada.");
        }

        private static void ValidarNumero(int numero)
        {
            if (numero <= 0)
                throw new ExcecaoDeDominio(
                    "O número da NF-e deve ser informado.");
        }

        private static void ValidarSerie(int serie)
        {
            if (serie <= 0)
                throw new ExcecaoDeDominio(
                    "A série da NF-e deve ser informada.");
        }

        private static void ValidarAmbiente(AmbienteEmissao ambiente)
        {
            if (!System.Enum.IsDefined(ambiente))
                throw new ExcecaoDeDominio(
                    "O ambiente de emissão informado é inválido.");
        }

        private static void ValidarTipoEmissao(TipoEmissao tipoEmissao)
        {
            if (!System.Enum.IsDefined(tipoEmissao))
                throw new ExcecaoDeDominio(
                    "O tipo de emissão informado é inválido.");
        }

        private static void ValidarModalidadeFrete(ModalidadeFrete modalidadeFrete)
        {
            if (!System.Enum.IsDefined(modalidadeFrete))
                throw new ExcecaoDeDominio(
                    "A modalidade de frete informada é inválida.");
        }

        private void ValidarStatusPendente(string acao)
        {
            if (Status != StatusDocumentoFiscal.Pendente)
                throw new ExcecaoDeDominio(
                    $"Somente uma NF-e pendente pode ser {acao}.");
        }

        private static string ValidarChaveAcesso(string chaveAcesso)
        {
            if (string.IsNullOrWhiteSpace(chaveAcesso))
                throw new ExcecaoDeDominio(
                    "A chave de acesso da NF-e deve ser informada.");

            chaveAcesso = SomenteNumeros(chaveAcesso);

            if (chaveAcesso.Length != 44)
                throw new ExcecaoDeDominio(
                    "A chave de acesso da NF-e deve conter 44 dígitos.");

            return chaveAcesso;
        }

        private static void ValidarTextoObrigatorio(string valor, string campo)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ExcecaoDeDominio($"{campo} deve ser informado.");
        }

        private static void ValidarJustificativaCancelamento(string justificativa)
        {
            if (string.IsNullOrWhiteSpace(justificativa) || justificativa.Trim().Length < 15)
                throw new ExcecaoDeDominio(
                    "A justificativa de cancelamento deve ter ao menos 15 caracteres.");
        }

        private static string SomenteNumeros(string valor)
            => Regex.Replace(valor, @"\D", "");
    }
}
