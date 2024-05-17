using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using SFA.DAS.PushNotifications.Application.Services;
using SFA.DAS.PushNotifications.Data.Extensions;
using SFA.DAS.PushNotifications.Data.Repositories;
using SFA.DAS.PushNotifications.Functions.Configuration;
using SFA.DAS.PushNotifications.Functions.StartupExtensions;

[assembly: NServiceBusTriggerFunction("SFA.DAS.PushNotifications")]

const string EndpointName = "SFA.DAS.PushNotifications";
const string ErrorEndpointName = $"{EndpointName}-error";

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureAppConfiguration((hostBuilderContext, builder) =>
    {
        builder.BuildAppConfiguration(hostBuilderContext.Configuration);
    })
.ConfigureServices((context, services) =>
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
    .AddPushNotificationsDataContext(context.Configuration);
    var configuration = context.Configuration;
    var functionsConfig = configuration.GetSection("SFA.DAS.PushNotifications.Functions").Get<PushNotificationsFunctions>();

    if (functionsConfig != null)
    {
        Environment.SetEnvironmentVariable("AzureWebJobsServiceBus", functionsConfig.NServiceBusConnectionString);
        Environment.SetEnvironmentVariable("NSERVICEBUS_LICENSE", functionsConfig.NServiceBusLicense);
    }
})
    .UseNServiceBus((config, endpointConfiguration) =>
    {
        endpointConfiguration.AdvancedConfiguration.EnableInstallers();
        endpointConfiguration.AdvancedConfiguration.SendFailedMessagesTo(ErrorEndpointName);
    
    })
    .Build();

host.Run();