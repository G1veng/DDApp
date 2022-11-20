namespace DDApp.API.Models.Direct
{
    public class DirectModel
    {
        public Guid RecipientId { get; set; }
        public string RecipientUserName { get; set; } = null!;
        public Guid DirectId { get; set; }
        public string? DirectImage { get; set; }
    }
}
