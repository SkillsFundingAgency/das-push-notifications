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
        public async Task The_ApplicationClient_IsUpdated_IfAlreadyExists(
              List<ApplicationClient> applicationClients,
            [Frozen] Mock<IPushNotificationsDataContext> mockContext,
            ApplicationClientRepository repository)
        {
            ApplicationClient applicationClient = applicationClients[0];
            applicationClient.Status = (int)ApplicationClientStatus.Inactive;
            mockContext
                .Setup(context => context.ApplicationClients)
                .ReturnsDbSet(applicationClients);

            CancellationToken cancellationToken = CancellationToken.None;
            await repository.AddWebPushNotificationSubscription(applicationClient, cancellationToken);

            mockContext.Verify(context => context.SaveChangesAsync(cancellationToken), Times.Once);
            applicationClient.Status.Should().Be((int)ApplicationClientStatus.Active);
        }
    }
}
