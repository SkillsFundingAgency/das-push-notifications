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
        private readonly Fixture _fixture = new();

        [SetUp]
        public void Setup()
        {
            _service = new Mock<IPushNotificationsService>();
            _event = _fixture.Create<RemoveWebPushSubscriptionCommand>();
            _handler = new RemoveWebPushSubscriptionCommandHandler(_service.Object, Mock.Of<ILogger<RemoveWebPushSubscriptionCommandHandler>>());
        }

        [Test]
        public async Task Run_Invokes_RemoveWebPushSubscription_Command()
        {
            await _handler.Handle(_event, Mock.Of<IMessageHandlerContext>());
            _service.Verify(x => x.RemoveWebPushNotificationSubscription(_event));
        }
    }
}