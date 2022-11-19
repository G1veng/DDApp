namespace DDApp.DAL.Entites
{
    public class User
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "empty";
        public string Email { get; set; } = "empty";
        public string PasswordHash { get; set; } = "empty";
        public DateTimeOffset BirthDate { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTimeOffset Created { get; set; }

        public virtual Avatar? Avatar { get; set; }
        public virtual ICollection<UserSession>? Session { get; set; }
        public virtual ICollection<Posts>? Posts { get; set; }
        public virtual ICollection<PostLikes>? PostLikes { get; set; }
        public virtual ICollection<PostCommentLikes>? PostCommentLikes { get; set; }
        public virtual ICollection<Subscriptions>? Subscriptions { get; set; }
        public virtual ICollection<Subscriptions>? Subscribers { get; set; }
    }
}
