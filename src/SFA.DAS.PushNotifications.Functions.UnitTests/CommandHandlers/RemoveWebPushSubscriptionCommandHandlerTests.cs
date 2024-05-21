using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.PushNotifications.Application.Services;
using SFA.DAS.PushNotifications.Functions.Handlers;
using SFA.DAS.PushNotifications.Messages.Commands;

namespace SFA.DAS.PushNotifications.Functions.UnitTests.CommandHandlers
{
    [TestFixture]
    public class RemoveWebPushSubscriptionCommandHandlerTests
    {
        private RemoveWebPushSubscriptionCommandHandler _handler;
        private RemoveWebPushSubscriptionCommand _event;
        private Mock<IPushNotificationsService> _service;
        private Mock<ILogger<RemoveWebPushSubscriptionCommandHandler>> _logger;
        private readonly Fixture _fixture = new();

        [SetUp]
        public void Setup()
        {
            _service = new Mock<IPushNotificationsService>();
            _event = _fixture.Create<RemoveWebPushSubscriptionCommand>();
            _logger = new Mock<ILogger<RemoveWebPushSubscriptionCommandHandler>>();
            _handler = new RemoveWebPushSubscriptionCommandHandler(_service.Object, _logger.Object);
        }

        [Test]
        public async Task Run_Invokes_RemoveWebPushSubscription_Command()
        {
            await _handler.Handle(_event, Mock.Of<IMessageHandlerContext>());
            _service.Verify(x => x.RemoveWebPushNotificationSubscription(_event));
            _logger.Verify(x => x.Log(
                 LogLevel.Debug,
                 It.IsAny<EventId>(),
                 It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(_event.Endpoint)),
                 null,
                 (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
                 Times.Once);
        }
    }
}