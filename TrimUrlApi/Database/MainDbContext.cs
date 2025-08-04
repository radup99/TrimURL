using Microsoft.EntityFrameworkCore;
using TrimUrlApi.Entities;

namespace TrimUrlApi.Database
{
    public class MainDbContext : DbContext
    {
        public DbSet<ShortUrl> ShortUrls { get; set; }

        public MainDbContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite(@"Data Source=Database.db");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShortUrl>().ToTable("ShortUrls");
            modelBuilder.Entity<User>().ToTable("Users");
        }
    }
}