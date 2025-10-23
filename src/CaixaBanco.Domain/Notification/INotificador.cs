namespace CaixaBanco.Domain.Notification
{
    public interface INotificador
    {
        bool TemNotificacoes();

        List<Notificacao> ObterNotificacoes();

        void Disparar(Notificacao notification);
    }
}
