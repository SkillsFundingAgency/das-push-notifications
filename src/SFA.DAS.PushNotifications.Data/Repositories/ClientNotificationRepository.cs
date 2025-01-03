﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SFA.DAS.PushNotifications.Messages.Commands;
using SFA.DAS.PushNotifications.Model.Entities;

namespace SFA.DAS.PushNotifications.Data.Repositories
{
    public interface IClientNotificationRepository
    {
        Task<ClientNotification> AddClientNotification(int applicationClientId, SendPushNotificationCommand message, CancellationToken cancellation);
        Task<ClientNotification> UpdateClientNotification(ClientNotification clientNotification, CancellationToken cancellationToken);
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
            string payload = PayloadContents(message);
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

        public async Task<ClientNotification> UpdateClientNotification(ClientNotification clientNotification, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating client notification for {Id}", clientNotification.Id);
            _context.ClientNotification.Update(clientNotification);
            await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation("Updated client notification for {Id}", clientNotification.Id);

            return clientNotification;
        }

        private string PayloadContents(SendPushNotificationCommand message)
        {
            var payload = JsonConvert.SerializeObject(new
            {
                title = message.Title,
                message = message.Body,
                url = "/Tasks"

            });

            return payload;
        }
    }
}
