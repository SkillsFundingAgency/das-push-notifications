using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using SFA.DAS.PushNotifications.Data;
using SFA.DAS.PushNotifications.Data.Extensions;

namespace SFA.DAS.PushNotifications.Functions.UnitTests.StartupExtensions
{
    [TestFixture]
    public class WhenAddingTheDataContextToStartup
    {
        [Test]
        public void ThenTheDataContextIsAddedToTheServiceCollection()
        {
            //Arrange
            var mockConfiguration = new Mock<IConfiguration>();
            mockConfiguration.Setup(c => c["EnvironmentName"]).Returns("LOCAL");

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(mockConfiguration.Object);
            services.AddPushNotificationsDataContext(mockConfiguration.Object);

            var serviceProvider = services.BuildServiceProvider();

            //Act
            var dataContext = serviceProvider.GetService<IPushNotificationsDataContext>();

            //Assert
            Assert.IsNotNull(dataContext);
        }
    }
}