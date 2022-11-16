using DDApp.API.Models.MetaData;
using DDApp.DAL.Entites;
using Microsoft.AspNetCore.Mvc;

namespace DDApp.API.Models
{
    public class PostModel
    {
        public Guid Id { get; set; }
        public DateTimeOffset Created { get; set; }
        public string Text { get; set; } = null!;
        public Guid AuthorId { get; set; }
        public List<ExternalPostFileLinkModel?>? PostFiles { get; set; } = new List<ExternalPostFileLinkModel?>();
        public string? AuthorAvatar { get; set; }
        public int CommentAmount { get; set; } = 0;
    }
}
