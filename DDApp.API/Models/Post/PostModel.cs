using DDApp.DAL.Entites;
using Microsoft.AspNetCore.Mvc;

namespace DDApp.API.Models
{
    public class PostModel
    {
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Text { get; set; } = null!;
        public virtual Guid AuthorId { get; set; }
        public List<string?>? Files { get; set; } = new List<string?>();
        public int CommentAmount { get; set; } = 0;
    }
}
