using NUnit.Framework;
using SharedKernel.Enums;
using SharedKernel.Notifications.Impl;

namespace SharedKernel.Test.Notifications
{
    [TestFixture]
    public class NotificationTests
    {
        private Notification _notification;

        [SetUp]
        public void Setup()
        {
            _notification = new Notification();
        }

        [Test]
        public void Add_SingleMessage_UpdatesNotificationType()
        {
            // Arrange
            string message = "Test message";
            NotificationType type = NotificationType.Expected;

            // Act
            _notification.Add(message, type);

            // Assert
            Assert.That(_notification.NotificationType, Is.EqualTo(type));
        }

        [Test]
        public void Add_SingleMessage_AddsToMessagesQueue()
        {
            // Arrange
            string message = "Test message";

            // Act
            _notification.Add(message);

            // Assert
            var messages = _notification.All();
            Assert.That(messages, Is.Not.Empty);
            Assert.That(messages.First(), Is.EqualTo(message));
        }

        [Test]
        public void Add_MultipleMessages_AddsAllMessagesToQueue()
        {
            // Arrange
            var messages = new List<string> { "Message 1", "Message 2", "Message 3" };

            // Act
            _notification.Add(messages);

            // Assert
            var allMessages = _notification.All().ToList();
            Assert.That(allMessages, Has.Count.EqualTo(3));
            Assert.That(allMessages, Is.EqualTo(messages));
        }

        [Test]
        public void Any_WithMessages_ReturnsTrue()
        {
            // Arrange
            _notification.Add("Message");

            // Act & Assert
            Assert.That(_notification.Any(), Is.True);
        }

        [Test]
        public void Any_WithoutMessages_ReturnsFalse()
        {
            // Act & Assert
            Assert.That(_notification.Any(), Is.False);
        }

        [Test]
        public void All_WhenCalled_ReturnsAllMessages()
        {
            // Arrange
            _notification.Add("Message 1");
            _notification.Add("Message 2");

            // Act
            var messages = _notification.All().ToList();

            // Assert
            Assert.That(messages, Has.Count.EqualTo(2));
            Assert.That(messages[0], Is.EqualTo("Message 1"));
            Assert.That(messages[1], Is.EqualTo("Message 2"));
        }

        [Test]
        public void GetSummary_ReturnsConcatenatedMessages()
        {
            // Arrange
            _notification.Add("Message 1");
            _notification.Add("Message 2");

            // Act
            var summary = _notification.GetSummary();

            // Assert
            string expectedSummary = "Message 1\r\nMessage 2\r\n";
            Assert.That(summary, Is.EqualTo(expectedSummary));
        }
    }
}
