using SFA.DAS.PushNotifications.Messages.Commands;
using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.PushNotifications.Model.Entities
{
    [ExcludeFromCodeCoverage]
    public class ClientNotification
    {
        public required Guid Id { get; set; }
        public int Status { get; set; }
        public DateTime CreatedTime { get; set; }
        public string FailureReason { get; set; } = null!;
        public DateTime TimeToSend { get; set; }
        public DateTime TimeSent { get; set; }
        public int ApplicationClientId { get; set; }
        public SendPushNotificationCommand Payload { get; set; } = null!;
    }

    [Flags]
    public enum ClientNotificationStatus
    {
        Pending = 1,
        Success = 2,
        Failed = 3
    }
}