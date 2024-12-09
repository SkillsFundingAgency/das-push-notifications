using Microsoft.EntityFrameworkCore;
using SFA.DAS.PushNotifications.Model.Entities;

namespace SFA.DAS.PushNotifications.Data
{
    public interface IPushNotificationsDataContext
    {
        DbSet<ApplicationClient> ApplicationClients { get; }
        DbSet<ClientNotification> ClientNotification { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}