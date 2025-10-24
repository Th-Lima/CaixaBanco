namespace CaixaBanco.Domain.Notification
{
    /// <summary>
    /// Classe que representa uma notificação para erros ou mensagens de validação
    /// </summary>
    public class Notificacao
    {
        public string Mensagem { get; }

        public Notificacao(string message)
        {
            Mensagem = message;
        }
    }
}
