using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Application.Fornecedores.DTOs
{
    internal static class FornecedorMapper
    {
        public static FornecedorResponse ParaResponse(Fornecedor fornecedor)
            => new(
                fornecedor.Id,
                fornecedor.Nome,
                fornecedor.Fantasia,
                fornecedor.Documento.Tipo.ToString(),
                fornecedor.Documento.Numero,
                fornecedor.IndicadorIE.ToString(),
                fornecedor.IE,
                fornecedor.IM,
                fornecedor.Celular,
                fornecedor.Fone,
                fornecedor.Email,
                fornecedor.Endereco.Cidade,
                fornecedor.Endereco.Uf);
    }
}
