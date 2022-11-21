using DDApp.API.Models.MetaData;

namespace DDApp.API.Models.Direct
{
    public class DirectMessageModel
    {
        public Guid DirectMessageId { get; set; }
        public string? DirectMessage { get; set; }
        public DateTimeOffset Sended { get; set; }
        public Guid SenderId { get; set; }
        public string SenderName { get; set; } = null!;
        public List<ExternalDirectFileLinkModel>? DirectFiles { get; set; } = new List<ExternalDirectFileLinkModel>();
    }
}
