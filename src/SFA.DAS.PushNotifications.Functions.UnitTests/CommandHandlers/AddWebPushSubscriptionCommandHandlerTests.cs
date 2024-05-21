using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.PushNotifications.Application.Services;
using SFA.DAS.PushNotifications.Functions.Handlers;
using SFA.DAS.PushNotifications.Messages.Commands;

namespace SFA.DAS.PushNotifications.Functions.UnitTests.CommandHandlers
{
    [TestFixture]
    public class AppAddWebPushSubscriptionCommandHandlerTests
    {
        private AddWebPushSubscriptionCommandHandler _handler;
        private AddWebPushSubscriptionCommand _event;
        private Mock<IPushNotificationsService> _service;
        private Mock<ILogger<AddWebPushSubscriptionCommandHandler>> _logger;
        private readonly Fixture _fixture = new();

        [SetUp]
        public void Setup()
        {
            _service = new Mock<IPushNotificationsService>();
            _event = _fixture.Create<AddWebPushSubscriptionCommand>();
            _logger = new Mock<ILogger<AddWebPushSubscriptionCommandHandler>>();
            _handler = new AddWebPushSubscriptionCommandHandler(_service.Object, _logger.Object);
        }

        [Test]
        public async Task Run_Invokes_AddWebPushSubscription_Command()
        {
            await _handler.Handle(_event, Mock.Of<IMessageHandlerContext>());
            _service.Verify(x => x.AddWebPushNotificationSubscription(_event));
            _logger.Verify(x => x.Log(
               LogLevel.Debug, It.IsAny<EventId>(), 
               It.Is<It.IsAnyType>((v, t) => v.ToString().Contains(_event.Endpoint)), null, 
               (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()),
               Times.Once());
        }
    }
}