using AutoFixture.NUnit3;
using FluentAssertions;
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
           ApplicationClientRepository repository)
        {
            ApplicationClient applicationClient = applicationClients[0];
            applicationClient.Status = (int)ApplicationClientStatus.Active;
            mockContext
                .Setup(context => context.ApplicationClients)
                .ReturnsDbSet(applicationClients);

            CancellationToken cancellationToken = CancellationToken.None;
            await repository.RemoveWebPushNotificationSubscription(applicationClient, cancellationToken);

            mockContext.Verify(context => context.SaveChangesAsync(cancellationToken), Times.Once);
            applicationClient.Status.Should().Be((int)ApplicationClientStatus.Unsubscribed);
        }
    }
}