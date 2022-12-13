using System.ComponentModel.DataAnnotations;

namespace DDApp.API.Models
{
    public class CreatePostModel
    {
        [Required]
        public string Text { get; set; } = null!;
        public List<MetadataModel> Files { get; set; } = null!;
    }
}
