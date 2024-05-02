using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.ApplicationInsights;
using Microsoft.Extensions.Logging;
using SFA.DAS.PushNotifications.Data.Extensions;
using SFA.DAS.PushNotifications.Data.Repositories;
using SFA.DAS.PushNotifications.Functions.StartupExtensions;
using SFA.DAS.PushNotifications.Services;

[assembly: NServiceBusTriggerFunction("SFA.DAS.PushNotifications.Functions")]

const string EndpointName = "SFA.DAS.PushNotifications.Functions";
const string ErrorEndpointName = $"{EndpointName}-error";

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((hostBuilderContext, builder) =>
    {
        builder.BuildAppConfiguration(hostBuilderContext.Configuration);
    })
.ConfigureServices((config, services) =>
{
    services.AddLogging(builder =>
    {
        builder.AddFilter<ApplicationInsightsLoggerProvider>(string.Empty, LogLevel.Information);
        builder.AddFilter<ApplicationInsightsLoggerProvider>("Microsoft", LogLevel.Warning);
    })
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights()
    .AddTransient<IPushNotificationsService, PushNotificationsService>()
    .AddTransient<IApplicationClientRepository, ApplicationClientRepository>()
    .AddPushNotificationsDataContext(config.Configuration);
})
    .UseNServiceBus((config, endpointConfiguration) =>
    {
        endpointConfiguration.AdvancedConfiguration.EnableInstallers();
        endpointConfiguration.AdvancedConfiguration.SendFailedMessagesTo(ErrorEndpointName);
    })
    .Build();

host.Run();