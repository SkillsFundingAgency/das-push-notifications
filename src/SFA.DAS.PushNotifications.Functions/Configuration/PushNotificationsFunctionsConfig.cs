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

        public VapidKeys VapidKeys { get; set; } = null!;
    }

    public class VapidKeys
    {
        public string PublicKey { get; set; } = null!;
        public string PrivateKey { get; set; } = null!;
    }
}
