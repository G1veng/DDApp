namespace DDApp.API.Models.MetaData
{
    public class ExternalPostFileLinkModel
    {
        public Guid PostId { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public string? Link { get; set; }
    }
}
