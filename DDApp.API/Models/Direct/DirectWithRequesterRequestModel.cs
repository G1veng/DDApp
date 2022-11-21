namespace DDApp.API.Models.Direct
{
    public class DirectWithRequesterRequestModel
    {
        public DirectModel Direct { get; set; } = null!;
        public Guid RequesterId { get; set; }
    }
}
