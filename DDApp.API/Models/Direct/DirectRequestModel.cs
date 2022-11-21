namespace DDApp.API.Models.Direct
{
    public class DirectRequestModel
    {
        public Guid DirectId { get; set; }
        public Guid RecipientId { get; set; }
        public List<DirectMessageModel>? DirectMessages { get; set; }

    }
}
