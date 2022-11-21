namespace DDApp.API.Models.Direct
{
    public class CreateDirectGroupModel
    {
        public string Title { get; set; } = null!;
        public MetadataModel? GroupImage { get; set; }
        public List<Guid> Members { get; set; } = null!;
    }
}
