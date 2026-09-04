using System.Text.RegularExpressions;
using DotNetCleanArchitecture.Domain.Core.Entidades;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;

namespace DotNetCleanArchitecture.Domain.Entidades
{
    public class Nfce : Entidade
    {
        public Guid VendaId { get; private set; }
        public int Numero { get; private set; }
        public int Serie { get; private set; }
        public AmbienteEmissao Ambiente { get; private set; }
        public TipoEmissaoNfce TipoEmissao { get; private set; }
        public StatusNfce Status { get; private set; }
        public DateTime DataEmissao { get; private set; }

        public string? ChaveAcesso { get; private set; }
        public string? ProtocoloAutorizacao { get; private set; }
        public DateTime? DataAutorizacao { get; private set; }
        public string? QrCode { get; private set; }
        public string? MotivoStatus { get; private set; }

        public string? ProtocoloCancelamento { get; private set; }
        public string? JustificativaCancelamento { get; private set; }
        public DateTime? DataCancelamento { get; private set; }

        private Nfce()
        {
        }

        public Nfce(Guid vendaId,
                   int numero,
                   int serie,
                   AmbienteEmissao ambiente,
                   TipoEmissaoNfce tipoEmissao = TipoEmissaoNfce.Normal)
        {
            ValidarVendaId(vendaId);
            ValidarNumero(numero);
            ValidarSerie(serie);
            ValidarAmbiente(ambiente);
            ValidarTipoEmissao(tipoEmissao);

            VendaId = vendaId;
            Numero = numero;
            Serie = serie;
            Ambiente = ambiente;
            TipoEmissao = tipoEmissao;
            Status = StatusNfce.Pendente;
            DataEmissao = DateTime.Now;
        }

        public void Autorizar(string chaveAcesso, string protocoloAutorizacao, string qrCode)
        {
            ValidarStatusPendente("autorizada");

            chaveAcesso = ValidarChaveAcesso(chaveAcesso);
            ValidarTextoObrigatorio(protocoloAutorizacao, "O protocolo de autorização");
            ValidarTextoObrigatorio(qrCode, "O QR Code");

            ChaveAcesso = chaveAcesso;
            ProtocoloAutorizacao = protocoloAutorizacao.Trim();
            QrCode = qrCode.Trim();
            DataAutorizacao = DateTime.Now;
            Status = StatusNfce.Autorizada;
        }

        public void Rejeitar(string motivo)
        {
            ValidarStatusPendente("rejeitada");
            ValidarTextoObrigatorio(motivo, "O motivo da rejeição");

            MotivoStatus = motivo.Trim();
            Status = StatusNfce.Rejeitada;
        }

        public void Denegar(string motivo)
        {
            ValidarStatusPendente("denegada");
            ValidarTextoObrigatorio(motivo, "O motivo da denegação");

            MotivoStatus = motivo.Trim();
            Status = StatusNfce.Denegada;
        }

        public void Cancelar(string protocoloCancelamento, string justificativa)
        {
            if (Status != StatusNfce.Autorizada)
                throw new ExcecaoDeDominio(
                    "Somente uma NFC-e autorizada pode ser cancelada.");

            ValidarTextoObrigatorio(protocoloCancelamento, "O protocolo de cancelamento");
            ValidarJustificativaCancelamento(justificativa);

            ProtocoloCancelamento = protocoloCancelamento.Trim();
            JustificativaCancelamento = justificativa.Trim();
            DataCancelamento = DateTime.Now;
            Status = StatusNfce.Cancelada;
        }

        private static void ValidarVendaId(Guid vendaId)
        {
            if (vendaId == Guid.Empty)
                throw new ExcecaoDeDominio(
                    "A venda da NFC-e deve ser informada.");
        }

        private static void ValidarNumero(int numero)
        {
            if (numero <= 0)
                throw new ExcecaoDeDominio(
                    "O número da NFC-e deve ser informado.");
        }

        private static void ValidarSerie(int serie)
        {
            if (serie <= 0)
                throw new ExcecaoDeDominio(
                    "A série da NFC-e deve ser informada.");
        }

        private static void ValidarAmbiente(AmbienteEmissao ambiente)
        {
            if (!System.Enum.IsDefined(ambiente))
                throw new ExcecaoDeDominio(
                    "O ambiente de emissão informado é inválido.");
        }

        private static void ValidarTipoEmissao(TipoEmissaoNfce tipoEmissao)
        {
            if (!System.Enum.IsDefined(tipoEmissao))
                throw new ExcecaoDeDominio(
                    "O tipo de emissão informado é inválido.");
        }

        private void ValidarStatusPendente(string acao)
        {
            if (Status != StatusNfce.Pendente)
                throw new ExcecaoDeDominio(
                    $"Somente uma NFC-e pendente pode ser {acao}.");
        }

        private static string ValidarChaveAcesso(string chaveAcesso)
        {
            if (string.IsNullOrWhiteSpace(chaveAcesso))
                throw new ExcecaoDeDominio(
                    "A chave de acesso da NFC-e deve ser informada.");

            chaveAcesso = SomenteNumeros(chaveAcesso);

            if (chaveAcesso.Length != 44)
                throw new ExcecaoDeDominio(
                    "A chave de acesso da NFC-e deve conter 44 dígitos.");

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
