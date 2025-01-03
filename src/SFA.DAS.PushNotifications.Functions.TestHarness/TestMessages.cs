﻿using NServiceBus.Logging;
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
               
                Console.WriteLine("Press 'A' for AddWebPushSubscription");
                Console.WriteLine("Press 'B' for RemoveWebSubscription");
                Console.WriteLine("Press 'C' for SendPushNotification");
                Console.WriteLine("Press 'Q' to quit");
                var key = Console.ReadKey();
                Console.WriteLine();

                switch (key.Key)
                {
                    case ConsoleKey.A:
                        var testAdd1 = new AddWebPushSubscriptionCommand()
                        {
                            ApprenticeId = new Guid("43d6900f-ff56-480f-8c61-f17337e89fba"),
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

                    case ConsoleKey.B:
                        var testRemove1 = new RemoveWebPushSubscriptionCommand()
                        {
                            ApprenticeId = Guid.NewGuid(),
                            Endpoint = "https://myendpoint/test"
                        };
                        log.Info($"Adding RemoveSubscription command, ApprenticeId = {testRemove1.ApprenticeId}");
                        await endpointInstance.Send(testRemove1);
                        break;

                    case ConsoleKey.C:
                        var testSend1 = new SendPushNotificationCommand()
                        {
                            ApprenticeAccountIdentifier = new Guid("43d6900f-ff56-480f-8c61-f17337e89fba"),
                            Title = "Test Notification",
                            Body = "This is a test notification"
                        };
                        log.Info($"Adding SendPushNotification command, ApprenticeId = {testSend1.ApprenticeAccountIdentifier}");
                        await endpointInstance.Send(testSend1);
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