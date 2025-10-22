namespace CaixaBanco.Domain.Notification
{
    public interface INotificador
    {
        bool HasNotification();

        List<Notificacao> GetNotifications();

        void Handle(Notificacao notification);
    }
}
