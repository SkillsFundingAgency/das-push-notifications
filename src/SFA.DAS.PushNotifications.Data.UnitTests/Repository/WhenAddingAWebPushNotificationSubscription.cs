using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
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
            ApplicationClientRepository repository,
            ApplicationClient applicationClient)
        {
            mockContext
                .Setup(context => context.ApplicationClients)
                .ReturnsDbSet(applicationClients);
           
            CancellationToken cancellationToken = CancellationToken.None;
            await repository.AddWebPushNotificationSubscription(applicationClient, cancellationToken);

            mockContext.Verify(context => context.SaveChangesAsync(cancellationToken), Times.Once);
            applicationClient.Status.Should().Be((int)ApplicationClientStatus.Active);
        }

        [Test, RecursiveMoqAutoData]
        public async Task The_ApplicationClient_IsReplaced_IfAlreadyExists(
            [Frozen] Mock<IPushNotificationsDataContext> mockContext,
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
            await repository.AddWebPushNotificationSubscription(applicationClient2, cancellationToken);

            applicationClient2.Status.Should().Be((int)ApplicationClientStatus.Active);
            applicationClients.Count.Should().Be(1);
        }
    }
}