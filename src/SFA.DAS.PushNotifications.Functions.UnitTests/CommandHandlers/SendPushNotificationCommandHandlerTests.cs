using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus.Testing;
using SFA.DAS.PushNotifications.Application.Services;
using SFA.DAS.PushNotifications.Functions.Handlers;
using SFA.DAS.PushNotifications.Messages.Commands;

namespace SFA.DAS.PushNotifications.Functions.UnitTests.CommandHandlers
{
    [TestFixture]
    public class SendPushNotificationCommandHandlerTests
    {
        private Mock<IPushNotificationsService> _service;
        private SendPushNotificationCommand _event;
        private Mock<ILogger<SendPushNotificationCommandHandler>> _logger;
        private SendPushNotificationCommandHandler _handler;
        private TestableMessageHandlerContext _context;
        private readonly Fixture _fixture = new();

        [SetUp]
        public void Setup()
        {
            _service = new Mock<IPushNotificationsService>();
            _event = _fixture.Create<SendPushNotificationCommand>();
            _logger = new Mock<ILogger<SendPushNotificationCommandHandler>>();
            _handler = new SendPushNotificationCommandHandler(_service.Object, _logger.Object);
            _context = new TestableMessageHandlerContext();
        }

        [Test]
        public async Task Run_Invokes_SendPushNotification_Command()
        {
            //Act
            await _handler.Handle(_event, _context);

            //Assert
            _service.Verify(x => x.ProcessPushNotificationMessage(_event));
            _logger.Verify(x => x.Log(
               LogLevel.Information, It.IsAny<EventId>(),
               It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Handle SendPushNotificationCommand for {_event.ApprenticeAccountIdentifier}")), null,
               (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
               Times.Once());
        }

        [Test]
        public async Task Run_Invokes_SendPushNotification_Command_Handles_NullEvent()
        {
            //Act & Assert
            Assert.DoesNotThrowAsync(async () => await _handler.Handle(null, _context));
            _logger.Verify(x => x.Log(
                  LogLevel.Error, It.IsAny<EventId>(),
                  It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error handling SendPushNotificationCommand. Missing or incorrect message supplied.")), null,
                  (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                  Times.Once());
        }
    }
}
