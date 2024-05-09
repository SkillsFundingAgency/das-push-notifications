using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.PushNotifications.Data.Entities;
using SFA.DAS.PushNotifications.Functions.Handlers;
using SFA.DAS.PushNotifications.Messages.Commands;
using SFA.DAS.PushNotifications.Services;

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
            _service.Verify(x => x.AddWebPushNotificationSubscription(It.Is<ApplicationClient>(s =>
            s.UserAccountId == _event.ApprenticeId &&
            s.Endpoint == _event.Endpoint &&
            s.SubscriptionPublicKey == _event.PublicKey &&
            s.SubscriptionAuthenticationSecret == _event.AuthenticationSecret)));
        }
    }
}