using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using System.Text.RegularExpressions;

namespace DotNetCleanArchitecture.Domain.Core.ObjetosValor
{
    public sealed class Cnpj : Documento
    {
        public override TipoDocumento Tipo => TipoDocumento.CNPJ;
        private Cnpj(string numero) : base(numero)
        {
        }

        public static Cnpj Criar(string numero)
        {
            numero = SomenteNumeros(numero);

            if (!Validar(numero))
                throw new ExcecaoDeDominio("CNPJ inválido.");

            return new Cnpj(numero);
        }

        private static string SomenteNumeros(string valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
                throw new ExcecaoDeDominio("O CNPJ deve ser informado.");

            return Regex.Replace(valor, @"\D", "");
        }

        private static bool Validar(string numero)
        {
            if(numero.Length != 14)
            return false;

            if (numero.Distinct().Count() == 1)
                return false;

            var tamanho = 12;
            var numeros = numero[..tamanho];
            var multiplicadores = new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            var soma = 0;

            for (var i = 0; i < tamanho; i++)
                soma += (numeros[i] - '0') * multiplicadores[i];

            var resto = soma % 11;
            var digito1 = resto < 2 ? 0 : 11 - resto;

            numeros += digito1;

            multiplicadores = new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

            soma = 0;

            for (var i = 0; i < 13; i++)
                soma += (numeros[i] - '0') * multiplicadores[i];

            resto = soma % 11;
            var digito2 = resto < 2 ? 0 : 11 - resto;

            return numero[12] - '0' == digito1 &&
                   numero[13] - '0' == digito2;
        }
    }
}
