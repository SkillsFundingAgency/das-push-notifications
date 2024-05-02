namespace SFA.DAS.PushNotifications.Messages.Commands
{
    public class RemoveWebPushSubscriptionCommand : ICommand
    {
        public Guid ApprenticeId { get; set; }
        public required string Endpoint { get; set; }
    }
}