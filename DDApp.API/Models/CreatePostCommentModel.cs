namespace DDApp.API.Models
{
    public class CreatePostCommentModel
    {
        public Guid PostId { get; set; }
        public string Text { get; set; } = null!;
    }
}
