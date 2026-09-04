using DotNetCleanArchitecture.Domain.Core.Entidades;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;

namespace DotNetCleanArchitecture.Domain.Entidades
{
    public class VendaPagamento : Entidade
    {
        public TipoPagamento FormaPagamento { get; private set; }
        public decimal Valor { get; private set; }
        public int QtdParcelas { get; private set; }

        private VendaPagamento()
        {
        }

        internal VendaPagamento(TipoPagamento formaPagamento,
                                decimal valor,
                                int qtdParcelas)
        {
            ValidarFormaPagamento(formaPagamento);
            ValidarValor(valor);
            ValidarQtdParcelas(qtdParcelas);

            FormaPagamento = formaPagamento;
            Valor = valor;
            QtdParcelas = qtdParcelas;
        }

        private static void ValidarFormaPagamento(TipoPagamento formaPagamento)
        {
            if (!System.Enum.IsDefined(formaPagamento))
                throw new ExcecaoDeDominio(
                    "A forma de pagamento informada é inválida.");
        }

        private static void ValidarValor(decimal valor)
        {
            if (valor <= 0)
                throw new ExcecaoDeDominio(
                    "O valor do pagamento deve ser maior que zero.");
        }

        private static void ValidarQtdParcelas(int qtdParcelas)
        {
            if (qtdParcelas <= 0)
                throw new ExcecaoDeDominio(
                    "A quantidade de parcelas deve ser maior que zero.");
        }
    }
}
