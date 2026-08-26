namespace DotNetCleanArchitecture.Domain.Core.Entidades
{
    public abstract class Entidade 
    {
        public Guid Id { get; protected set; }
        public DateTime DtInclusao { get; protected set; }
        public Guid UsuarioInclusaoId { get; protected set; }
        public DateTime? DtUltimaAlteracao { get; protected set; }
        public Guid? UsuarioUltimaAlteracaoId { get; protected set; }

        protected Entidade()
        {
            Id = Guid.NewGuid();
            DtInclusao = DateTime.Now;
        }

        public void DefinirAuditoriaInclusao(Guid usuarioId)
        {
            UsuarioInclusaoId = usuarioId;
        }

        public void DefinirAuditoriaAlteracao(Guid usuarioId)
        {
            UsuarioUltimaAlteracaoId = usuarioId;
            DtUltimaAlteracao = DateTime.Now;
        }
    }
}
