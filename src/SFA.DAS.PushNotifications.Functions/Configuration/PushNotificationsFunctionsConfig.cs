using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.PushNotifications.Configuration
{
    [ExcludeFromCodeCoverage]
    public class PushNotificationsFunctionsConfig
    {
        public string NServiceBusConnectionString { get; set; } = null!;
        public string DbConnectionString { get; set; } = null!;
        public string NServiceBusLicense { get; set; } = null!;
        public string NServiceBusEndpointName { get; set; } = null!;
        public string VapidPublicKey { get; set; } = null!;
        public string VapidPrivateKey { get; set; } = null!;
        public string ContactEmail { get; set; } = null!;
    }
}
