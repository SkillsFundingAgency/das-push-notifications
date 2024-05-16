using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.PushNotifications.Functions.Configuration;
using SFA.DAS.PushNotifications.Functions.TestMessages;
using SFA.DAS.PushNotifications.Messages.Commands;
using System.Net;

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

        var functionsConfig = configuration.GetSection("SFA.DAS.PushNotifications.Functions:SFA.DAS.PushNotifications.Functions").Get<PushNotificationsFunctions>();

        if (functionsConfig != null && functionsConfig.NServiceBusConnectionString == "UseLearningTransport=true")
        {
            var transport = endpointConfiguration.UseTransport<LearningTransport>();
            transport.StorageDirectory(
                Path.Combine(
                    Directory.GetCurrentDirectory()
                        .Substring(0, Directory.GetCurrentDirectory().IndexOf("src")),
                    @"src\.learningtransport"));

            transport.Routing().RouteToEndpoint(typeof(AddWebPushSubscriptionCommand), "SFA.DAS.PushNotifications.AddWebPushSubscription");
            transport.Routing().RouteToEndpoint(typeof(RemoveWebPushSubscriptionCommand), "SFA.DAS.PushNotifications.RemoveWebPushSubscription");

        }
        var endpointInstance = await Endpoint.Start(endpointConfiguration);

        var testHarness = new AddTestMessages(endpointInstance);

        await testHarness.Run();
        await endpointInstance.Stop();
    } 
}