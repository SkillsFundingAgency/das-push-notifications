using Microsoft.Extensions.Logging;
using SFA.DAS.PushNotifications.Data.Entities;
using SFA.DAS.PushNotifications.Data.Repositories;

namespace SFA.DAS.PushNotifications.Services;

public interface IPushNotificationsService
{
    Task<int> AddWebPushNotificationSubscription(ApplicationClient apprenticeAppSubscription);
    Task RemoveWebPushNotificationSubscription(ApplicationClient apprenticeAppRemoveSubscription);
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

    public async Task<int> AddWebPushNotificationSubscription(ApplicationClient apprenticeAppSubscription)
    {
        CancellationToken cancellationToken = CancellationToken.None;

        _logger.LogInformation($"Adding subscription for {apprenticeAppSubscription.Endpoint}");
        var appClientId = await _applicationClientRepository.AddWebPushNotificationSubscription(apprenticeAppSubscription, cancellationToken);
        return appClientId;
    }

    public async Task RemoveWebPushNotificationSubscription(ApplicationClient apprenticeAppRemoveSubscription)
    {
        CancellationToken cancellationToken = CancellationToken.None;

        _logger.LogInformation($"Unsubscribing to push notifications for {apprenticeAppRemoveSubscription.Endpoint}");
        await _applicationClientRepository.RemoveWebPushNotificationSubscription(apprenticeAppRemoveSubscription, cancellationToken);
    }
}