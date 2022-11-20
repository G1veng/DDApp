using System;
namespace DDApp.DAL.Entites
{
    public class PostComments
    {
        public Guid Id { get; set; }
        public string Text { get; set; } = null!;
        public DateTimeOffset Created { get; set; }
        public bool IsActive { get; set; } = true;

        public virtual Posts Post { get; set; } = null!;
        public virtual User Author { get; set; } = null!;
        public virtual ICollection<PostCommentLikes>? PostCommentLikes { get; set; }
    }
}
