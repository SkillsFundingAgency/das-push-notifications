using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.PushNotifications.Data.Repositories;
using SFA.DAS.PushNotifications.Data.UnitTests.DatabaseMock;
using SFA.DAS.PushNotifications.Model.Entities;
using SFA.DAS.Testing.AutoFixture;
using static SFA.DAS.PushNotifications.Model.Entities.ApplicationClientStatusEnum;

namespace SFA.DAS.PushNotifications.Data.UnitTests.Repository
{

    [TestFixture]
    public class WhenRemovingAWebPushNotificationSubscription
    {
        [Test, RecursiveMoqAutoData]
        public async Task The_ApplicationClient_IsUpdated_IfAlreadyExists(
             List<ApplicationClient> applicationClients,
           [Frozen] Mock<IPushNotificationsDataContext> mockContext,
           [Frozen] Mock<ILogger<ApplicationClientRepository>> logger,
           ApplicationClientRepository repository)
        {
            //Arrange
            ApplicationClient applicationClient = applicationClients[0];
            applicationClient.Status = (int)ApplicationClientStatus.Active;
            mockContext
                .Setup(context => context.ApplicationClients)
                .ReturnsDbSet(applicationClients);

            CancellationToken cancellationToken = CancellationToken.None;

            //Act
            await repository.RemoveWebPushNotificationSubscription(applicationClient, cancellationToken);

            //Assert
            mockContext.Verify(context => context.SaveChangesAsync(cancellationToken), Times.Once);
            applicationClient.Status.Should().Be((int)ApplicationClientStatus.Unsubscribed);
            logger.Verify(x => x.Log(LogLevel.Debug, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Removing push notification subscription for Endpoint")), null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
            logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Removed push notification subscription for Endpoint")), null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
        }

        [Test, RecursiveMoqAutoData]
        public async Task Then_NothingHappens_IfEndpointNotInDb(
          [Frozen] Mock<IPushNotificationsDataContext> mockContext,
          ApplicationClientRepository repository)
        {   
            //Arrange
            List<ApplicationClient> applicationClients = new();
            mockContext
                .Setup(context => context.ApplicationClients)
                .ReturnsDbSet(applicationClients);

            ApplicationClient applicationClient = new()
            {
                Id = 1,
                Endpoint = "testendpoint",
                SubscriptionAuthenticationSecret = "testsecret",
                SubscriptionPublicKey = "testkey"
            };
            CancellationToken cancellationToken = CancellationToken.None;

            //Act
            await repository.RemoveWebPushNotificationSubscription(applicationClient, cancellationToken);

            //Assert
            applicationClients.Count.Should().Be(0);
        }
    }
}