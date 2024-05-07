namespace SFA.DAS.PushNotifications.Functions.Configuration
{
    public class PushNotificationsFunctions
    {
        public string DatabaseConnectionString { get; set; }
        public string NServiceBusConnectionString { get; set; }
        public string NServiceBusLicense {  get; set; }
        public string NServiceBusEndpointName { get; set; }
        public string SharedServiceBusNamespace { get; set; }
    }
}
