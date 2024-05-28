using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework.Internal;
using SFA.DAS.PushNotifications.Data.Repositories;
using SFA.DAS.PushNotifications.Data.UnitTests.DatabaseMock;
using SFA.DAS.Testing.AutoFixture;
using static SFA.DAS.PushNotifications.Model.Entities.ApplicationClientStatusEnum;
using ApplicationClient = SFA.DAS.PushNotifications.Model.Entities.ApplicationClient;

namespace SFA.DAS.PushNotifications.Data.UnitTests.Repository
{
    [TestFixture]
    public class WhenAddingAWebPushNotificationSubscription
    {
        [Test, RecursiveMoqAutoData]
        public async Task Then_ApplicationClient_IsAdded(
         List<ApplicationClient> applicationClients,
         [Frozen] Mock<IPushNotificationsDataContext> mockContext,
         [Frozen] Mock<ILogger<ApplicationClientRepository>> logger,
         ApplicationClientRepository repository,
         ApplicationClient applicationClient)
        {
            //Arrange
            mockContext
                .Setup(context => context.ApplicationClients)
                .ReturnsDbSet(applicationClients);

            CancellationToken cancellationToken = CancellationToken.None;

            //Act
            await repository.AddWebPushNotificationSubscription(applicationClient, cancellationToken);

            //Assert
            mockContext.Verify(context => context.SaveChangesAsync(cancellationToken), Times.Once);
            applicationClient.Status.Should().Be((int)ApplicationClientStatus.Active);
            logger.Verify(x => x.Log(LogLevel.Information,It.IsAny<EventId>(),It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Adding push notification subscription for Endpoint")), null,(Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
            logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Added push notification subscription for Endpoint")), null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));

        }

        [Test, RecursiveMoqAutoData]
        public async Task The_ApplicationClient_IsReplaced_IfAlreadyExists(
            [Frozen] Mock<IPushNotificationsDataContext> mockContext,
            [Frozen] Mock<ILogger<ApplicationClientRepository>> logger,
            ApplicationClientRepository repository)
        {
            List<ApplicationClient> applicationClients = new();
            ApplicationClient applicationClient = new()
            {
                Id = 1,
                Endpoint = "testendpoint",
                SubscriptionAuthenticationSecret = "testsecret",
                SubscriptionPublicKey = "testkey"
            };
            mockContext
                  .Setup(context => context.ApplicationClients)
                  .ReturnsDbSet(applicationClients);
            applicationClients.Add(applicationClient);

            ApplicationClient applicationClient2 = new()
            {
                Id = 2,
                Endpoint = "testendpoint",
                SubscriptionAuthenticationSecret = "testsecret",
                SubscriptionPublicKey = "testkey"
            };
            CancellationToken cancellationToken = CancellationToken.None;

            //Act
            await repository.AddWebPushNotificationSubscription(applicationClient2, cancellationToken);

            //Assert
            applicationClient2.Status.Should().Be((int)ApplicationClientStatus.Active);
            applicationClients.Count.Should().Be(1);
            logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Adding push notification subscription for")), null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
        }
    }
}