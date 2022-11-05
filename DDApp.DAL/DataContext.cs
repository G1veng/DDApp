using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
