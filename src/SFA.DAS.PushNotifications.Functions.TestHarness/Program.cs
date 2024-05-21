using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.PushNotifications.Messages.Commands;

namespace SFA.DAS.PushNotifications.Functions.TestHarness;

internal class Program
{
    private const string EndpointName = "SFA.DAS.PushNotifications";
    private const string ConfigName = "SFA.DAS.PushNotifications.Functions";
    

    public static async Task Main()
    {
        var builder = new ConfigurationBuilder()
            .AddAzureTableStorage(ConfigName);

        var configuration = builder.Build();

        var endpointConfiguration = new EndpointConfiguration(EndpointName);
        endpointConfiguration.SendFailedMessagesTo($"{EndpointName}-errors");
        endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
        endpointConfiguration.SendOnly();

        if (configuration["EnvironmentName"] == "LOCAL")
        {
            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            transport.StorageDirectory(
                Path.Combine(
                    Directory.GetCurrentDirectory()
                        .Substring(0, Directory.GetCurrentDirectory().IndexOf("src")),
                    @"src\.learningtransport"));
            var routing = transport.Routing();

            routing.RouteToEndpoint(typeof(AddWebPushSubscriptionCommand), "SFA.DAS.PushNotifications.AddWebPushSubscription");
            routing.RouteToEndpoint(typeof(RemoveWebPushSubscriptionCommand), "SFA.DAS.PushNotifications.RemoveWebPushSubscription");

        }

        var endpointInstance = await Endpoint.Start(endpointConfiguration);

        await TestMessages.AddMessages(endpointInstance);

        await endpointInstance.Stop();
    }
    
}

