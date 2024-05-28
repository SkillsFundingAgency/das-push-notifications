using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.PushNotifications.Application.Services;
using SFA.DAS.PushNotifications.Data.Repositories;
using SFA.DAS.PushNotifications.Messages.Commands;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PushNotifications.Application.UnitTests.Services
{
    [TestFixture]
    public class PushNotificationsServiceTests
    {
        private Mock<ILogger<PushNotificationsService>> _logger;
        private Mock<IApplicationClientRepository> _repository;
        private PushNotificationsService _service;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<PushNotificationsService>>();
            _repository = new Mock<IApplicationClientRepository>();
            _service = new PushNotificationsService(_repository.Object, _logger.Object);
        }


        [Test, MoqAutoData]
        public async Task Add_Web_Push_Notification_Subscription_Returns_Id(AddWebPushSubscriptionCommand message)
        {
            // Act
            var result = _service.AddWebPushNotificationSubscription(message);

            // Assert
            result.Id.Should().BeGreaterThan(0);
            _logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Adding subscription for ")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        [Test, MoqAutoData]
        public void Remove_Web_Push_Notification_Subscription(RemoveWebPushSubscriptionCommand message)
        {
            //Act & Assert
            Assert.DoesNotThrowAsync(async () => await _service.RemoveWebPushNotificationSubscription(message));
        }


        [Test, MoqAutoData]
        public void Add_Web_Push_Notification_Subscriptions_ThrowsException_OnEmptyEndpoint(AddWebPushSubscriptionCommand message)
        {
            //Arrange
            message.Endpoint = "";

            //Act & Assert
            Assert.ThrowsAsync<ArgumentException>(async () => await _service.AddWebPushNotificationSubscription(message));
            _logger.Verify(x => x.Log(
              LogLevel.Error, It.IsAny<EventId>(),
              It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Endpoint is null or empty")), null,
              (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
        }
    }
}