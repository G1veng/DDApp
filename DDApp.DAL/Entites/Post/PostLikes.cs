namespace DDApp.DAL.Entites
{
    public class PostLikes
    {
        public Guid PostId { get; set; }
        public Guid UserId { get; set; }

        public virtual Posts Post { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
