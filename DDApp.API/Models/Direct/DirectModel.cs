namespace DDApp.API.Models.Direct
{
    public class DirectModel
    {
        public Guid DirectId { get; set; }
        public string DirectTitle { get; set; } = null!;
        public List<DirectMemberModel> DirectMembers { get; set; } = null!;
        public DirectImageModel? DirectImage { get; set; } = null!;
    }
}
