using Microsoft.EntityFrameworkCore;

namespace DDApp.DAL
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Entites.User>()
                .HasIndex(f => f.Email)
                .IsUnique();

            modelBuilder.
                Entity<Entites.Avatar>()
                .ToTable(nameof(Avatars));

            modelBuilder
                .Entity<Entites.PostCommentLikes>()
                .HasKey(u => new {u.UserId, u.PostCommentId});

            modelBuilder
                .Entity<Entites.PostLikes>()
                .HasKey(u => new { u.UserId, u.PostId });
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseNpgsql(b => b.MigrationsAssembly("DDApp.API"));

        public DbSet<Entites.User> Users => Set<Entites.User>();
        public DbSet<Entites.UserSession> UserSessions => Set<Entites.UserSession>();
        public DbSet<Entites.Attach> Attaches => Set<Entites.Attach>();
        public DbSet<Entites.Avatar> Avatars => Set<Entites.Avatar>();
        public DbSet<Entites.Posts> Posts => Set<Entites.Posts>();
        public DbSet<Entites.PostComments> PostComments => Set<Entites.PostComments>();
        public DbSet<Entites.PostFiles> PostFiles => Set<Entites.PostFiles>();
        public DbSet<Entites.PostCommentLikes> PostCommentsLikes => Set<Entites.PostCommentLikes>();
        public DbSet<Entites.PostLikes> PostLikes => Set<Entites.PostLikes>();
    }
}
