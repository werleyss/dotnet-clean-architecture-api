using System.Text.RegularExpressions;
using DotNetCleanArchitecture.Domain.Core.Entidades;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;

namespace DotNetCleanArchitecture.Domain.Entidades
{
    public class Venda : Entidade
    {
        public int Numero { get; private set; }
        public Guid EmpresaId { get; private set; }
        public Guid ClienteId { get; private set; }
        public DateTime DataEmissao { get; private set; }
        public string NaturezaOperacao { get; private set; } = string.Empty;
        public string CFOP { get; private set; } = string.Empty;
        public StatusVenda Status { get; private set; }
        public string? Observacoes { get; private set; }

        private readonly List<VendaItem> _itens = new();
        public IReadOnlyCollection<VendaItem> Itens => _itens.AsReadOnly();

        private readonly List<VendaPagamento> _pagamentos = new();
        public IReadOnlyCollection<VendaPagamento> Pagamentos => _pagamentos.AsReadOnly();

        public decimal ValorProdutos { get; private set; }
        public decimal ValorDesconto { get; private set; }
        public decimal ValorFrete { get; private set; }
        public decimal ValorOutrasDespesas { get; private set; }
        public decimal ValorTotal { get; private set; }

        private Venda()
        {
        }

        public Venda(int numero,
                    Guid empresaId,
                    Guid clienteId,
                    DateTime dataEmissao,
                    string naturezaOperacao,
                    string cfop,
                    string? observacoes)
        {
            ValidarNumero(numero);
            ValidarEmpresaId(empresaId);
            ValidarClienteId(clienteId);
            ValidarNaturezaOperacao(naturezaOperacao);
            cfop = ValidarCFOP(cfop);

            Numero = numero;
            EmpresaId = empresaId;
            ClienteId = clienteId;
            DataEmissao = dataEmissao;
            NaturezaOperacao = naturezaOperacao.Trim();
            CFOP = cfop;
            Observacoes = observacoes?.Trim();
            Status = StatusVenda.Orcamento;
        }

        public void AdicionarItem(Guid produtoId,
                                  decimal quantidade,
                                  decimal valorUnitario,
                                  decimal valorDesconto = 0,
                                  CstIcms? cst = null,
                                  Csosn? csosn = null)
        {
            ValidarEdicaoPermitida();

            var item = new VendaItem(produtoId, quantidade, valorUnitario, valorDesconto, cst, csosn);
            _itens.Add(item);
            RecalcularTotais();
        }

        public void RemoverItem(Guid itemId)
        {
            ValidarEdicaoPermitida();

            var item = _itens.FirstOrDefault(i => i.Id == itemId);

            if (item is null)
                throw new ExcecaoDeDominio("Item não encontrado na venda.");

            _itens.Remove(item);
            RecalcularTotais();
        }

        public void DefinirFrete(decimal valorFrete)
        {
            ValidarEdicaoPermitida();
            ValidarNaoNegativo(valorFrete, "valor do frete");

            ValorFrete = valorFrete;
            RecalcularTotais();
        }

        public void DefinirOutrasDespesas(decimal valorOutrasDespesas)
        {
            ValidarEdicaoPermitida();
            ValidarNaoNegativo(valorOutrasDespesas, "valor de outras despesas");

            ValorOutrasDespesas = valorOutrasDespesas;
            RecalcularTotais();
        }

        public void AdicionarPagamento(TipoPagamento formaPagamento, decimal valor, int qtdParcelas = 1)
        {
            ValidarEdicaoPermitida();

            var pagamento = new VendaPagamento(formaPagamento, valor, qtdParcelas);
            _pagamentos.Add(pagamento);
        }

        public void RemoverPagamento(Guid pagamentoId)
        {
            ValidarEdicaoPermitida();

            var pagamento = _pagamentos.FirstOrDefault(p => p.Id == pagamentoId);

            if (pagamento is null)
                throw new ExcecaoDeDominio("Pagamento não encontrado na venda.");

            _pagamentos.Remove(pagamento);
        }

        public void Faturar()
        {
            if (Status != StatusVenda.Orcamento)
                throw new ExcecaoDeDominio("Somente vendas em orçamento podem ser faturadas.");

            if (_itens.Count == 0)
                throw new ExcecaoDeDominio("A venda deve ter ao menos um item para ser faturada.");

            if (_pagamentos.Count == 0)
                throw new ExcecaoDeDominio("A venda deve ter ao menos um pagamento para ser faturada.");

            if (_pagamentos.Sum(p => p.Valor) != ValorTotal)
                throw new ExcecaoDeDominio("A soma dos pagamentos deve ser igual ao valor total da venda.");

            Status = StatusVenda.Faturada;
        }

        public void Cancelar()
        {
            if (Status == StatusVenda.Cancelada)
                throw new ExcecaoDeDominio("A venda já está cancelada.");

            Status = StatusVenda.Cancelada;
        }

        private void ValidarEdicaoPermitida()
        {
            if (Status != StatusVenda.Orcamento)
                throw new ExcecaoDeDominio("Não é possível alterar uma venda que não está em orçamento.");
        }

        private void RecalcularTotais()
        {
            ValorProdutos = _itens.Sum(i => i.Quantidade * i.ValorUnitario);
            ValorDesconto = _itens.Sum(i => i.ValorDesconto);
            ValorTotal = ValorProdutos - ValorDesconto + ValorFrete + ValorOutrasDespesas;
        }

        private static void ValidarNumero(int numero)
        {
            if (numero <= 0)
                throw new ExcecaoDeDominio("O número da venda deve ser informado.");
        }

        private static void ValidarEmpresaId(Guid empresaId)
        {
            if (empresaId == Guid.Empty)
                throw new ExcecaoDeDominio("A empresa da venda deve ser informada.");
        }

        private static void ValidarClienteId(Guid clienteId)
        {
            if (clienteId == Guid.Empty)
                throw new ExcecaoDeDominio("O cliente da venda deve ser informado.");
        }

        private static void ValidarNaturezaOperacao(string naturezaOperacao)
        {
            if (string.IsNullOrWhiteSpace(naturezaOperacao))
                throw new ExcecaoDeDominio("A natureza da operação deve ser informada.");
        }

        private static string ValidarCFOP(string cfop)
        {
            if (string.IsNullOrWhiteSpace(cfop))
                throw new ExcecaoDeDominio("O CFOP da venda deve ser informado.");

            cfop = SomenteNumeros(cfop);

            if (cfop.Length != 4)
                throw new ExcecaoDeDominio("O CFOP deve conter 4 dígitos.");

            if (cfop[0] is not ('5' or '6' or '7'))
                throw new ExcecaoDeDominio(
                    "O CFOP de uma venda deve iniciar com 5, 6 ou 7 (operação de saída).");

            return cfop;
        }

        private static void ValidarNaoNegativo(decimal valor, string campo)
        {
            if (valor < 0)
                throw new ExcecaoDeDominio($"O {campo} não pode ser negativo.");
        }

        private static string SomenteNumeros(string valor)
            => Regex.Replace(valor, @"\D", "");
    }
}
