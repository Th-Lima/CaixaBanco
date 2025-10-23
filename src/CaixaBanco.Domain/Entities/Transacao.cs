namespace CaixaBanco.Domain.Entities
{
    public class Transacao
    {
        public Guid Id { get; private set; }
        public Guid? ContaOrigemId { get; private set; }
        public Conta? ContaOrigem { get; private set; }
        public Guid? ContaDestinoId { get; private set; }
        public Conta? ContaDestino { get; private set; }
        public decimal Valor { get; private set; }
        public string? Descricao { get; private set; }
        public DateTime CriadoEm { get; private set; }

        private Transacao() { }

        public Transacao(Guid? contaOrigemId, Guid? contaDestinoId, decimal valor, string? descricao = null)
        {
            Id = Guid.NewGuid();
            ContaOrigemId = contaOrigemId;
            ContaDestinoId = contaDestinoId;
            Valor = valor;
            Descricao = descricao;
            CriadoEm = DateTime.UtcNow.Date;
        }
    }
}
