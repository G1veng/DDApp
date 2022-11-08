namespace DDApp.API.Models
{
    public class PostCommentModel
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
        public DateTimeOffset Created { get; set; }
        public int Likes { get; set; } = 0;
        public Guid AuthorId { get; set; }
    }
}
