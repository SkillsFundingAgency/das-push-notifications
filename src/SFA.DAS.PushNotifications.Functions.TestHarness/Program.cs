using Microsoft.Extensions.Configuration;
using NServiceBus.Logging;
using NServiceBus.Routing.MessageDrivenSubscriptions;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.PushNotifications.Functions.Configuration;
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

        var functionsConfig = configuration.GetSection("SFA.DAS.PushNotifications.Functions:SFA.DAS.PushNotifications.Functions").Get<PushNotificationsFunctions>();

        if (functionsConfig != null && functionsConfig.NServiceBusConnectionString == "UseLearningTransport=true")
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

        await AddMessages(endpointInstance);

        await endpointInstance.Stop();
    }

    static ILog log = LogManager.GetLogger<Program>();
    public static async Task AddMessages(IEndpointInstance endpointInstance)
    {

        while (true)
        {
            log.Info("Press 'A' to add test messages, or 'Q' to quit.");
            var key = Console.ReadKey();
            Console.WriteLine();

            switch (key.Key)
            {
                case ConsoleKey.A:
                    var testAdd1 = new AddWebPushSubscriptionCommand()
                    {
                        ApprenticeId = Guid.NewGuid(),
                        Endpoint = "a",
                        PublicKey = "b",
                        AuthenticationSecret = "c"
                    };
                    log.Info($"Adding AddSubscription command, ApprenticeId = {testAdd1.ApprenticeId}");
                    await endpointInstance.Send(testAdd1);

                    var testAdd2 = new AddWebPushSubscriptionCommand()
                    {
                        ApprenticeId = Guid.NewGuid(),
                        Endpoint = "x",
                        PublicKey = "b",
                        AuthenticationSecret = "c"
                    };
                    log.Info($"Adding AddSubscription command, ApprenticeId = {testAdd2.ApprenticeId}");
                    await endpointInstance.Send(testAdd2);
                    break;

                case ConsoleKey.Q:
                    return;

                default:
                    log.Info("Unknown input. Please try again.");
                    break;
            }
        }

    }

}

