namespace DotNetCleanArchitecture.Application.Clientes.DTOs
{
    public record EnderecoRequest(
        string Logradouro,
        string Numero,
        string? Complemento,
        string Bairro,
        int CodigoIBGE,
        string Cidade,
        int CodigoUf,
        string Uf,
        string Cep,
        int CodigoPais,
        string Pais);
}
