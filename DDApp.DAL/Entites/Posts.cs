namespace DDApp.DAL.Entites
{
    public class Posts
    {
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Text { get; set; } = null!;
        public bool IsActive { get; set; } = true;

        public virtual ICollection<PostComments>? Comments { get; set; }
        public virtual ICollection<PostFiles>? PostFiles { get; set; }
        public virtual User Author { get; set; } = null!;
        public virtual ICollection<PostLikes>? PostLikes { get; set; }
    }
}
