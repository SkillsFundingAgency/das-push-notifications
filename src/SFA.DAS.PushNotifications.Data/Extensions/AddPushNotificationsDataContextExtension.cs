using Azure.Core;
using Azure.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.PushNotifications.Data.Repositories;

namespace SFA.DAS.PushNotifications.Data.Extensions;

public static class AddPushNotificationsDataContextExtension
{
    private static readonly string AzureResource = "https://database.windows.net/";

    private static readonly ChainedTokenCredential AzureTokenProvider = new ChainedTokenCredential(
        new ManagedIdentityCredential(),
        new AzureCliCredential(),
        new VisualStudioCodeCredential(),
        new VisualStudioCredential()
    );

    public static IServiceCollection AddPushNotificationsDataContext(this IServiceCollection services, IConfiguration configuration)
    {
        var sqlConnectionString = configuration["SFA.DAS.PushNotifications.Functions:DbConnectionString"]!;
        var environmentName = configuration["EnvironmentName"]!;

        services.AddDbContext<PushNotificationsDataContext>((serviceProvider, options) =>
        {
            SqlConnection connection = null!;

            if (!environmentName.Equals("LOCAL", StringComparison.CurrentCultureIgnoreCase))
            {
                connection = new SqlConnection
                {
                    ConnectionString = sqlConnectionString,
                    AccessToken = AzureTokenProvider.GetToken(new TokenRequestContext(scopes: new string[] { AzureResource })).Token
                };
            }
            else
            {
                connection = new SqlConnection(sqlConnectionString);
            }

            options.UseSqlServer(
                connection,
                o => o.CommandTimeout((int)TimeSpan.FromMinutes(5).TotalSeconds));
        });

        services.AddScoped<IPushNotificationsDataContext, PushNotificationsDataContext>(provider => provider.GetService<PushNotificationsDataContext>()!);
        services.AddScoped<IApplicationClientRepository, ApplicationClientRepository>();

        return services;
    }
}