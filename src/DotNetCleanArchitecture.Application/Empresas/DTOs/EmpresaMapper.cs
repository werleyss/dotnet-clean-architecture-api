using DotNetCleanArchitecture.Domain.Entidades;

namespace DotNetCleanArchitecture.Application.Empresas.DTOs
{
    internal static class EmpresaMapper
    {
        public static EmpresaResponse ParaResponse(Empresa empresa)
            => new(
                empresa.Id,
                empresa.Nome,
                empresa.Fantasia,
                empresa.CRT.ToString(),
                empresa.Documento.Tipo.ToString(),
                empresa.Documento.Numero,
                empresa.IE,
                empresa.IEST,
                empresa.IM,
                empresa.CNAE,
                empresa.Fone,
                empresa.Endereco.Cidade,
                empresa.Endereco.Uf);
    }
}
