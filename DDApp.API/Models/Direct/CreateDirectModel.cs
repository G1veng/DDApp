namespace DDApp.API.Models.Direct
{
    public class CreateDirectModel
    {
        public Guid? Id { get; set; }
        public Guid UserId { get; set; }
        public string? Title { get; set; }
        public string? DirectImage { get; set; }
    }
}
