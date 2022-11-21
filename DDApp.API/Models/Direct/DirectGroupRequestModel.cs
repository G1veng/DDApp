namespace DDApp.API.Models.Direct
{
    public class DirectGroupRequestModel
    {
        public Guid DirectId { get; set; }
        public string DirectTitle { get; set; } = null!;
        public virtual List<Guid> DirectMembers { get; set; } = null!;
        public virtual string? DirectImage { get; set; }
    }
}
