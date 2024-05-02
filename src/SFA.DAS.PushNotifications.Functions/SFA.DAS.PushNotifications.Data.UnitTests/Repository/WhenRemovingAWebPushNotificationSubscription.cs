using AutoFixture.NUnit3;
using FluentAssertions;
using Moq;
using SFA.DAS.PushNotifications.Data.Entities;
using SFA.DAS.PushNotifications.Data.Repositories;
using SFA.DAS.PushNotifications.Data.UnitTests.DatabaseMock;
using SFA.DAS.Testing.AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static SFA.DAS.PushNotifications.Data.Entities.ApplicationClientStatusEnum;

namespace SFA.DAS.PushNotifications.Data.UnitTests.Repository
{
    [TestFixture]
    public  class WhenRemovingAWebPushNotificationSubscription
    {
        [Test, RecursiveMoqAutoData]
        public async Task Then_ApplicationClientStatus_IsUpdated(
             List<ApplicationClient> applicationClients,
            [Frozen] Mock<IPushNotificationsDataContext> mockContext,
            ApplicationClientRepository repository,
            ApplicationClient applicationClient)
        {
            applicationClient.Status = (int)ApplicationClientStatus.Inactive;

            mockContext
                .Setup(context => context.ApplicationClients)
                .ReturnsDbSet(applicationClients);

            CancellationToken cancellationToken = CancellationToken.None;
            await repository.RemoveWebPushNotificationSubscription(applicationClient, cancellationToken);

            mockContext.Verify(context => context.SaveChangesAsync(cancellationToken), Times.Once());
            applicationClient.Status.Should().Be((int)ApplicationClientStatus.Inactive);


        }
    }
}
