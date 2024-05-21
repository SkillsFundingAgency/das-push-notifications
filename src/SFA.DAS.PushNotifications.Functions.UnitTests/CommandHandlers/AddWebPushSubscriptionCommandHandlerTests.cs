using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using NServiceBus;
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
        private readonly Fixture _fixture = new();

        [SetUp]
        public void Setup()
        {
            _service = new Mock<IPushNotificationsService>();
            _event = _fixture.Create<AddWebPushSubscriptionCommand>();
            _handler = new AddWebPushSubscriptionCommandHandler(_service.Object, Mock.Of<ILogger<AddWebPushSubscriptionCommandHandler>>());
        }

        [Test]
        public async Task Run_Invokes_AddWebPushSubscription_Command()
        {
            await _handler.Handle(_event, Mock.Of<IMessageHandlerContext>());
            _service.Verify(x => x.AddWebPushNotificationSubscription(_event));
        }
    }
}