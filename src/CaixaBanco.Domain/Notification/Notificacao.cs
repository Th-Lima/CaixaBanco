namespace CaixaBanco.Domain.Notification
{
    public class Notificacao
    {
        public string Mensagem { get; }

        public Notificacao(string message)
        {
            Mensagem = message;
        }
    }
}
