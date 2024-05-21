using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.PushNotifications.Functions.StartupExtensions
{
    [ExcludeFromCodeCoverage]
    public static class ConfigurationExtensions
    {
        public static IConfiguration BuildAppConfiguration(this IConfigurationBuilder configBuilder, IConfiguration configuration)
        {
            configBuilder
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddEnvironmentVariables();
            configBuilder.AddJsonFile("local.settings.json", optional: true);

            var config = configBuilder.Build();

            configBuilder.AddAzureTableStorage(options =>
            {
                
                options.ConfigurationKeys = ["SFA.DAS.PushNotifications.Functions"];
                options.StorageConnectionString = config["ConfigurationStorageConnectionString"];
                options.EnvironmentName = config["EnvironmentName"];
                options.PreFixConfigurationKeys = false;
            });
            return configBuilder.Build();
        }
    }
}
