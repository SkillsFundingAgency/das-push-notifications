using System.Diagnostics.CodeAnalysis;

namespace SFA.DAS.PushNotifications.Model.Entities
{
    [ExcludeFromCodeCoverage]
    public class ClientNotification
    {
        public required string MessageId { get; set; }
        public int Status { get; set; }
        public string FailureReason { get; set; } = null!;
        public DateTime TimeToSend { get; set; }
        public DateTime StatusTime { get; set; }
    }
}