namespace CaixaBanco.Domain.Entities
{
    public class InativacaoConta
    {
        public Guid Id { get; private set; }
        public string Documento { get; private set; } = null!;
        public DateTime InativadoEm { get; private set; }
        public string UsuarioResponsavel { get; private set; } = null!;

        private InativacaoConta() { }

        public InativacaoConta(string documento, string usuarioResponsavel)
        {
            Id = Guid.NewGuid();
            Documento = documento ?? throw new ArgumentNullException(nameof(documento));
            UsuarioResponsavel = usuarioResponsavel ?? throw new ArgumentNullException(nameof(usuarioResponsavel));
            InativadoEm = DateTime.UtcNow;
        }
    }
}
