namespace CaixaBanco.Domain.Notification
{
    /// <summary>
    /// Classe responsável por gerenciar notificações de erros e mensagens
    /// </summary>
    public class Notificador : INotificador
    {
        private readonly List<Notificacao> _notificacoes;

        public Notificador()
        {
            _notificacoes = new List<Notificacao>();
        }

        public void Disparar(Notificacao notification)
        {
            _notificacoes.Add(notification);
        }

        public List<Notificacao> ObterNotificacoes()
        {
            return _notificacoes;
        }

        public bool TemNotificacoes()
        {
            return _notificacoes.Any();
        }
    }
}
