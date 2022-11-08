using System.ComponentModel.DataAnnotations;

namespace DDApp.API.Models
{
    public class CreatePostCommentModel
    {
        [Required]
        public Guid PostId { get; set; }
        [Required]
        public string Text { get; set; } = null!;
    }
}
