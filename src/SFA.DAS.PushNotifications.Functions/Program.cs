using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.ApplicationInsights;
using SFA.DAS.PushNotifications.Application.Services;
using SFA.DAS.PushNotifications.Configuration;
using SFA.DAS.PushNotifications.Data.Extensions;
using SFA.DAS.PushNotifications.Data.Repositories;
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
    .AddTransient<IClientNotificationRepository, ClientNotificationRepository>()
    .AddPushNotificationsDataContext(context.Configuration);
    var configuration = context.Configuration;
    var functionsConfig = configuration.GetSection("SFA.DAS.PushNotifications.Functions").Get<PushNotificationsFunctionsConfig>();

    if (functionsConfig != null && functionsConfig.NServiceBusConnectionString != "UseLearningEndpoint=true")
    {
        Environment.SetEnvironmentVariable("NSERVICEBUS_LICENSE", functionsConfig.NServiceBusLicense);
        Environment.SetEnvironmentVariable("AzureWebJobsStorage", functionsConfig.NServiceBusConnectionString);
    }
   
})
    .UseNServiceBus(config =>
    {
        var endpointConfiguration = new EndpointConfiguration(EndpointName);
        endpointConfiguration.EnableInstallers();
        endpointConfiguration.SendFailedMessagesTo(ErrorEndpointName);
        endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
    })
    .Build();
host.Run();