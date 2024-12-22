using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.PushNotifications.Messages.Commands;
using System.Diagnostics.CodeAnalysis;

[assembly: ExcludeFromCodeCoverage]
namespace SFA.DAS.PushNotifications.Functions.TestHarness;


internal static class Program
{
    private const string EndpointName = "SFA.DAS.PushNotifications";
    private const string ConfigName = "SFA.DAS.PushNotifications.Functions";
    

    public static async Task Main()
    {
        var builder = new ConfigurationBuilder()
            .AddAzureTableStorage(ConfigName)
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables()
            .AddJsonFile("local.settings.json", optional: true);

        var config = builder.Build();

        var endpointConfiguration = new EndpointConfiguration(EndpointName);
        endpointConfiguration.SendFailedMessagesTo($"{EndpointName}-errors");
        endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();

        endpointConfiguration.Conventions()
            .DefiningCommandsAs(t => t.Namespace != null && t.Namespace.EndsWith("Commands"))
            .DefiningEventsAs(t => t.Namespace != null && t.Namespace.EndsWith("Events"));
        endpointConfiguration.SendOnly();

        var transport = endpointConfiguration.UseTransport<AzureServiceBusTransport>();
        transport.Routing().RouteToEndpoint(typeof(SendPushNotificationCommand), EndpointName);
        var connectionString = config["Values:AzureWebJobsServiceBus"];
        transport.ConnectionString(connectionString);
        var routing = transport.Routing();

        routing.RouteToEndpoint(typeof(AddWebPushSubscriptionCommand), EndpointName);
        routing.RouteToEndpoint(typeof(RemoveWebPushSubscriptionCommand), EndpointName);
        routing.RouteToEndpoint(typeof(SendPushNotificationCommand), EndpointName);


        var endpointInstance = await Endpoint.Start(endpointConfiguration);

        await TestMessages.AddMessages(endpointInstance);

        await endpointInstance.Stop();
    }
    
}

