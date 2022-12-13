using System.ComponentModel.DataAnnotations;

namespace DDApp.API.Models
{
    public class CreatePostCommentModel
    {
        public Guid Id { get; set; }
        [Required]
        public string Text { get; set; } = null!;
        public DateTimeOffset Created { get; set; }
        public int Likes { get; set; } = 0;
        public Guid PostId { get; set; }
    }
}
