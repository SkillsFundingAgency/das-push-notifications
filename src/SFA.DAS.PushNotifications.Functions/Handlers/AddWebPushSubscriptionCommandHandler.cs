using Microsoft.Extensions.Logging;
using SFA.DAS.PushNotifications.Application.Services;
using SFA.DAS.PushNotifications.Messages.Commands;

namespace SFA.DAS.PushNotifications.Functions.Handlers
{
    public class AddWebPushSubscriptionCommandHandler : IHandleMessages<AddWebPushSubscriptionCommand>
    {
        private readonly IPushNotificationsService _pushNotificationsService;
        private readonly ILogger<AddWebPushSubscriptionCommandHandler> _logger;

        public AddWebPushSubscriptionCommandHandler(IPushNotificationsService pushNotificationsService, ILogger<AddWebPushSubscriptionCommandHandler> logger)
        {
            _pushNotificationsService = pushNotificationsService;
            _logger = logger;
        }
        public async Task Handle(AddWebPushSubscriptionCommand message, IMessageHandlerContext context)
        {
            if(message != null)
            {
                _logger.LogInformation("Handle AddWebPushSubscriptionCommand for {ApprenticeId}", message.ApprenticeId);
                await _pushNotificationsService.AddWebPushNotificationSubscription(message);
            }
            else
            {
                _logger.LogError("Error handling AddWebPushSubscriptionCommand. Missing or incorrect message supplied.");
            }
        }
    }
}