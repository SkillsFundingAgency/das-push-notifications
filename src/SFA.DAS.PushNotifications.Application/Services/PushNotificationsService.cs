﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SFA.DAS.PushNotifications.Data.Repositories;
using SFA.DAS.PushNotifications.Messages.Commands;
using SFA.DAS.PushNotifications.Model.Entities;
using WebPush;

namespace SFA.DAS.PushNotifications.Application.Services;

public interface IPushNotificationsService
{
    Task<int> AddWebPushNotificationSubscription(AddWebPushSubscriptionCommand message);
    Task RemoveWebPushNotificationSubscription(RemoveWebPushSubscriptionCommand message);
    Task ProcessPushNotificationMessage(SendPushNotificationCommand message);
   
}

public class PushNotificationsService : IPushNotificationsService
{
    private readonly IApplicationClientRepository _applicationClientRepository;
    private readonly IClientNotificationRepository _clientNotificationRepository;
    private readonly ILogger<PushNotificationsService> _logger;
    private readonly IConfiguration _configuration;

    public PushNotificationsService(
        IApplicationClientRepository applicationClientRepository,
        IClientNotificationRepository clientNotificationRepository,
        ILogger<PushNotificationsService> logger,
        IConfiguration configuration)
    {
        _applicationClientRepository = applicationClientRepository;
        _clientNotificationRepository = clientNotificationRepository;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<int> AddWebPushNotificationSubscription(AddWebPushSubscriptionCommand message)
    {
        CancellationToken cancellationToken = CancellationToken.None;

        _logger.LogInformation("Adding subscription for {Endpoint}", message.Endpoint);

        if(string.IsNullOrEmpty(message.Endpoint))
        {
            _logger.LogError("Endpoint is null or empty for apprenticeId {ApprenticeId}", message.ApprenticeId);
            throw new ArgumentException("Endpoint is null or empty");
        }
      
        var applicationClient = new ApplicationClient
        {
            UserAccountId = message.ApprenticeId,
            Endpoint = message.Endpoint,
            SubscriptionPublicKey = message.PublicKey,
            SubscriptionAuthenticationSecret = message.AuthenticationSecret
        };
        var appClientId = await _applicationClientRepository.AddWebPushNotificationSubscription(applicationClient, cancellationToken);
        _logger.LogInformation("Added application client {AppClientId}", appClientId);
        return appClientId;
    }

    public async Task RemoveWebPushNotificationSubscription(RemoveWebPushSubscriptionCommand message)
    {
        _logger.LogInformation("Unsubscribing to push notifications for {Endpoint}", message.Endpoint);

        CancellationToken cancellationToken = CancellationToken.None;
        var applicationClient = new ApplicationClient 
        { 
            UserAccountId = message.ApprenticeId,
            Endpoint = message.Endpoint 
        };

        await _applicationClientRepository.RemoveWebPushNotificationSubscription(applicationClient, cancellationToken);

        _logger.LogInformation("Unsubscribed to push notifications for {Endpoint}", applicationClient.Endpoint);
    }

    public async Task ProcessPushNotificationMessage(SendPushNotificationCommand message)
    {
        if (!string.IsNullOrEmpty(message.ApprenticeId.ToString()))
        {
            CancellationToken cancellationToken = CancellationToken.None;
            List<ApplicationClient> applicationClients = await _applicationClientRepository.GetApplicationClients((int)ApplicationEnum.Application.ApprenticeApp, message.ApprenticeId);
        
            if(applicationClients == null || applicationClients.Count > 0)
            {
                foreach(var appClient in applicationClients)
                {
                    //create encrypted message
                    var clientNotification = await _clientNotificationRepository.AddClientNotification(appClient.Id, message, cancellationToken);
                    if(clientNotification != null)
                    {
                        var subscription = await GetSubscriptionDetails(appClient);
                        await SendNotification(subscription, clientNotification.Payload.Body);
                    }
                }
            }
        }
    }

    private async Task<PushSubscription> GetSubscriptionDetails(ApplicationClient appClient)
    {
        var subscription = new PushSubscription(appClient.Endpoint,
          appClient.SubscriptionPublicKey, appClient.SubscriptionAuthenticationSecret);

        if (subscription.Endpoint != null)
        {
            return subscription;
        }
        return null;
    }

    private async Task SendNotification(PushSubscription subscription, string payload)
    {
        var vapidDetails = new VapidDetails(
            "mailto:cathy.groom@education.gov.uk",
            _configuration["VapidPublicKey"],
            _configuration["VapidPrivateKey"]
       );
        var webPushClient = new WebPushClient();
        try
        {
            webPushClient.SendNotification(subscription, payload, vapidDetails);
            _logger.LogInformation("Push notification sent successfully.");
        }
        catch (WebPushException exception)
        {
            _logger.LogInformation("Error sending notification: " + exception.Message);
        }
    }
}