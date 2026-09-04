using DotNetCleanArchitecture.Domain.Core.Entidades;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;

namespace DotNetCleanArchitecture.Domain.Entidades
{
    public class VendaItem : Entidade
    {
        public Guid ProdutoId { get; private set; }
        public decimal Quantidade { get; private set; }
        public decimal ValorUnitario { get; private set; }
        public decimal ValorDesconto { get; private set; }
        public decimal ValorTotal { get; private set; }
        public CstIcms? CST { get; private set; }
        public Csosn? CSOSN { get; private set; }

        private VendaItem()
        {
        }

        internal VendaItem(Guid produtoId,
                           decimal quantidade,
                           decimal valorUnitario,
                           decimal valorDesconto,
                           CstIcms? cst,
                           Csosn? csosn)
        {
            quantidade = ArredondarQuantidade(quantidade);

            ValidarProdutoId(produtoId);
            ValidarQuantidade(quantidade);
            ValidarNaoNegativo(valorUnitario, "valor unitário");
            ValidarNaoNegativo(valorDesconto, "valor de desconto");
            ValidarSituacaoTributaria(cst, csosn);

            var valorBruto = quantidade * valorUnitario;
            ValidarDesconto(valorDesconto, valorBruto);

            ProdutoId = produtoId;
            Quantidade = quantidade;
            ValorUnitario = valorUnitario;
            ValorDesconto = valorDesconto;
            ValorTotal = valorBruto - valorDesconto;
            CST = cst;
            CSOSN = csosn;
        }

        private static void ValidarProdutoId(Guid produtoId)
        {
            if (produtoId == Guid.Empty)
                throw new ExcecaoDeDominio(
                    "O produto do item deve ser informado.");
        }

        private static decimal ArredondarQuantidade(decimal quantidade)
            => Math.Round(quantidade, 4, MidpointRounding.AwayFromZero);

        private static void ValidarQuantidade(decimal quantidade)
        {
            if (quantidade <= 0)
                throw new ExcecaoDeDominio(
                    "A quantidade do item deve ser maior que zero.");
        }

        private static void ValidarNaoNegativo(decimal valor, string campo)
        {
            if (valor < 0)
                throw new ExcecaoDeDominio(
                    $"O {campo} do item não pode ser negativo.");
        }

        private static void ValidarDesconto(decimal valorDesconto, decimal valorBruto)
        {
            if (valorDesconto > valorBruto)
                throw new ExcecaoDeDominio(
                    "O valor de desconto do item não pode ser maior que o valor bruto.");
        }

        private static void ValidarSituacaoTributaria(CstIcms? cst, Csosn? csosn)
        {
            if (cst.HasValue && csosn.HasValue)
                throw new ExcecaoDeDominio(
                    "Informe o CST ou o CSOSN do item, não ambos.");

            if (cst.HasValue && !System.Enum.IsDefined(cst.Value))
                throw new ExcecaoDeDominio("O CST informado é inválido.");

            if (csosn.HasValue && !System.Enum.IsDefined(csosn.Value))
                throw new ExcecaoDeDominio("O CSOSN informado é inválido.");
        }
    }
}
