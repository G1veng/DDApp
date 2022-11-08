using DDApp.DAL.Entites;

namespace DDApp.API.Models.Post
{
    public class RequestPostModel
    {
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Text { get; set; } = null!;
        public virtual Guid AuthorId { get; set; }
        public List<PostFiles>? PostFiles { get; set; }
        public int CommentAmount { get; set; } = 0;
        public Func<PostFiles, string?>? LinkGenerator { get; set; }
    }
}
