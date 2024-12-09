using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus.Testing;
using NUnit.Framework.Internal;
using SFA.DAS.PushNotifications.Application.Services;
using SFA.DAS.PushNotifications.Functions.Handlers;
using SFA.DAS.PushNotifications.Messages.Commands;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PushNotifications.Functions.UnitTests.CommandHandlers
{
    [TestFixture]
    public class RemoveWebPushSubscriptionCommandHandlerTests
    {
        private RemoveWebPushSubscriptionCommandHandler _handler;
        private Mock<IPushNotificationsService> _service;
        private Mock<ILogger<RemoveWebPushSubscriptionCommandHandler>> _logger;
        private TestableMessageHandlerContext _context;

        [SetUp]
        public void Setup()
        {
            _service = new Mock<IPushNotificationsService>();
            _logger = new Mock<ILogger<RemoveWebPushSubscriptionCommandHandler>>();
            _handler = new RemoveWebPushSubscriptionCommandHandler(_service.Object, _logger.Object);
            _context = new TestableMessageHandlerContext();
        }

        [Test, MoqAutoData]
        public async Task Run_Invokes_RemoveWebPushSubscription_Command(
            RemoveWebPushSubscriptionCommand message)
        {
            await _handler.Handle(message, _context);
            _service.Verify(x => x.RemoveWebPushNotificationSubscription(message));
            _logger.Verify(x => x.Log(
               LogLevel.Information, It.IsAny<EventId>(),
               It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Handle RemoveWebPushSubscriptionCommand for")), null,
               (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
        }

        [Test]
        public async Task Run_Invokes_RemoveWebPushSubscription_Command_Handles_NullEvent()
        {
            await _handler.Handle(null, _context);
            _logger.Verify(x => x.Log(
                 LogLevel.Error, It.IsAny<EventId>(),
                 It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error handling RemoveWebPushSubscriptionCommand. Missing or incorrect message supplied.")), null,
                 (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                 Times.Once());
        }
    }
}