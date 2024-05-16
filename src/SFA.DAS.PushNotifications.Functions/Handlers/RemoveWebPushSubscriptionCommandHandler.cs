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
            _logger.LogInformation($"Handling {nameof(RemoveWebPushSubscriptionCommandHandler)} for {message.Endpoint}");

            try
            {
                await _pushNotificationsService.RemoveWebPushNotificationSubscription(message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error handling ApprenticeSubscriptionDeleteEvent for {message.Endpoint}");
            }
        }
    }
}