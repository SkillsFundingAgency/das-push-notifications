using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.PushNotifications.Functions.Configuration;
using SFA.DAS.PushNotifications.Messages.Commands;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.PushNotifications.Functions.StartupExtensions
{
    [ExcludeFromCodeCoverage]
    public static class AddNServiceBusExtension
    {
        public static IServiceCollection? AddNServiceBus(this IServiceCollection services, IConfiguration configuration)
        {
            var functionsConfig = configuration.GetSection("SFA.DAS.PushNotifications.Functions").Get<PushNotificationsFunctions>();

            if(functionsConfig != null)
            {
                var endpointConfiguration = new EndpointConfiguration(functionsConfig.NServiceBusEndpointName);
                endpointConfiguration.SendFailedMessagesTo($"{functionsConfig.NServiceBusEndpointName}-error");
                endpointConfiguration.EnableInstallers();
                endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();

                if (!string.IsNullOrEmpty(functionsConfig.NServiceBusLicense))
                {
                    endpointConfiguration.License(functionsConfig.NServiceBusLicense);
                }

                if (configuration["EnvironmentName"] == "LOCAL")
                {
                    var transport = endpointConfiguration.UseTransport<LearningTransport>();
                    var routing = transport.Routing();
                    routing.AddRouting(functionsConfig.NServiceBusEndpointName);
                }
                services.AddSingleton(endpointConfiguration);
                services.AddHostedService<NServiceBusHostedService>();
                
                return services;
            }
            else
            {
                return null;
            }
           
        }

        public static void AddRouting(this RoutingSettings routingSettings, string endpointName)
        {
            routingSettings.RouteToEndpoint(typeof(AddWebPushSubscriptionCommand), endpointName);
            routingSettings.RouteToEndpoint(typeof(RemoveWebPushSubscriptionCommand), endpointName);
        }

        public class NServiceBusHostedService : IHostedService
        {
            private readonly IEndpointInstance _endpointInstance;

            public NServiceBusHostedService(EndpointConfiguration endpointConfiguration)
            {
                _endpointInstance = Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
            }

            public Task StartAsync(CancellationToken cancellationToken) => Task.CompletedTask;

            public async Task StopAsync(CancellationToken cancellationToken)
            {
                await _endpointInstance.Stop().ConfigureAwait(false);
            }
        }
    }
}