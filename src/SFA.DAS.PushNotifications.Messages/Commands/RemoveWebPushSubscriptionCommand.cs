namespace SFA.DAS.PushNotifications.Messages.Commands
{
    public class RemoveWebPushSubscriptionCommand
    {
        public Guid ApprenticeId { get; set; }
        public required string Endpoint { get; set; }
    }
}