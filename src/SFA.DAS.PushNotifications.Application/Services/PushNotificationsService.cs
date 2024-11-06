using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
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
        IConfiguration configuration
        )
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
            _logger.LogError("Endpoint is null or empty for apprenticeId {ApprenticeId}", message.ApprenticeAccountIdentifier);
            throw new ArgumentException("Endpoint is null or empty");
        }
      
        var applicationClient = new ApplicationClient
        {
            UserAccountId = message.ApprenticeAccountIdentifier,
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
        if (!string.IsNullOrEmpty(message.ApprenticeAccountIdentifier.ToString()))
        {
            CancellationToken cancellationToken = CancellationToken.None;
            List<ApplicationClient> applicationClients = await _applicationClientRepository.GetApplicationClients((int)ApplicationEnum.Application.ApprenticeApp, message.ApprenticeAccountIdentifier);
        
            if(applicationClients != null && applicationClients.Count > 0)
            {
                foreach(var appClient in applicationClients)
                {
                    var clientNotification = await _clientNotificationRepository.AddClientNotification(appClient.Id, message, cancellationToken);
                    if(clientNotification != null)
                    {
                        var subscription = await GetSubscriptionDetails(appClient);
                        if (!string.IsNullOrEmpty(subscription.Endpoint))
                        { 
                            _logger.LogInformation("Sending notification for Client Notification Id {ClientNotificationId}", clientNotification.Id);
                            var result = await SendNotification(subscription, clientNotification.Payload);
                            if (result == "success")
                            {
                                clientNotification.Status = (int)ClientNotificationStatus.Success;

                            }
                            else
                            {
                                clientNotification.Status = (int)ClientNotificationStatus.Failed;
                                clientNotification.FailureReason = result;
                            }

                            await _clientNotificationRepository.UpdateClientNotification(clientNotification, cancellationToken);
                        }
                    }
                }
            }
            else
            {
                _logger.LogInformation("No application clients stored for this apprentice account identifier: {ApprenticeAccountIdentifier}", message.ApprenticeAccountIdentifier);
                await Task.CompletedTask;
            }
        }
    }

    private async Task<PushSubscription> GetSubscriptionDetails(ApplicationClient appClient)
    {
        var subscription = new PushSubscription(appClient.Endpoint,
          appClient.SubscriptionPublicKey, appClient.SubscriptionAuthenticationSecret);

        return subscription;
    }

    public async Task<string> SendNotification(PushSubscription subscription, string payload)
    {
        var publicKey = _configuration["SFA.DAS.PushNotifications.Functions:VapidKeys:PublicKey"];
        var privateKey = _configuration["SFA.DAS.PushNotifications.Functions:VapidKeys:PrivateKey"];

        var vapidDetails = new VapidDetails(
            "mailto:cathy.groom@education.gov.uk",
            publicKey, privateKey
        );

        if (!string.IsNullOrEmpty(vapidDetails.PublicKey))
        {
            var webPushClient = new WebPushClient();
            try
            {
                CancellationToken cancellationToken = CancellationToken.None;
                await webPushClient.SendNotificationAsync(subscription, payload, vapidDetails, cancellationToken);
                _logger.LogInformation("Push notification sent successfully.");
                return "success";
            }
            catch (WebPushException exception)
            {
                _logger.LogError(exception, "Error sending notification: {Message}", exception.Message);
                return exception.Message;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending notification: {Message}", ex.Message);
                return ex.Message;
            }
        }
        
        return "No Vapid Keys Found";
    }
}