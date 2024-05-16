using SFA.DAS.PushNotifications.Messages.Commands;

namespace SFA.DAS.PushNotifications.Functions.TestMessages
{
    public class AddTestMessages
    {
        private readonly IMessageSession _publisher;

        public AddTestMessages(IMessageSession publisher)
        {
            _publisher = publisher;
        }

        public async Task Run()
        {

            var key = ConsoleKey.Escape;

            while (key != ConsoleKey.X)
            {
                Console.Clear();
                Console.WriteLine("Add Test Messages");
                Console.WriteLine("------------");
                Console.WriteLine("Press A to add messages");
                Console.WriteLine("Press X to Exit");
                key = Console.ReadKey().Key;

                try
                {
                    switch (key)
                    {
                        case ConsoleKey.A:
                            var testAdd1 = new AddWebPushSubscriptionCommand()
                            {
                                ApprenticeId = Guid.NewGuid(),
                                Endpoint = "a",
                                PublicKey = "b",
                                AuthenticationSecret = "c"
                            };
                            await _publisher.Publish(testAdd1);

                            var testAdd2 = new AddWebPushSubscriptionCommand()
                            {
                                ApprenticeId = Guid.NewGuid(),
                                Endpoint = "x",
                                PublicKey = "b",
                                AuthenticationSecret = "c"
                            };
                            await _publisher.Publish(testAdd2);
                            Console.WriteLine($"Test Messages added to Push Notifications Queue");
                            break;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine();
                }

                if (key == ConsoleKey.X) break;

                Console.WriteLine();

            }
        }
    }
}