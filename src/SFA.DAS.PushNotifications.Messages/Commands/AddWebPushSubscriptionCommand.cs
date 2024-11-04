namespace SFA.DAS.PushNotifications.Messages.Commands
{
    public class AddWebPushSubscriptionCommand : ICommand
    {
        public Guid ApprenticeAccountIdentifier { get; set; }
        public required string Endpoint { get; set; }
        public string PublicKey { get; set; } = null!;
        public string AuthenticationSecret { get; set; } = null!;
    }
}