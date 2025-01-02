using AutoFixture.NUnit3;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
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
            [Frozen] Mock<IConfiguration> configuration,
        ClientNotificationRepository repository,
            SendPushNotificationCommand message)
        {
            //Arrange
            mockContext
                .Setup(context => context.ClientNotification)
                .ReturnsDbSet(clientNotification);

            CancellationToken cancellationToken = CancellationToken.None;
            int applicationClientId = 1;
            configuration.Setup(x => x["EnvironmentName"]).Returns("LOCAL");

            //Act
            await repository.AddClientNotification(applicationClientId, message, cancellationToken);

            //Assert
            mockContext.Verify(context => context.SaveChangesAsync(cancellationToken), Times.Once);
            logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Adding clientNotification for applicationClientId")), null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
            logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Added client notification for ")), null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));

        }

        [Test, MoqAutoData]
        public async Task Then_GetApplicationClients_ReturnsList(
            List<ApplicationClient> applicationClients,
         [Frozen] Mock<IPushNotificationsDataContext> mockContext,
         ApplicationClientRepository repository,
         ApplicationClient applicationClient)
        {
            //Arrange
            mockContext
                .Setup(context => context.ApplicationClients)
                .ReturnsDbSet(applicationClients);

            var applicationId = 1;
            var apprenticeId = Guid.NewGuid();

            applicationClients[0].Status = (int)ApplicationClientStatus.Active;
            applicationClients[0].ApplicationId = applicationId;
            applicationClients[0].UserAccountId = apprenticeId;

            //Act
            var result = await repository.GetApplicationClients(applicationId, apprenticeId);

            //Assert
            result.Count.Should().Be(1);
            result.All(x => x.UserAccountId == apprenticeId).Should().BeTrue();
        }

        [Test, MoqAutoData]
        public async Task Then_The_ClientNotification_Status_GetsUpdated(
            List<ClientNotification> clientNotifications,
            [Frozen] Mock<IPushNotificationsDataContext> mockContext,
            [Frozen] Mock<ILogger<ClientNotificationRepository>> logger,
            ClientNotificationRepository repository,
            ClientNotification clientNotification)
        {
            //Arrange
            mockContext
               .Setup(context => context.ClientNotification)
               .ReturnsDbSet(clientNotifications);
            clientNotification.Status = (int)ClientNotificationStatus.Success;
            CancellationToken cancellationToken = CancellationToken.None;

            //Act
            var result = await repository.UpdateClientNotification(clientNotification, cancellationToken);

            //Assert
            result.Status.Should().Be((int)ClientNotificationStatus.Success);
            mockContext.Verify(context => context.SaveChangesAsync(cancellationToken), Times.Once);
            logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Updating client notification for")), null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
            logger.Verify(x => x.Log(LogLevel.Information, It.IsAny<EventId>(), It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Updated client notification for")), null, (Func<It.IsAnyType, Exception, string>)It.IsAny<object>()));
        }

        [Test, MoqAutoData]
        public async Task Then_The_CorrectTasksUrl_IsReturned_ForLocal(
            List<ClientNotification> clientNotifications,
            [Frozen] Mock<IPushNotificationsDataContext> mockContext,
            [Frozen] Mock<ILogger<ClientNotificationRepository>> logger,
            [Frozen] Mock<IConfiguration> configuration,
            ClientNotificationRepository repository,
            ClientNotification clientNotification)
        {
            //Arrange
            mockContext
               .Setup(context => context.ClientNotification)
               .ReturnsDbSet(clientNotifications);
            clientNotification.Status = (int)ClientNotificationStatus.Success;
            CancellationToken cancellationToken = CancellationToken.None;
            configuration.Setup(x => x["EnvironmentName"]).Returns("LOCAL");

            //Act
            var result = repository.GetTasksUrl();

            //Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("local");
        }

        [Test, MoqAutoData]
        public async Task Then_The_CorrectTasksUrl_IsReturned_ForProd(
           List<ClientNotification> clientNotifications,
           [Frozen] Mock<IPushNotificationsDataContext> mockContext,
           [Frozen] Mock<ILogger<ClientNotificationRepository>> logger,
           [Frozen] Mock<IConfiguration> configuration,
           ClientNotificationRepository repository,
           ClientNotification clientNotification)
        {
            //Arrange
            mockContext
               .Setup(context => context.ClientNotification)
               .ReturnsDbSet(clientNotifications);
            clientNotification.Status = (int)ClientNotificationStatus.Success;
            CancellationToken cancellationToken = CancellationToken.None;
            configuration.Setup(x => x["EnvironmentName"]).Returns("PRD");

            //Act
            var result = repository.GetTasksUrl();

            //Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("my-apprenticeship.apprenticeships.education.gov.uk/Tasks");
        }

        [Test, MoqAutoData]
        public async Task Then_The_CorrectTasksUrl_IsReturned_ForNonProd(
           List<ClientNotification> clientNotifications,
           [Frozen] Mock<IPushNotificationsDataContext> mockContext,
           [Frozen] Mock<ILogger<ClientNotificationRepository>> logger,
           [Frozen] Mock<IConfiguration> configuration,
           ClientNotificationRepository repository,
           ClientNotification clientNotification)
        {
            //Arrange
            mockContext
               .Setup(context => context.ClientNotification)
               .ReturnsDbSet(clientNotifications);
            clientNotification.Status = (int)ClientNotificationStatus.Success;
            CancellationToken cancellationToken = CancellationToken.None;
            configuration.Setup(x => x["EnvironmentName"]).Returns("TEST");

            //Act
            var result = repository.GetTasksUrl();

            //Assert
            result.Should().NotBeNullOrEmpty();
            result.Should().Contain("test-apprentice-app.apprenticeships.education.gov.uk/Tasks");
        }
    }
}
