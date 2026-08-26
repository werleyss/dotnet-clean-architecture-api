namespace DotNetCleanArchitecture.Domain.Core.Excecoes
{
    public class ExcecaoDeDominio : Exception
    {
        public ExcecaoDeDominio(string mensagem) : base(mensagem)
        { 
        }
    }
}
