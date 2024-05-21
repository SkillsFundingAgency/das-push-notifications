using Microsoft.Extensions.Logging;
using SFA.DAS.PushNotifications.Application.Services;
using SFA.DAS.PushNotifications.Messages.Commands;

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
            try
            {
                _logger.LogDebug("Handling RemoveWebPushSubscriptionCommandHandler for {Endpoint}", message.Endpoint);
                await _pushNotificationsService.RemoveWebPushNotificationSubscription(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error handling RemoveWebPushSubscriptionCommandHandler for {Endpoint}", message.Endpoint);
            }
            _logger.LogInformation("Finished handling RemoveWebPushSubscriptionCommandHandler for {Endpoint}", message.Endpoint);
        }
    }
}