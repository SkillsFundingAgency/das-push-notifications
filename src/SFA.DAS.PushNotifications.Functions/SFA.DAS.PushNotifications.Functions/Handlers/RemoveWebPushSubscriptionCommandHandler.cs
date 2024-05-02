using Microsoft.Extensions.Logging;
using SFA.DAS.PushNotifications.Data.Entities;
using SFA.DAS.PushNotifications.Messages.Commands;
using SFA.DAS.PushNotifications.Services;

namespace SFA.DAS.PushNotifications.Functions.Handlers
{
    public class RemoveWebPushSubscriptionCommandHandler : IHandleMessages<RemoveWebPushSubscriptionCommand>
    {
        private readonly IPushNotificationsService _pushNotificationsService;
        private readonly ILogger<RemoveWebPushSubscriptionCommandHandler> _logger;

        public RemoveWebPushSubscriptionCommandHandler(IPushNotificationsService pushNotificationsService, ILogger<RemoveWebPushSubscriptionCommandHandler> logger)
        {
            _pushNotificationsService = pushNotificationsService;
            _logger = logger;
        }

        public async Task Handle(RemoveWebPushSubscriptionCommand message, IMessageHandlerContext context)
        {
            _logger.LogInformation($"Handling {nameof(RemoveWebPushSubscriptionCommandHandler)} for {message.Endpoint}");

            try
            {
                var applicationClient = new ApplicationClient
                {
                    UserAccountId = message.ApprenticeId,
                    Endpoint = message.Endpoint
                };

                await _pushNotificationsService.RemoveWebPushNotificationSubscription(applicationClient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error handling ApprenticeSubscriptionDeleteEvent for {message.Endpoint}");
            }
        }
    }
}