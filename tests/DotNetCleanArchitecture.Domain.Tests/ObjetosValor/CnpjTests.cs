using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Core.ObjetosValor;

namespace DotNetCleanArchitecture.Domain.Tests;

public class CnpjTests
{
    [Fact]
    public void Deve_criar_cnpj_valido()
    {
        // Act
        var cnpj = Cnpj.Criar("00.000.000/E08G-12");

        // Assert
        Assert.NotNull(cnpj);
        Assert.Equal("00000000E08G12", cnpj.Numero);
    }

    [Fact]
    public void Deve_impedir_cnpj_invalido()
    {
        // Act
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            Cnpj.Criar("00.000.000/E08G-11"));

        // Assert
        Assert.Equal(
            "CNPJ inválido.",
            excecao.Message);
    }

    [Fact]
    public void Deve_impedir_cnpj_vazio()
    {
        // Act
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            Cnpj.Criar(""));

        // Assert
        Assert.Equal(
            "O CNPJ deve ser informado.",
            excecao.Message);
    }
}
