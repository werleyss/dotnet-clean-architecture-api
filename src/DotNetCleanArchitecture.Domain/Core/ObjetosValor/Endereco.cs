using DotNetCleanArchitecture.Domain.Core.Excecoes;

namespace DotNetCleanArchitecture.Domain.Core.ObjetosValor
{
    public sealed class Endereco
    {
        public string Logradouro { get; private set; }
        public string Numero { get; private set; }
        public string? Complemento { get; private set; }
        public string Bairro { get; private set; }

        public int CodigoIBGE { get; private set; }
        public string Cidade { get; private set; }

        public int CodigoUf { get; private set; }
        public string Uf { get; private set; }

        public string Cep { get; private set; }

        public int CodigoPais { get; private set; }
        public string Pais { get; private set; }

        private Endereco()
        {
        }

        private Endereco(
            string logradouro,
            string numero,
            string? complemento,
            string bairro,
            int codigoIBGE,
            string cidade,
            int codigoUf,
            string uf,
            string cep,
            int codigoPais,
            string pais)
        {
            Logradouro = logradouro;
            Numero = numero;
            Complemento = complemento;
            Bairro = bairro;
            CodigoIBGE = codigoIBGE;
            Cidade = cidade;
            CodigoUf = codigoUf;
            Uf = uf;
            Cep = cep;
            CodigoPais = codigoPais;
            Pais = pais;
        }

        public static Endereco Criar(
            string logradouro,
            string numero,
            string? complemento,
            string bairro,
            int codigoIBGE,
            string cidade,
            int codigoUf,
            string uf,
            string cep,
            int codigoPais,
            string pais)
        {
            if (string.IsNullOrWhiteSpace(logradouro))
                throw new ExcecaoDeDominio("O logradouro deve ser informado.");

            if (string.IsNullOrWhiteSpace(numero))
                throw new ExcecaoDeDominio("O número deve ser informado.");

            if (string.IsNullOrWhiteSpace(bairro))
                throw new ExcecaoDeDominio("O bairro deve ser informado.");

            if (codigoIBGE <= 0)
                throw new ExcecaoDeDominio("O código do IBGE deve ser informado.");

            if (string.IsNullOrWhiteSpace(cidade))
                throw new ExcecaoDeDominio("A cidade deve ser informada.");

            if (codigoUf <= 0)
                throw new ExcecaoDeDominio("O código da UF deve ser informado.");

            if (string.IsNullOrWhiteSpace(uf))
                throw new ExcecaoDeDominio("A UF deve ser informada.");

            if (string.IsNullOrWhiteSpace(cep))
                throw new ExcecaoDeDominio("O CEP deve ser informado.");

            if (codigoPais <= 0)
                throw new ExcecaoDeDominio("O código do país deve ser informado.");

            if (string.IsNullOrWhiteSpace(pais))
                throw new ExcecaoDeDominio("O país deve ser informado.");

            return new Endereco(
                logradouro.Trim(),
                numero.Trim(),
                complemento?.Trim(),
                bairro.Trim(),
                codigoIBGE,
                cidade.Trim(),
                codigoUf,
                uf.Trim().ToUpperInvariant(),
                cep.Trim(),
                codigoPais,
                pais.Trim());
        }
    }
}
