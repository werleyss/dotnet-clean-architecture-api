using DotNetCleanArchitecture.Domain.Core.Enum;

namespace DotNetCleanArchitecture.Domain.Core.ObjetosValor
{
    public abstract class Documento
    {
        public string Numero {  get; protected set; }
        public abstract TipoDocumento Tipo { get; }

        protected Documento(string numero)
        {
            Numero = numero;
        }
    }
}
