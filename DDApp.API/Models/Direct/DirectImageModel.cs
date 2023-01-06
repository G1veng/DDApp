namespace DDApp.API.Models.Direct
{
    public class DirectImageModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string MimeType { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public long Size { get; set; }
        public string? Link { get; set; }
    }
}
