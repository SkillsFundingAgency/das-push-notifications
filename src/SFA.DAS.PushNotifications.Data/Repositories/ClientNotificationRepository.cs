using Microsoft.EntityFrameworkCore.Storage.Json;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.PushNotifications.Messages.Commands;
using SFA.DAS.PushNotifications.Model.Entities;
using System;
using System.Threading;

namespace SFA.DAS.PushNotifications.Data.Repositories
{
    public interface IClientNotificationRepository
    {
        Task<ClientNotification> AddClientNotification(int applicationClientId, SendPushNotificationCommand message, CancellationToken cancellation);
        Task SendPushNotification(ClientNotification notification, CancellationToken cancellation);
    }

    public class ClientNotificationRepository : IClientNotificationRepository
    {
        private readonly IPushNotificationsDataContext _context;
        private readonly ILogger<ClientNotificationRepository> _logger;

        public ClientNotificationRepository(IPushNotificationsDataContext context, ILogger<ClientNotificationRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ClientNotification> AddClientNotification(int applicationClientId, SendPushNotificationCommand message, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Adding clientNotification for applicationClientId {Id}", applicationClientId);
            string payload = await PayloadContents(message);
            ClientNotification clientNotification = new()
            {
                Status = (int)ClientNotificationStatus.Pending,
                CreatedTime = DateTime.UtcNow,
                ApplicationClientId = applicationClientId,
                Payload = payload,
                Id = Guid.NewGuid(),
                TimeToSend = DateTime.UtcNow,
                TimeSent = DateTime.UtcNow
            };

            _context.ClientNotification.Add(clientNotification);

            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Added client notification for {ClientNotificationId}", clientNotification.Id);

            return clientNotification;
        }

        public async Task SendPushNotification(ClientNotification notification, CancellationToken cancellation)
        {
            _logger.LogInformation("Sending push notification for {applicationClientId}", notification.ApplicationClientId);
            await _context.ClientNotification.AddAsync(notification, cancellation);
            await _context.SaveChangesAsync(cancellation);
        }

        private async Task<string> PayloadContents(SendPushNotificationCommand message)
        {
            var payload = JsonConvert.SerializeObject(new
            {
                title = message.Title,
                message = message.Body,
                url = "https://localhost:5003/Tasks?status=0"
            });

            return payload;
        }
    }
}
