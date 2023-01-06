namespace DDApp.API.Models.Direct
{
    public class CreateDirectMessageModel
    {
        public Guid DirectId { get; set; }
        public Guid? DirectMessageId { get; set; }
        public string? Message { get; set; }
        public List<MetadataModel>? Files { get; set; }
        public DateTimeOffset? Sended { get; set; }
    }
}
