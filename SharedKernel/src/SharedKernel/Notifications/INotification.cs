using SharedKernel.Enums;

namespace SharedKernel.Notifications
{
    public interface INotification
    {
        public NotificationType NotificationType { get; }

        void Add(string message, NotificationType notificationType = NotificationType.Expected);

        void Add(IEnumerable<string> messages);

        bool Any();

        IEnumerable<string> All();

        string GetSummary();

    }
}
