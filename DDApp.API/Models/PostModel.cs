using DDApp.DAL.Entites;

namespace DDApp.API.Models
{
    public class PostModel
    {
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Text { get; set; } = null!;
        public virtual Guid AuthorId { get; set; }
        public List<string>? Files { get; set; }
    }
}
