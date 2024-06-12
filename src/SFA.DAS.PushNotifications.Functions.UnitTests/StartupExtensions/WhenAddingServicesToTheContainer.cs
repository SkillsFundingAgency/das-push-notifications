using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.PushNotifications.Application.Services;
using SFA.DAS.PushNotifications.Data;
using SFA.DAS.PushNotifications.Data.Extensions;
using SFA.DAS.PushNotifications.Data.Repositories;

namespace SFA.DAS.PushNotifications.Functions.UnitTests.StartupExtensions
{
    public class WhenAddingServicesToTheContainer
    {
        [Test]
        [TestCase(typeof(IPushNotificationsDataContext))]
        [TestCase(typeof(IPushNotificationsService))]
        [TestCase(typeof(IApplicationClientRepository))]
        public void Then_The_Dependencies_Are_Correctly_Resolved(Type toResolve)
        {
            //Arrange
            ServiceCollection serviceCollection = new();
            SetupServiceCollection(serviceCollection);

            //Act
            var provider = serviceCollection.BuildServiceProvider();
            var type = provider.GetService(toResolve);

            //Assert
            Assert.That(type, Is.Not.Null);
        }

        private static void SetupServiceCollection(IServiceCollection serviceCollection)
        {
            
            var config = GenerateConfiguration();
           
            serviceCollection.AddSingleton<IConfiguration>(config);
            serviceCollection.AddLogging();
            serviceCollection.AddPushNotificationsDataContext(config);
            serviceCollection.AddTransient<IPushNotificationsDataContext, PushNotificationsDataContext>();
            serviceCollection.AddTransient<IPushNotificationsService, PushNotificationsService>();
            serviceCollection.AddTransient<IApplicationClientRepository, ApplicationClientRepository>();
        }

        private static IConfiguration GenerateConfiguration()
        {
            var configSource = new MemoryConfigurationSource();

            var provider = new MemoryConfigurationProvider(configSource);

            provider.Add("EnvironmentName", "LOCAL");
            return new ConfigurationRoot(new List<IConfigurationProvider> { provider });
        }
    }
}
