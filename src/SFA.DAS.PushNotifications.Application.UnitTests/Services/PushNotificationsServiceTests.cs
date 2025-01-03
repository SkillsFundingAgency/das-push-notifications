﻿using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.PushNotifications.Application.Services;
using SFA.DAS.PushNotifications.Data.Repositories;
using SFA.DAS.PushNotifications.Messages.Commands;
using SFA.DAS.PushNotifications.Model.Entities;
using SFA.DAS.Testing.AutoFixture;
using WebPush;

namespace SFA.DAS.PushNotifications.Application.UnitTests.Services
{
    [TestFixture]
    public class PushNotificationsServiceTests
    {
        private Mock<ILogger<PushNotificationsService>> _logger;
        private Mock<IApplicationClientRepository> _applicationClientRepository;
        private Mock<IClientNotificationRepository> _clientNotificationRepository;
        private Mock<IConfiguration> _configuration;
        private PushNotificationsService _service;

        [SetUp]
        public void Setup()
        {
            _logger = new Mock<ILogger<PushNotificationsService>>();
            _applicationClientRepository = new Mock<IApplicationClientRepository>();
            _clientNotificationRepository = new Mock<IClientNotificationRepository>();
            _configuration = new Mock<IConfiguration>();
            _service = new PushNotificationsService(_applicationClientRepository.Object, _clientNotificationRepository.Object, _logger.Object, _configuration.Object);
        }

        [Test, MoqAutoData]
        public void Add_Web_Push_Notification_Subscription_Returns_Id(AddWebPushSubscriptionCommand message)
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

        [Test, MoqAutoData]
        public void Process_PushNotifications_WithNoAppClients_LogsMessage(SendPushNotificationCommand message)
        {
            // Act
            var result = _service.ProcessPushNotificationMessage(message);

            // Assert
            result.Id.Should().BeGreaterThan(0);
            _logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("No application clients stored for this apprentice account identifier")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        [Test, MoqAutoData]
        public void Process_PushNotifications_WithAppClients_SendsMessage(
            [Frozen] ClientNotification clientNotification,
            [Frozen] ApplicationClient applicationClient,
            [Frozen] SendPushNotificationCommand message)
        {
            //Arrange
            _applicationClientRepository.Setup(x => x.GetApplicationClients(It.IsAny<int>(), It.IsAny<Guid>()))
                .ReturnsAsync(new List<ApplicationClient> { applicationClient });
            _clientNotificationRepository.Setup(x => x.AddClientNotification(It.IsAny<int>(), It.IsAny<SendPushNotificationCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(clientNotification);

            // Act
            var result = _service.ProcessPushNotificationMessage(message);

            // Assert
            result.Id.Should().BeGreaterThan(0);
            _logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Information),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Sending notification for Client Notification Id")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        [Test, MoqAutoData]
        public void SendNotification_InvalidSubscriptionEncoding_ReturnsError(
           [Frozen] ClientNotification clientNotification,
           [Frozen] PushSubscription subscription
           )
        {
            //Arrange
            _configuration.Setup(c => c["SFA.DAS.PushNotifications.Functions:VapidPublicKey"]).Returns("testpublickey");
            _configuration.Setup(c => c["SFA.DAS.PushNotifications.Functions:VapidPrivateKey"]).Returns("testprivatekey");

            string payload = "{\"title\":\"Test\",\"message\":\"Test message\"}";
            subscription.Endpoint = "https://example.com/fcm/send/invalidendpoint";
            subscription.P256DH = "invalidp256dh";
            subscription.Auth = "invalidauth";

            // Act
            var result = _service.SendNotification(subscription, payload);

            // Assert
            result.Result.Should().Contain("The input is not a valid Base-64 string as it contains a non-base 64 character, more than two padding characters, or an illegal character among the padding characters.");
            _logger.Verify(
                x => x.Log(
                    It.Is<LogLevel>(l => l == LogLevel.Error),
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Error sending notification: ")),
                    It.IsAny<Exception>(),
                    It.Is<Func<It.IsAnyType, Exception, string>>((v, t) => true)));
        }

        [Test, MoqAutoData]
        public void SendNotification_NoVapidKeys_ReturnsError(
           [Frozen] ClientNotification clientNotification,
           [Frozen] PushSubscription subscription
           )
        {
            //Arrange
            string payload = "{\"title\":\"Test\",\"message\":\"Test message\"}";
            clientNotification.Status = (int)ClientNotificationStatus.Pending;

            // Act
            var result = _service.SendNotification(subscription, payload);

            // Assert
            result.Result.Should().Be("No Vapid Keys Found");
        }
    }
}