using Microsoft.Extensions.Logging;
using SFA.DAS.PushNotifications.Data.Entities;
using SFA.DAS.PushNotifications.Messages.Commands;
using SFA.DAS.PushNotifications.Services;

namespace SFA.DAS.PushNotifications.Functions.Handlers
{
    public class AddWebPushSubscriptionCommandHandler : IHandleMessages<AddWebPushSubscriptionCommand>
    {
        private readonly IPushNotificationsService _pushNotificationsService;
        private readonly ILogger<AddWebPushSubscriptionCommandHandler> _logger;
        private const int ApprenticeAppApplicationId = 1;

        public AddWebPushSubscriptionCommandHandler(IPushNotificationsService pushNotificationsService, ILogger<AddWebPushSubscriptionCommandHandler> logger)
        {
            _pushNotificationsService = pushNotificationsService;
            _logger = logger;
        }
        public async Task Handle(AddWebPushSubscriptionCommand message, IMessageHandlerContext context)
        {
            _logger.LogInformation($"Handling {nameof(AddWebPushSubscriptionCommand)} for {message.Endpoint}");

            try
            {
                var applicationClient = new ApplicationClient
                {
                    ApplicationId = ApprenticeAppApplicationId,
                    UserAccountId = message.ApprenticeId,
                    Endpoint = message.Endpoint,
                    SubscriptionPublicKey = message.PublicKey,
                    SubscriptionAuthenticationSecret = message.AuthenticationSecret
                };

                await _pushNotificationsService.AddWebPushNotificationSubscription(applicationClient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error handling ApprenticeSubscriptionCreateEvent for {message.Endpoint}");
            }
        }
    }
}