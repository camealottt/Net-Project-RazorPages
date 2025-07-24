using Microsoft.EntityFrameworkCore;
using Razor_Tutorial_Test.Model;

namespace Razor_Tutorial_Test.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)
        {
        }        

        public DbSet<Category> Category { get; set; }

        public DbSet<User> User { get; set; }

        public DbSet<Items> Items { get; set; }

        public DbSet<ItemImages> ItemImages { get; set; }

        public DbSet<FollowRecord> FollowRecord { get; set; }

        public DbSet<LikeRecord> LikeRecord { get; set; }

        public DbSet<CommentRecord> CommentRecord { get; set; }

        public DbSet<OrderRecord> OrderRecord { get; set; }
        public DbSet<ChatMessage> ChatMessage { get; set; }
        public DbSet<Notification> Notifications { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Items>()
                .Property(i => i.Price)
                .HasPrecision(18, 2);

            modelBuilder.Entity<OrderRecord>()
                .Property(o => o.MoneyOffered)
                .HasPrecision(18, 2);
        }
    }
}
