namespace DDApp.API.Models.Push
{
    public class SendPushModel
    {
        public Guid? UserId { get; set; }
        public PushModel Push { get; set; } = null!;
    }
}
