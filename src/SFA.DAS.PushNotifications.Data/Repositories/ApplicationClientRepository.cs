using Microsoft.Extensions.Logging;
using SFA.DAS.PushNotifications.Model.Entities;
using static SFA.DAS.PushNotifications.Model.Entities.ApplicationClientStatusEnum;
using static SFA.DAS.PushNotifications.Model.Entities.ApplicationEnum;

namespace SFA.DAS.PushNotifications.Data.Repositories;

public interface IApplicationClientRepository
{
    Task<int> AddWebPushNotificationSubscription(ApplicationClient applicationClient, CancellationToken cancellationToken);
    Task RemoveWebPushNotificationSubscription(ApplicationClient applicationClient, CancellationToken cancellationToken);
}

public class ApplicationClientRepository : IApplicationClientRepository
{
    private readonly IPushNotificationsDataContext _context;
    private readonly ILogger<ApplicationClientRepository> _logger;

    public ApplicationClientRepository(IPushNotificationsDataContext context, ILogger<ApplicationClientRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<int> AddWebPushNotificationSubscription(ApplicationClient applicationClient, CancellationToken cancellationToken)
    {
       _logger.LogDebug("Adding push notification subscription for {Endpoint}", applicationClient.Endpoint);

        var appClients = _context.ApplicationClients.Where(x => x.Endpoint == applicationClient.Endpoint);
        if(appClients.Any()) { 
            foreach(var appClient in appClients)
            {
                _context.ApplicationClients.Remove(appClient);
            }
        }
        
        applicationClient.ApplicationId = (int)Application.ApprenticeApp;
        applicationClient.DateCreated = DateTime.UtcNow;
        applicationClient.Status = (int)ApplicationClientStatus.Active;
        _context.ApplicationClients.Add(applicationClient);
        
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Added push notification subscription for {Endpoint}", applicationClient.Endpoint);

        return applicationClient.Id;
    }

    public async Task RemoveWebPushNotificationSubscription(ApplicationClient applicationClient, CancellationToken cancellationToken)
    {
        _logger.LogDebug("Removing push notification subscription for {Endpoint}", applicationClient.Endpoint);
        var appClient = _context.ApplicationClients.FirstOrDefault(x => x.Endpoint == applicationClient.Endpoint);
        if (appClient == null)
        {
            return;
        }
        appClient.Status = (int)ApplicationClientStatus.Unsubscribed;
        appClient.LastUpdatedOn = DateTime.UtcNow;
        _context.ApplicationClients.Update(appClient);
        await _context.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("Removed push notification subscription for {Endpoint}", applicationClient.Endpoint);

    }
}