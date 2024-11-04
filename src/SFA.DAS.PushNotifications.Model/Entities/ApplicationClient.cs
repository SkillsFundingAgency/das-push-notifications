namespace SFA.DAS.PushNotifications.Model.Entities
{
    public class ApplicationClient
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public Guid UserAccountId { get; set; }
        public int? PushProtocol { get; set; }
        public int Status { get; set; }
        public required string Endpoint { get; set; }
        public string SubscriptionPublicKey { get; set; } = null!;
        public string SubscriptionAuthenticationSecret { get; set; } = null!;
        public string? Handle { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public DateTime? LastUpdatedOn { get; set; }
    }

    [Flags]
    public enum ApplicationClientStatus
    {
        Active = 1,
        Inactive = 2,
        Expired = 3,
        Unsubscribed = 4
    }
}