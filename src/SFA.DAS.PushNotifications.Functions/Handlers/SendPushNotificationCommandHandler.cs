using Microsoft.Extensions.Logging;
using SFA.DAS.PushNotifications.Application.Services;
using SFA.DAS.PushNotifications.Messages.Commands;

namespace SFA.DAS.PushNotifications.Functions.Handlers
{
    public class SendPushNotificationCommandHandler : IHandleMessages<SendPushNotificationCommand>
    {
        private readonly IPushNotificationsService _pushNotificationsService;
        private readonly ILogger<SendPushNotificationCommandHandler> _logger;

        public SendPushNotificationCommandHandler(IPushNotificationsService pushNotificationsService, ILogger<SendPushNotificationCommandHandler> logger)
        {
            _pushNotificationsService = pushNotificationsService;
            _logger = logger;
        }
        public async Task Handle(SendPushNotificationCommand message, IMessageHandlerContext context)
        {
            if (message != null)
            {
                string preMessage = $"Handle SendPushNotificationCommand for {message.ApprenticeAccountIdentifier}";
                _logger.LogInformation(preMessage);
                await _pushNotificationsService.ProcessPushNotificationMessage(message);
            }
            else
            {
                _logger.LogError("Error handling SendPushNotificationCommand. Missing or incorrect message supplied.");
            }
        }
    }
}
