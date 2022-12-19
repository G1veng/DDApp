using System.ComponentModel.DataAnnotations;

namespace DDApp.API.Models
{
    public class CreatePostModel
    {
        public Guid? Id { get; set; }
        [Required]
        public string Text { get; set; } = null!;
        [Required]
        public List<MetadataModel> Files { get; set; } = null!;
    }
}
