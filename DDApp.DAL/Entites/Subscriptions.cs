namespace DDApp.DAL.Entites
{
    public class Subscriptions
    {
        /// <summary>
        /// Id на кого подписался пользователь
        /// </summary>
        public Guid SubscriptionId { get; set; }

        /// <summary>
        /// Id того, кто подписался
        /// </summary>
        public Guid SubscriberId { get; set; }

        public virtual User UserSubscriber { get; set; } = null!;
        public virtual User UserSubscription { get; set; } = null!;
    }
}
