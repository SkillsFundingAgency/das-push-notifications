using Microsoft.EntityFrameworkCore;
using SFA.DAS.PushNotifications.Data.Entities;

namespace SFA.DAS.PushNotifications.Data
{
    public interface IPushNotificationsDataContext
    {
        DbSet<ApplicationClient> ApplicationClients { get; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}