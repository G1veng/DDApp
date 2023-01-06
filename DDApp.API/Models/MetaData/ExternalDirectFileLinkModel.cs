namespace DDApp.API.Models.MetaData
{
    public class ExternalDirectFileLinkModel
    {
        public string? Link { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public long Size { get; set; }
    }
}
