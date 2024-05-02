using SFA.DAS.PushNotifications.Data.Entities;
using static SFA.DAS.PushNotifications.Data.Entities.ApplicationClientStatusEnum;

namespace SFA.DAS.PushNotifications.Data.Repositories;

public interface IApplicationClientRepository
{
    Task<int> AddWebPushNotificationSubscription(ApplicationClient applicationClient, CancellationToken cancellationToken);
    Task RemoveWebPushNotificationSubscription(ApplicationClient applicationClient, CancellationToken cancellationToken);
    Task DeleteWebPushNotificationSubscription(ApplicationClient applicationClient, CancellationToken cancellationToken);
}

public class ApplicationClientRepository : IApplicationClientRepository
{
    private readonly IPushNotificationsDataContext _context;

    public ApplicationClientRepository(IPushNotificationsDataContext context)
    {
        _context = context;
    }

    public async Task<int> AddWebPushNotificationSubscription(ApplicationClient applicationClient, CancellationToken cancellationToken)
    {
        var appClient = _context.ApplicationClients.Where(x => x.Endpoint == applicationClient.Endpoint).FirstOrDefault();
        if (appClient != null)
        {
            appClient.Status = (int)ApplicationClientStatus.Active;
            _context.ApplicationClients.Update(appClient);
        }
        else
        {
            applicationClient.DateCreated = DateTime.UtcNow;
            applicationClient.Status = (int)ApplicationClientStatus.Active;
            _context.ApplicationClients.Add(applicationClient);
        }
        await _context.SaveChangesAsync(cancellationToken);
        return applicationClient.Id;
    }

    public async Task RemoveWebPushNotificationSubscription(ApplicationClient applicationClient, CancellationToken cancellationToken)
    {
        var appClient = _context.ApplicationClients.Where(x => x.Endpoint == applicationClient.Endpoint).FirstOrDefault();
        if (appClient != null)
        {
            appClient.Status = (int)ApplicationClientStatus.Unsubscribed;
            _context.ApplicationClients.Update(appClient);
        }
        else
        {
            applicationClient.DateCreated = DateTime.UtcNow;
            _context.ApplicationClients.Add(applicationClient);
        }
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteWebPushNotificationSubscription(ApplicationClient applicationClient, CancellationToken cancellationToken)
    {
        var appClient = _context.ApplicationClients.Where(x => x.Endpoint == applicationClient.Endpoint).FirstOrDefault();
        if (appClient != null)
        {
            _context.ApplicationClients.Remove(appClient);
        }
        await _context.SaveChangesAsync(cancellationToken);
    }
}