using SharedKernel.Enums;
using System.Text;

namespace SharedKernel.Notifications.Impl
{
    public class Notification : INotification
    {
        private readonly Queue<string> _messages = new();
        private NotificationType _notificationType = NotificationType.Expected;

        public NotificationType NotificationType => _notificationType;

        public void Add(string message, NotificationType notificationType = NotificationType.Expected)
        {
            _notificationType = notificationType;
            _messages.Enqueue(message);
        }

        public void Add(IEnumerable<string> messages)
        {
            foreach (var message in messages)
            {
                _messages.Enqueue(message);
            }
        }

        public bool Any() =>
            _messages.Count > 0;

        public IEnumerable<string> All() =>
            _messages.AsEnumerable();

        public string GetSummary() =>
            _messages.Aggregate(
                new StringBuilder(),
                (sb, message) => sb.AppendLine(message))
            .ToString();
    }
}
