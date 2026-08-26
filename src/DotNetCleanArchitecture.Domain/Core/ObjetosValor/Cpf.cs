using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using System.Text.RegularExpressions;

namespace DotNetCleanArchitecture.Domain.Core.ObjetosValor
{
    public sealed class Cpf : Documento
    {
        public override TipoDocumento Tipo => TipoDocumento.CPF;

        private Cpf(string numero) : base(numero)
        {
        }

        public static Cpf Criar(string numero)
        {
            numero = SomenteNumeros(numero);

            if (!Validar(numero))
                throw new ExcecaoDeDominio("CPF inválido.");

            return new Cpf(numero);
        }

        private static string SomenteNumeros(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ExcecaoDeDominio("O CPF deve ser informado.");

            return Regex.Replace(valor, @"\D", "");
        }

        private static bool Validar(string numero) 
        {
            if (numero.Length != 11)
                return false;

            if (numero.Distinct().Count() == 1)
                return false;

            var soma = 0;

            for (var i = 0; i < 9; i++)
                soma += (numero[i] - '0') * (10 - i);

            var resto = soma % 11;
            var digito1 = resto < 2 ? 0 : 11 - resto;

            soma = 0;

            for (var i = 0; i < 10; i++)
                soma += (numero[i] - '0') * (11 - i);

            resto = soma % 11;
            var digito2 = resto < 2 ? 0 : 11 - resto;

            return numero[9] - '0' == digito1 &&
                   numero[10] - '0' == digito2;
        }
    }
}
