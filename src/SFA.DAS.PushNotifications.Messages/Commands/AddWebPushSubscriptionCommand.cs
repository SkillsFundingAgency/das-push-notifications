namespace SFA.DAS.PushNotifications.Messages.Commands
{
    public class AddWebPushSubscriptionCommand
    {
        public Guid ApprenticeId { get; set; }
        public required string Endpoint { get; set; }
        public string PublicKey { get; set; } = null!;
        public string AuthenticationSecret { get; set; } = null!;
    }
}