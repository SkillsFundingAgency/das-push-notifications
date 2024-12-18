﻿using Microsoft.Extensions.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.PushNotifications.Messages.Commands;
using System.Diagnostics.CodeAnalysis;

[assembly: ExcludeFromCodeCoverage]
namespace SFA.DAS.PushNotifications.Functions.TestHarness;


internal static class Program
{
    private const string EndpointName = "SFA.DAS.PushNotifications";
    private const string ConfigName = "SFA.DAS.PushNotifications.Functions";
    

    public static async Task Main()
    {
        var builder = new ConfigurationBuilder()
            .AddAzureTableStorage(ConfigName);

        builder.Build();

        var endpointConfiguration = new EndpointConfiguration(EndpointName);
        endpointConfiguration.SendFailedMessagesTo($"{EndpointName}-errors");
        endpointConfiguration.UseSerialization<NewtonsoftJsonSerializer>();
        //endpointConfiguration.SendOnly();

        var transport = endpointConfiguration.UseTransport<LearningTransport>();
            transport.StorageDirectory(
                Path.Combine(
                    Directory.GetCurrentDirectory()
                        .Substring(0, Directory.GetCurrentDirectory().IndexOf("src")),
                    @"src\.learningtransport"));
        var routing = transport.Routing();

        routing.RouteToEndpoint(typeof(AddWebPushSubscriptionCommand), EndpointName);
        routing.RouteToEndpoint(typeof(RemoveWebPushSubscriptionCommand), EndpointName);
        routing.RouteToEndpoint(typeof(SendPushNotificationCommand), EndpointName);


        var endpointInstance = await Endpoint.Start(endpointConfiguration);

        await TestMessages.AddMessages(endpointInstance);

        await endpointInstance.Stop();
    }
    
}

