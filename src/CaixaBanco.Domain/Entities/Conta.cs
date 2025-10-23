using CaixaBanco.Domain.Enums;

namespace CaixaBanco.Domain.Entities
{
    public class Conta
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; } = null!;
        public string Documento { get; private set; } = null!;
        public decimal Saldo { get; private set; }
        public DateTime DataAbertura { get; private set; }
        public StatusConta Status { get; private set; }

        public ICollection<Transacao> Transacoes { get; private set; } = new List<Transacao>();

        private Conta() { }

        public Conta(string nome, string documento)
        {
            Id = Guid.NewGuid();
            Nome = nome ?? throw new ArgumentNullException(nameof(nome));
            Documento = documento ?? throw new ArgumentNullException(nameof(documento));
            Saldo = 1000m; // Saldo inicial
            DataAbertura = DateTime.Now;
            Status = StatusConta.Ativa;
        }

        public void Creditar(decimal valor)
        {
            if (valor <= 0) 
                throw new ArgumentException("Valor deve ser positivo", nameof(valor));

            Saldo += valor;
        }

        public void Debitar(decimal valor)
        {
            if (valor <= 0) 
                throw new ArgumentException("Valor deve ser positivo", nameof(valor));

            if (Saldo < valor) 
                throw new InvalidOperationException("Saldo insuficiente");

            Saldo -= valor;
        }

        public void Inativar() => Status = StatusConta.Inativa;
    }
}
