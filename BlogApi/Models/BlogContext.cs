using Microsoft.EntityFrameworkCore;

namespace BlogApi.Models
{

    public partial class BlogContext : DbContext
    {
        public BlogContext(DbContextOptions
        <BlogContext> options)
            : base(options)
        {
        }
        public virtual DbSet<BlogApplications> BlogApplication { get; set; }
        public virtual DbSet<UserDetails> UserDetail { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlogApplications>(entity => {
                entity.HasKey(k => k.BlogId);
            });

            modelBuilder.Entity<UserDetails>(entity => {
                entity.HasKey(k => k.UserId);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
