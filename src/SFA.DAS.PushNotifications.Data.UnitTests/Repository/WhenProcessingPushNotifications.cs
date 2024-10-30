using AutoFixture.NUnit3;
using Microsoft.Extensions.Logging;
using Moq;
using SFA.DAS.PushNotifications.Data.Repositories;
using SFA.DAS.PushNotifications.Data.UnitTests.DatabaseMock;
using SFA.DAS.PushNotifications.Messages.Commands;
using SFA.DAS.PushNotifications.Model.Entities;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PushNotifications.Data.UnitTests.Repository
{
    [TestFixture]
    public class WhenProcessingPushNotifications
    {
        [Test, MoqAutoData]
        public async Task Then_A_New_ClientNotification_IsAdded(
            List<ClientNotification> clientNotification,
            [Frozen] Mock<IPushNotificationsDataContext> mockContext,
            [Frozen] Mock<ILogger<ClientNotificationRepository>> logger,
            ClientNotificationRepository repository,
            SendPushNotificationCommand message)
        {
            //Arrange
            mockContext
                .Setup(context => context.ClientNotification)
                .ReturnsDbSet(clientNotification);

            CancellationToken cancellationToken = CancellationToken.None;
            int applicationClientId = 1;

            //Act
            await repository.AddClientNotification(applicationClientId, message, cancellationToken);

            //Assert
            mockContext.Verify(context => context.SaveChangesAsync(cancellationToken), Times.Once);
            logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Adding clientNotification for applicationClientId")), null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
            logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Added client notification for ")), null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));

        }
    }
}
