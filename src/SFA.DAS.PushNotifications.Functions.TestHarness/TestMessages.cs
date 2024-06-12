using NServiceBus.Logging;
using SFA.DAS.PushNotifications.Messages.Commands;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.PushNotifications.Functions
{
    [ExcludeFromCodeCoverage]
    public class TestMessages
    {
        static ILog log = LogManager.GetLogger<TestMessages>();
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
}