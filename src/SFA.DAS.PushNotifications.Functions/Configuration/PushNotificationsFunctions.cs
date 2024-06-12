using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.PushNotifications.Functions.Configuration
{
    [ExcludeFromCodeCoverage]
    public class PushNotificationsFunctions
    {
        public string NServiceBusConnectionString { get; set; } = null!;
        public string DbConnectionString { get; set; } = null!;
        public string NServiceBusLicense { get; set; } = null!;
        public string NServiceBusEndpointName { get; set; } = null!;   
    }
}
