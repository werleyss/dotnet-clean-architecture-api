using DotNetCleanArchitecture.Domain.Core.Enum;
using DotNetCleanArchitecture.Domain.Core.Excecoes;
using System.Text.RegularExpressions;

namespace DotNetCleanArchitecture.Domain.Core.ObjetosValor;

public sealed class Cnpj : Documento
{
    public override TipoDocumento Tipo => TipoDocumento.CNPJ;
    private Cnpj(string numero) : base(numero)
    {
    }

    public static Cnpj Criar(string numero)
    {
        numero = Normalizar(numero);

        if (!Validar(numero))
            throw new ExcecaoDeDominio("CNPJ inválido.");

        return new Cnpj(numero);
    }

    private static string Normalizar(string valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
            throw new ExcecaoDeDominio(
                "O CNPJ deve ser informado.");

        return Regex.Replace(
            valor.ToUpperInvariant(),
            @"[^A-Z0-9]",
            "");
    }

    private static bool Validar(string numero)
    {
        if (numero.Length != 14)
            return false;

        if (!Regex.IsMatch(numero, @"^[A-Z0-9]{12}[0-9]{2}$"))
            return false;

        if (numero.Distinct().Count() == 1)
            return false;

        var digito1 = CalcularDigito(
            numero[..12],
            new[] { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 });

        var digito2 = CalcularDigito(
            numero[..12] + digito1,
            new[] { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 });

        return numero[12] - '0' == digito1 &&
               numero[13] - '0' == digito2;
    }

    private static int CalcularDigito(
        string valor,
        int[] multiplicadores)
    {
        var soma = 0;

        for (var i = 0; i < valor.Length; i++)
        {
            soma += ValorCaractere(valor[i]) * multiplicadores[i];
        }

        var resto = soma % 11;

        return resto < 2
            ? 0
            : 11 - resto;
    }

    private static int ValorCaractere(char caractere)
    {
        return caractere - 48;
    }
}