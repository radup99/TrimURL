using Microsoft.EntityFrameworkCore;
using TrimUrlApi.Entities;

namespace TrimUrlApi.Database
{
    public class MainDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<ShortUrl> ShortUrls { get; set; }

        public MainDbContext(DbContextOptions<MainDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ShortUrl>().ToTable("ShortUrls");
            modelBuilder.Entity<User>().ToTable("Users");
        }
    }
}