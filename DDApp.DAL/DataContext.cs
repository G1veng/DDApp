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
                .HasKey(x => new {x.UserId, x.PostCommentId });

            modelBuilder
                .Entity<Entites.PostLikes>()
                .HasKey(x => new { x.UserId, x.PostId });

            modelBuilder
                .Entity<Entites.Subscriptions>()
                .HasKey(x => new {x.SubscriberId, x.SubscriptionId});

            modelBuilder.
                Entity<Entites.Subscriptions>()
                .HasOne(x => x.UserSubscriber)
                .WithMany(x => x.Subscribers)
                .HasForeignKey(x => x.SubscriberId);

            modelBuilder.
                Entity<Entites.Subscriptions>()
                .HasOne(x => x.UserSubscription)
                .WithMany(x => x.Subscriptions)
                .HasForeignKey(x => x.SubscriptionId);

            modelBuilder.
                Entity<Entites.DirectMessages>()
                .HasKey(x => x.DirectMessageId);

            modelBuilder
                .Entity<Entites.DirectMembers>()
                .HasKey(x => new { x.DirectId, x.UserId });

            modelBuilder
                .Entity<Entites.DirectMessages>()
                .HasOne(x => x.User)
                .WithMany(x => x.DirectMessages)
                .HasForeignKey(x => x.SenderId);

            modelBuilder
                .Entity<Entites.DirectFiles>()
                .HasOne(x => x.DirectMessage)
                .WithMany(x => x.DirectFiles)
                .HasForeignKey(x => x.DirectMessagesId);
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
        public DbSet<Entites.Subscriptions> Subscriptions => Set<Entites.Subscriptions>();
        public DbSet<Entites.Direct> Directs => Set<Entites.Direct>();
        public DbSet<Entites.DirectMessages> DirectMessages => Set<Entites.DirectMessages>();
        public DbSet<Entites.DirectMembers> DirectMembers => Set<Entites.DirectMembers>();
        public DbSet<Entites.DirectFiles> DirectFiles => Set<Entites.DirectFiles>();
    }
}
