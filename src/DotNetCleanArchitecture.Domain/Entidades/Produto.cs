using System.Text.RegularExpressions;
using DotNetCleanArchitecture.Domain.Core.Entidades;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;

namespace DotNetCleanArchitecture.Domain.Entidades
{
    public class Produto : Entidade
    {
        public string Codigo { get; private set; } = string.Empty;
        public string Descricao { get; private set; } = string.Empty;
        public TipoProduto Tipo { get; private set; }
        public bool Ativo { get; private set; }

        public string NCM { get; private set; } = string.Empty;
        public string? CEST { get; private set; }
        public OrigemMercadoria OrigemMercadoria { get; private set; }
        public CstIcms? CST { get; private set; }
        public Csosn? CSOSN { get; private set; }
        public string? EAN { get; private set; }
        public string? EANTrib { get; private set; }
        public string UN { get; private set; } = "UN";

        public decimal VlrCusto { get; private set; }
        public decimal VlrVenda { get; private set; }

        public decimal EstoqueAtual { get; private set; }
        public decimal EstoqueMinimo { get; private set; }
        public decimal PesoLiquido { get; private set; }
        public decimal PesoBruto { get; private set; }
        public decimal Altura { get; private set; }
        public decimal Largura { get; private set; }
        public decimal Profundidade { get; private set; }

        public string? InfoAdicional { get; private set; }

        private Produto()
        {
        }

        public Produto(string codigo,
                       string descricao,
                       TipoProduto tipo,
                       string ncm,
                       string? cest,
                       OrigemMercadoria origemMercadoria,
                       CstIcms? cst,
                       Csosn? csosn,
                       string? ean,
                       string? eanTrib,
                       string un,
                       decimal vlrCusto,
                       decimal vlrVenda,
                       decimal estoqueAtual,
                       decimal estoqueMinimo,
                       decimal pesoLiquido,
                       decimal pesoBruto,
                       decimal altura,
                       decimal largura,
                       decimal profundidade,
                       string? infoAdicional,
                       bool ativo = true)
        {
            ValidarCodigo(codigo);
            ValidarDescricao(descricao);
            ncm = ValidarNCM(ncm);
            cest = ValidarCEST(cest);
            ValidarSituacaoTributaria(cst, csosn);
            ean = ValidarGTIN(ean, "EAN");
            eanTrib = ValidarGTIN(eanTrib, "EAN tributável");
            ValidarUN(un);
            ValidarNaoNegativo(vlrCusto, "valor de custo");
            ValidarNaoNegativo(vlrVenda, "valor de venda");
            ValidarNaoNegativo(estoqueMinimo, "estoque mínimo");
            ValidarNaoNegativo(pesoLiquido, "peso líquido");
            ValidarNaoNegativo(pesoBruto, "peso bruto");
            ValidarNaoNegativo(altura, "altura");
            ValidarNaoNegativo(largura, "largura");
            ValidarNaoNegativo(profundidade, "profundidade");
            ValidarPesoBruto(pesoBruto, pesoLiquido);

            Codigo = codigo.Trim();
            Descricao = descricao.Trim();
            Tipo = tipo;
            Ativo = ativo;
            NCM = ncm;
            CEST = cest;
            OrigemMercadoria = origemMercadoria;
            CST = cst;
            CSOSN = csosn;
            EAN = ean;
            EANTrib = eanTrib;
            UN = un.Trim().ToUpperInvariant();
            VlrCusto = vlrCusto;
            VlrVenda = vlrVenda;
            EstoqueAtual = estoqueAtual;
            EstoqueMinimo = estoqueMinimo;
            PesoLiquido = pesoLiquido;
            PesoBruto = pesoBruto;
            Altura = altura;
            Largura = largura;
            Profundidade = profundidade;
            InfoAdicional = infoAdicional?.Trim();
        }

        public void Ativar() => Ativo = true;

        public void Inativar() => Ativo = false;

        public void RegistrarEntradaEstoque(decimal quantidade)
        {
            quantidade = ArredondarQuantidade(quantidade);
            ValidarQuantidadeMovimento(quantidade);

            EstoqueAtual += quantidade;
        }

        public void RegistrarSaidaEstoque(decimal quantidade)
        {
            quantidade = ArredondarQuantidade(quantidade);
            ValidarQuantidadeMovimento(quantidade);

            EstoqueAtual -= quantidade;
        }

        private static void ValidarCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                throw new ExcecaoDeDominio(
                    "O código do produto deve ser informado.");
        }

        private static void ValidarDescricao(string descricao)
        {
            if (string.IsNullOrWhiteSpace(descricao))
                throw new ExcecaoDeDominio(
                    "A descrição do produto deve ser informada.");
        }

        private static string ValidarNCM(string ncm)
        {
            if (string.IsNullOrWhiteSpace(ncm))
                throw new ExcecaoDeDominio(
                    "O NCM do produto deve ser informado.");

            ncm = SomenteNumeros(ncm);

            if (ncm.Length != 8)
                throw new ExcecaoDeDominio(
                    "O NCM deve conter 8 dígitos.");

            return ncm;
        }

        private static string? ValidarCEST(string? cest)
        {
            if (string.IsNullOrWhiteSpace(cest))
                return null;

            cest = SomenteNumeros(cest);

            if (cest.Length != 7)
                throw new ExcecaoDeDominio(
                    "O CEST deve conter 7 dígitos.");

            return cest;
        }

        private static void ValidarSituacaoTributaria(CstIcms? cst, Csosn? csosn)
        {
            if (cst.HasValue && csosn.HasValue)
                throw new ExcecaoDeDominio(
                    "Informe o CST ou o CSOSN, não ambos.");

            if (cst.HasValue && !System.Enum.IsDefined(cst.Value))
                throw new ExcecaoDeDominio("O CST informado é inválido.");

            if (csosn.HasValue && !System.Enum.IsDefined(csosn.Value))
                throw new ExcecaoDeDominio("O CSOSN informado é inválido.");
        }

        private static string? ValidarGTIN(string? gtin, string campo)
        {
            if (string.IsNullOrWhiteSpace(gtin))
                return null;

            gtin = SomenteNumeros(gtin);

            if (gtin.Length is not (8 or 12 or 13 or 14))
                throw new ExcecaoDeDominio(
                    $"O {campo} deve conter 8, 12, 13 ou 14 dígitos.");

            return gtin;
        }

        private static void ValidarUN(string un)
        {
            if (string.IsNullOrWhiteSpace(un))
                throw new ExcecaoDeDominio(
                    "A unidade do produto deve ser informada.");
        }

        private static void ValidarNaoNegativo(decimal valor, string campo)
        {
            if (valor < 0)
                throw new ExcecaoDeDominio(
                    $"O {campo} do produto não pode ser negativo.");
        }

        private static void ValidarPesoBruto(decimal pesoBruto, decimal pesoLiquido)
        {
            if (pesoBruto < pesoLiquido)
                throw new ExcecaoDeDominio(
                    "O peso bruto não pode ser menor que o peso líquido.");
        }

        private static decimal ArredondarQuantidade(decimal quantidade)
            => Math.Round(quantidade, 4, MidpointRounding.AwayFromZero);

        private static void ValidarQuantidadeMovimento(decimal quantidade)
        {
            if (quantidade <= 0)
                throw new ExcecaoDeDominio(
                    "A quantidade do movimento de estoque deve ser maior que zero.");
        }

        private static string SomenteNumeros(string valor)
            => Regex.Replace(valor, @"\D", "");
    }
}
