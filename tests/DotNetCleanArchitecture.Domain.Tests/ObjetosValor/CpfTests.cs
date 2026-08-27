using DotNetCleanArchitecture.Domain.Core.Excecoes;
using DotNetCleanArchitecture.Domain.Core.ObjetosValor;

namespace DotNetCleanArchitecture.Domain.Tests.ObjetosValor;

public class CpfTests
{
    [Fact]
    public void Deve_criar_cpf_valido()
    {
        // Act
        var cpf = Cpf.Criar("529.982.247-25");

        // Assert
        Assert.NotNull(cpf);
        Assert.Equal("52998224725", cpf.Numero);
    }

    [Fact]
    public void Deve_impedir_cpf_invalido()
    {
        // Act
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            Cpf.Criar("529.982.247-26"));

        // Assert
        Assert.Equal(
            "CPF inválido.",
            excecao.Message);
    }

    [Fact]
    public void Deve_impedir_cpf_vazio()
    {
        // Act
        var excecao = Assert.Throws<ExcecaoDeDominio>(() =>
            Cpf.Criar(""));

        // Assert
        Assert.Equal(
            "O CPF deve ser informado.",
            excecao.Message);
    }
}
