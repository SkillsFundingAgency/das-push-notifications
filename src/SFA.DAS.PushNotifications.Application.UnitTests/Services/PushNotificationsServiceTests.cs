using FluentAssertions;
using SFA.DAS.PushNotifications.Application.Services;
using SFA.DAS.PushNotifications.Messages.Commands;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.PushNotifications.Application.UnitTests.Services
{
    [TestFixture]
    public class PushNotificationsServiceTests
    {
        [Test, MoqAutoData]
        public async Task Add_Web_Push_Notification_Subscription_Returns_Id(
            PushNotificationsService service,
            AddWebPushSubscriptionCommand message)
        {
            var result = await service.AddWebPushNotificationSubscription(message);
            result.Should().BeGreaterThan(0);
        }

        [Test, MoqAutoData]
        public async Task Remove_Web_Push_Notification_Subscription(
           PushNotificationsService service,
           RemoveWebPushSubscriptionCommand message)
        {
            Assert.DoesNotThrowAsync(async () => await service.RemoveWebPushNotificationSubscription(message));
        }
    }
}