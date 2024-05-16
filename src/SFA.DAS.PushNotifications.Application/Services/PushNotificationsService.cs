using Microsoft.Extensions.Logging;
using SFA.DAS.PushNotifications.Data.Repositories;
using SFA.DAS.PushNotifications.Messages.Commands;
using SFA.DAS.PushNotifications.Model.Entities;

namespace SFA.DAS.PushNotifications.Application.Services;

public interface IPushNotificationsService
{
    Task<int> AddWebPushNotificationSubscription(AddWebPushSubscriptionCommand message);
    Task RemoveWebPushNotificationSubscription(RemoveWebPushSubscriptionCommand message);
}

public class PushNotificationsService : IPushNotificationsService
{
    private readonly IApplicationClientRepository _applicationClientRepository;
    private readonly ILogger<PushNotificationsService> _logger;

    public PushNotificationsService(
        IApplicationClientRepository applicationClientRepository,
        ILogger<PushNotificationsService> logger)
    {
        _applicationClientRepository = applicationClientRepository;
        _logger = logger;
    }

    public async Task<int> AddWebPushNotificationSubscription(AddWebPushSubscriptionCommand message)
    {
        CancellationToken cancellationToken = CancellationToken.None;

        _logger.LogInformation($"Adding subscription for {message.Endpoint}");

        var applicationClient = new ApplicationClient
        {
            UserAccountId = message.ApprenticeId,
            Endpoint = message.Endpoint,
            SubscriptionPublicKey = message.PublicKey,
            SubscriptionAuthenticationSecret = message.AuthenticationSecret
        };
        var appClientId = await _applicationClientRepository.AddWebPushNotificationSubscription(applicationClient, cancellationToken);
        return appClientId;
    }

    public async Task RemoveWebPushNotificationSubscription(RemoveWebPushSubscriptionCommand message)
    {
        CancellationToken cancellationToken = CancellationToken.None;
        var applicationClient = new ApplicationClient 
        { 
            UserAccountId = message.ApprenticeId,
            Endpoint = message.Endpoint 
        };

        _logger.LogInformation($"Unsubscribing to push notifications for {applicationClient.Endpoint}");

        await _applicationClientRepository.RemoveWebPushNotificationSubscription(applicationClient, cancellationToken);
    }
}