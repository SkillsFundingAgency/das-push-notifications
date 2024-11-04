namespace SFA.DAS.PushNotifications.Messages.Commands
{
    public class SendPushNotificationCommand : ICommand
    {
        public Guid ApprenticeAccountIdentifier { get; set; }
        public string Title { get; set; } = null!;
        public string Body { get; set; } = null!;
    }
}
