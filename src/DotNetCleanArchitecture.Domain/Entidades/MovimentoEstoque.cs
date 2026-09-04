using DotNetCleanArchitecture.Domain.Core.Entidades;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;

namespace DotNetCleanArchitecture.Domain.Entidades
{
    public class MovimentoEstoque : Entidade
    {
        public Guid ProdutoId { get; private set; }
        public TipoMovimentoEstoque Tipo { get; private set; }
        public OrigemMovimentoEstoque Origem { get; private set; }
        public decimal Quantidade { get; private set; }
        public decimal SaldoAnterior { get; private set; }
        public decimal SaldoAtual { get; private set; }
        public DateTime DataMovimento { get; private set; }
        public Guid? DocumentoOrigemId { get; private set; }
        public string? Observacao { get; private set; }

        private MovimentoEstoque()
        {
        }

        public MovimentoEstoque(Guid produtoId,
                                TipoMovimentoEstoque tipo,
                                OrigemMovimentoEstoque origem,
                                decimal quantidade,
                                decimal saldoAnterior,
                                Guid? documentoOrigemId = null,
                                string? observacao = null)
        {
            quantidade = ArredondarQuantidade(quantidade);

            ValidarProdutoId(produtoId);
            ValidarTipo(tipo);
            ValidarOrigem(origem);
            ValidarQuantidade(quantidade);

            ProdutoId = produtoId;
            Tipo = tipo;
            Origem = origem;
            Quantidade = quantidade;
            SaldoAnterior = saldoAnterior;
            SaldoAtual = tipo == TipoMovimentoEstoque.Entrada
                ? saldoAnterior + quantidade
                : saldoAnterior - quantidade;
            DataMovimento = DateTime.Now;
            DocumentoOrigemId = documentoOrigemId;
            Observacao = observacao?.Trim();
        }

        private static void ValidarProdutoId(Guid produtoId)
        {
            if (produtoId == Guid.Empty)
                throw new ExcecaoDeDominio(
                    "O produto do movimento de estoque deve ser informado.");
        }

        private static void ValidarTipo(TipoMovimentoEstoque tipo)
        {
            if (!System.Enum.IsDefined(tipo))
                throw new ExcecaoDeDominio(
                    "O tipo do movimento de estoque informado é inválido.");
        }

        private static void ValidarOrigem(OrigemMovimentoEstoque origem)
        {
            if (!System.Enum.IsDefined(origem))
                throw new ExcecaoDeDominio(
                    "A origem do movimento de estoque informada é inválida.");
        }

        private static void ValidarQuantidade(decimal quantidade)
        {
            if (quantidade <= 0)
                throw new ExcecaoDeDominio(
                    "A quantidade do movimento de estoque deve ser maior que zero.");
        }

        private static decimal ArredondarQuantidade(decimal quantidade)
            => Math.Round(quantidade, 4, MidpointRounding.AwayFromZero);
    }
}
