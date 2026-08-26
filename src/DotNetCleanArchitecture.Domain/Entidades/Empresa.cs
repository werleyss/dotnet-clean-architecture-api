using DotNetCleanArchitecture.Domain.Core.Entidades;
using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Core.ObjetosValor;

namespace DotNetCleanArchitecture.Domain.Entidades
{
    public class Empresa : Entidade
    {
        public string Nome { get; private set; } = string.Empty;
        public string Fantasia { get; private set; } = string.Empty;
        public CRT CRT { get; private set; }
        public Documento Documento { get; private set; }
        public string? IE { get; private set; }
        public string? IEST { get; private set; }
        public string? IM { get; private set; }
        public string? CNAE { get; private set; }
        public string? Fone { get; private set; }
        public Endereco Endereco { get; private set; }

        private Empresa() 
        { 
        }

        public Empresa(string nome, 
                       string fantasia, 
                       CRT crt, 
                       Documento documento, 
                       string? ie, 
                       string? iest, 
                       string? im, 
                       string? cnae, 
                       string? fone, 
                       Endereco endereco)
        {
            ValidarNome(nome);
            ValidarFantasia(fantasia);
            ValidarDocumento(documento);
            ValidarEndereco(endereco);

            Nome = nome;
            Fantasia = fantasia;
            CRT = crt;
            Documento = documento;
            IE = ie;
            IEST = iest;
            IM = im;
            CNAE = cnae;
            Fone = fone;
            Endereco = endereco;
        }

        private static void ValidarNome(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome))
                throw new ExcecaoDeDominio(
                    "O nome da empresa deve ser informado.");
        }

        private static void ValidarFantasia(string fantasia)
        {
            if (string.IsNullOrWhiteSpace(fantasia))
                throw new ExcecaoDeDominio(
                    "O nome fantasia deve ser informado.");
        }

        private static void ValidarDocumento(Documento documento)
        {
            if (documento is null)
                throw new ExcecaoDeDominio("O documento da empresa deve ser informado.");
        }

        private static void ValidarEndereco(Endereco endereco)
        {
            if (endereco is null)
                throw new ExcecaoDeDominio("O endereço da empresa deve ser informado.");
        }
    }
}
