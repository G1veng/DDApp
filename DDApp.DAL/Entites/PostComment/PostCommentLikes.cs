using System;
namespace DDApp.DAL.Entites
{
    public class PostCommentLikes
    {
        public Guid PostCommentId { get; set; }
        public Guid UserId { get; set; }

        public virtual PostComments PostComment { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }
}
