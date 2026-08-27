using DotNetCleanArchitecture.Domain.Core.Entidades;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Core.ObjetosValor;

namespace DotNetCleanArchitecture.Domain.Entidades
{
    public class Fornecedor : Entidade
    {
        public string Nome { get; private set; } = string.Empty;
        public string? Fantasia {  get; private set; }
        public Documento Documento { get; private set; }
        public IndicadorIE IndicadorIE { get; private set; }
        public string? IE {  get; private set; }
        public string? IM { get; private set; }
        public string? Celular { get; private set; }
        public string? Fone { get; private set; }
        public string? Email { get; private set; }

        public Endereco Endereco { get; private set; }

        private Fornecedor()
        {
        }

        public Fornecedor(string nome,
                          string? fantasia,
                          Documento documento,
                          IndicadorIE indicadorIE,
                          string? ie,
                          string? im,
                          string? celular,
                          string? fone,
                          string? email,
                          Endereco endereco)
        {
            ValidarNome(nome);
            ValidarDocumento(documento);
            ValidarEndereco(endereco);
            ValidarIE(indicadorIE, ie);

            Nome = nome.Trim();
            Fantasia = fantasia?.Trim();
            Documento = documento;
            IndicadorIE = indicadorIE;
            IE = ie?.Trim();
            IM = im?.Trim();
            Fone = fone?.Trim();
            Celular = celular?.Trim();
            Email = email?.Trim();
            Endereco = endereco;
        }

        private static void ValidarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ExcecaoDeDominio(
                    "O nome do fornecedor deve ser informado.");
        }

        private static void ValidarDocumento(Documento documento)
        {
            if (documento is null)
                throw new ExcecaoDeDominio(
                    "O documento do fornecedor deve ser informado.");
        }

        private static void ValidarEndereco(Endereco endereco)
        {
            if (endereco is null)
                throw new ExcecaoDeDominio(
                    "O endereço do fornecedor deve ser informado.");
        }

        private static void ValidarIE(IndicadorIE indicadorIE, string? ie)
        {
            if (indicadorIE == IndicadorIE.Contribuinte &&
                string.IsNullOrWhiteSpace(ie))
            {
                throw new ExcecaoDeDominio(
                    "A inscrição estadual deve ser informada para contribuinte.");
            }

            if (indicadorIE != IndicadorIE.Contribuinte &&
                !string.IsNullOrWhiteSpace(ie))
            {
                throw new ExcecaoDeDominio(
                    "A inscrição estadual não deve ser informada para não contribuinte ou isento.");
            }
        }
    }
}
