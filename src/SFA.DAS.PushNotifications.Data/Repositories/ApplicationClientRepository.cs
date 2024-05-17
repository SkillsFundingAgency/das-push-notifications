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

    public ApplicationClientRepository(IPushNotificationsDataContext context)
    {
        _context = context;
    }

    public async Task<int> AddWebPushNotificationSubscription(ApplicationClient applicationClient, CancellationToken cancellationToken)
    {
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
        return applicationClient.Id;
    }

    public async Task RemoveWebPushNotificationSubscription(ApplicationClient applicationClient, CancellationToken cancellationToken)
    {
        var appClient = _context.ApplicationClients.FirstOrDefault(x => x.Endpoint == applicationClient.Endpoint);
        if (appClient == null)
        {
            return;
        }
        appClient.Status = (int)ApplicationClientStatus.Unsubscribed;
        appClient.LastUpdatedOn = DateTime.UtcNow;
        _context.ApplicationClients.Update(appClient);
        await _context.SaveChangesAsync(cancellationToken);
    }
}