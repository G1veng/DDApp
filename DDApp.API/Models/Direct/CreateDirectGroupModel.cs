using System.ComponentModel.DataAnnotations;

namespace DDApp.API.Models.Direct
{
    public class CreateDirectGroupModel
    {
        [Required]
        public string Title { get; set; } = null!;
        public MetadataModel? GroupImage { get; set; }
        public List<Guid> Members { get; set; } = null!;
    }
}
