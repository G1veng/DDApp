namespace DDApp.API.Models.Subscription
{
    public class SubscriptionModel
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } = null!;
        public string? UserAvatar { get; set; }
        public DateTimeOffset LastEntered { get; set; }
    }
}
